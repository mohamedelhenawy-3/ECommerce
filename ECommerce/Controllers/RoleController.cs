using CustomIdentityUser.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelClasses.ViewModels;

namespace ECommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _rolemanage;


        public RoleController(RoleManager<IdentityRole> rolemaager)
        {
            this._rolemanage = rolemaager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult New()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> New(RoleViewModel rvm)
        {
            if (ModelState.IsValid == true)
            {
                IdentityRole idr = new IdentityRole();
                idr.Name = rvm.RoleName;
                //save in db 
                IdentityResult result = await _rolemanage.CreateAsync(idr);
                if (result.Succeeded == true)
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            return View(rvm);

        } 
    }
}
