using DataBaseAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.ViewModels;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signmanager;
        private readonly AppDBContext _context;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signmanager,AppDBContext context)
        {
            this._usermanager = userManager;
            this._signmanager = signmanager;
            this._context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
       
        public async Task<IActionResult> Register(RegisterViewModel registerviewmodel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerviewmodel);
            }

            ApplicationUser appuser = new ApplicationUser
            {
                UserName = registerviewmodel.Email,  // Use Email as Username
                Email = registerviewmodel.Email,
                FirstName = registerviewmodel.FirstName,
                LastName = registerviewmodel.LastName,
                Address = registerviewmodel.Address
            };

            IdentityResult result = await _usermanager.CreateAsync(appuser, registerviewmodel.Password); // Correct way to handle password
            if (result.Succeeded)
            {
                await _signmanager.SignInAsync(appuser, isPersistent: false);
                return RedirectToAction("Login", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerviewmodel);
            }
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appuser = await _usermanager.FindByEmailAsync(lvm.Email);
                if (appuser != null)
                {
                    bool found = await _usermanager.CheckPasswordAsync(appuser, lvm.Password); // This will work with hashed passwords
                    if (found)
                    {
                        await _signmanager.SignInAsync(appuser, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View(lvm);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                // Remove all UserCart items associated with this user
                var userCarts = _context.UserCarts.Where(c => c.UserId == userId);
                _context.UserCarts.RemoveRange(userCarts);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            // Sign the user out
            await _signmanager.SignOutAsync();

            // Optionally, clear session or cookies if needed
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account"); // If you have a Login action
                                                         // or 
                                                         // return RedirectToAction("Index", "Home"); // Redirect to home page
        }

    }
}
