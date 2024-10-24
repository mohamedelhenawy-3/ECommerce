using ECommerce.Utilty;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses.ViewModels;
using ModelClasses;
using DataBaseAccess;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace ECommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDBContext _context;
        public OrderController(AppDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
     
        [Authorize]
        [Route("Order/OrderDetailsReview/{id?}")]
        public IActionResult OrderDetailsReview(string id)
        {
            // Use the passed UserId or fallback to the currently authenticated user's ID
            var userId = id;

            // Make sure you have the correct parameter
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User ID is null or empty. User might not be authenticated properly.");
            }

            // Retrieve the current user from the database
            var Current = _context.AppUser.FirstOrDefault(x => x.Id == userId);

            // Initialize the OrderViewModel
            OrderViewModel orderviewmodel = new OrderViewModel()
            {
                UserCartList = _context.UserCarts
                    .Include(x => x.Product)
                    .ThenInclude(v => v.ImgUrls)
                    .Where(x => x.UserId == userId)
                    .ToList(),
                UserOrderHeader = new UserOrderHeader(),
                UserCartId = userId
            };

            // If the user data is successfully retrieved
            if (Current != null)
            {
                orderviewmodel.UserOrderHeader.DeliveryStreetAddress = Current.Address;
                orderviewmodel.UserOrderHeader.CIty = Current.City;
                orderviewmodel.UserOrderHeader.State = Current.City;
                orderviewmodel.UserOrderHeader.PostalCode = Current.PostalCode;
                orderviewmodel.UserOrderHeader.Name = Current.FirstName + " " + Current.LastName;
               
            }

            // Update cart count
            var count = _context.UserCarts.Where(u => u.UserId == userId).Count();
            HttpContext.Session.SetInt32(CartCount.sessionCount, count);

            // Return the view with the OrderViewModel
            return View(orderviewmodel);
        }

    }
}
