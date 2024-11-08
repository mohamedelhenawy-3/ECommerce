using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ModelClasses;
using ModelClasses.ViewModels;
using DataBaseAccess;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ECommerce.Utilty;

namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger,AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }


        public IActionResult Index(string? SearchByName ,string? SearchByCategory)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var count = _context.UserCarts.Where(x => x.UserId.Contains(userId)).Count();
            HttpContext.Session.SetInt32(CartCount.sessionCount, count);

            HomeViewModel HomeViewmodel = new HomeViewModel();
            if(SearchByName != null)
            {
                HomeViewmodel.Products = _context.Products.Include(v => v.ImgUrls).Where(v => EF.Functions.Like(v.Name, $"%{SearchByName}")).ToList();
                HomeViewmodel.Categories = _context.Categories.ToList();

            }else if(SearchByCategory != null)
            {
                var category = _context.Categories.FirstOrDefault(c => c.Name == SearchByCategory);
                HomeViewmodel.Products = _context.Products.Include(v => v.ImgUrls).Where(c => c.CategoryId == category.Id).ToList();
                HomeViewmodel.Categories = _context.Categories.Where(c => c.Name.Contains(SearchByCategory)).ToList();

            }
            else
            {
                HomeViewmodel.Products = _context.Products.Include(v => v.ImgUrls).ToList();
                HomeViewmodel.Categories = _context.Categories.ToList();

            }



            return View(HomeViewmodel);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Users()
        {
            return View();
        }
    }
}
