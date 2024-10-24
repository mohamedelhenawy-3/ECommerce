using DataBaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.ViewModels;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    public class UserCartController : Controller
    {
        private readonly AppDBContext _context;
        public UserCartController(AppDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int ProductId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var existingCart = await _context.UserCarts.Where(x => x.UserId.Contains(userId)).ToListAsync();
                if (existingCart.Count > 0)
                {
                    var getproductincart = existingCart.FirstOrDefault(x => x.ProductId == ProductId);
                    if (getproductincart != null)
                    {
                        getproductincart.quantity += 1;
                        _context.UserCarts.Update(getproductincart);

                    }
                    else
                    {
                        var newCart = new UserCart()
                        {
                            UserId = userId,
                            ProductId = ProductId,
                            quantity = 1

                        };
                        _context.UserCarts.AddAsync(newCart);

                    }
                }
                else
                {
                    var newUserCart = new UserCart()
                    {
                        UserId = userId,
                        ProductId = ProductId,
                        quantity = 1

                    };
                    _context.UserCarts.AddAsync(newUserCart);
                };
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public IActionResult ShowCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID

            // Create a CartViewModel and pass the data
            CartViewModel model = new CartViewModel
            {
                ProductList = _context.UserCarts
                                      .Include(uc => uc.Product) // Include the product details
                                          .ThenInclude(p => p.Category) // Include the product's category
                                      .Include(uc => uc.Product) // Include product again for image URLs
                                          .ThenInclude(p => p.ImgUrls) // Include product images
                                      .Where(c => c.UserId == userId) // Filter by user ID
                                      .ToList() // Keep it as a list of UserCart
            };

            return View(model);
        }

        [Authorize]
        public IActionResult IncreaseQuantity(int ProductId)
        {
            var productquantity = _context.UserCarts.FirstOrDefault(x => x.ProductId == ProductId);
            if (productquantity != null)
            {
                productquantity.quantity = productquantity.quantity + 1;
                _context.Update(productquantity);
            }
            _context.SaveChanges();


            return RedirectToAction("ShowCart", "UserCart");
        }
        [Authorize]
        public IActionResult DecrementQuantity(int ProductId)
        {
            var productquantity = _context.UserCarts.FirstOrDefault(x => x.ProductId == ProductId);
            if (productquantity != null)
            {
                if (productquantity.quantity - 1 < 0)
                {
                    _context.Remove(productquantity);
                }
                else
                {
                    productquantity.quantity = productquantity.quantity - 1;
                    _context.Update(productquantity);

                }
                _context.SaveChanges();
            }

            return RedirectToAction("ShowCart", "UserCart");

        }

        [Authorize]
        public IActionResult DeleteItem(int ProductId)
        {
            var product = _context.UserCarts.FirstOrDefault(x => x.ProductId == ProductId);
            if (product != null)
            {
                _context.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("ShowCart", "UserCart");

        }


    }
}

