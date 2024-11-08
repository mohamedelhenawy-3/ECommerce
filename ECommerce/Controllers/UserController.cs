using DataBaseAccess;
using ECommerce.Repository.CategoryRepo;
using ECommerce.Repository.UserRepo;
using Microsoft.AspNetCore.Mvc;
using ModelClasses;

namespace ECommerce.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        private readonly AppDBContext context;
        private readonly IUserRepository _userrepository;

        public UserController(AppDBContext db, IUserRepository userrepo)
        {
            context = db;
            _userrepository = userrepo;
        }

        // Change action name to Index for clarity
        [HttpGet("Index")] // You can use [HttpGet] if you want the default route
        public IActionResult Index()
        {
            var users = context.Users.ToList();
            return View(users); // Looks for Views/User/Index.cshtml
        }


        [HttpGet("User/Edit/{id}")]
        public IActionResult Edit(string id)
        {
            var user = _userrepository.UserById(id);
            if (user != null)
            {
                return View(user);
            }
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                _userrepository.UpdateUser(user, user.Id);
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpPost("User/Delete/{id}")]
        public IActionResult Delete(string id)
        {
            var user = _userrepository.UserById(id);
            if (user != null)
            {
                _userrepository.RemoveUser(id);
            }
            return RedirectToAction("Index");
        }



    }
}
