using ECommerce.Utilty;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses.ViewModels;
using ModelClasses;
using DataBaseAccess;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stripe.Checkout;


namespace ECommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDBContext _context;
        [BindProperty]
        public OrderDetialsViewModel OrderDetailsVM { get; set; }

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

        //ORDER DONE 
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> OrderDone(OrderViewModel orderviewmodel)
        {
            // Step 1: Retrieve the current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID is null or empty.");
                return RedirectToAction("Error", "Home");
            }

            // Step 2: Retrieve the current user from the database
            var currentUser = await _context.AppUser.FirstOrDefaultAsync(x => x.Id == userId);
            OrderViewModel OVM = new OrderViewModel()
            {
                UserCartList = _context.UserCarts.Include(x => x.Product).Where(x => x.UserId.Contains(userId)).ToList(),
                UserOrderHeader = new UserOrderHeader()
            };
            if (currentUser != null)
            {
                OVM.UserOrderHeader.TotalAmount = OVM.UserCartList.Sum(cart => (double)cart.Product.Price * cart.quantity);

                // Validate the TotalAmount
                if (OVM.UserOrderHeader.TotalAmount <= 0)
                {
                    ModelState.AddModelError("", "Total amount must be greater than zero.");
                    return View(orderviewmodel); // Return to the view with an error message
                }

                OVM.UserOrderHeader.DeliveryStreetAddress = orderviewmodel.UserOrderHeader.DeliveryStreetAddress;
                OVM.UserOrderHeader.CIty = orderviewmodel.UserOrderHeader.CIty;
                OVM.UserOrderHeader.State = orderviewmodel.UserOrderHeader.State;
                OVM.UserOrderHeader.PostalCode = orderviewmodel.UserOrderHeader.PostalCode;
                OVM.UserOrderHeader.Name = orderviewmodel.UserOrderHeader.Name;
                OVM.UserOrderHeader.DateOfOrder = DateTime.Now;
                OVM.UserOrderHeader.PhoneNumber = orderviewmodel.UserOrderHeader.PhoneNumber;
                OVM.UserOrderHeader.OrderState = "Pending";
                OVM.UserOrderHeader.PaymentState = "Not Paid";
                OVM.UserOrderHeader.UserId = userId; // Ensure UserId is properly set
                await _context.UserOrderHeaders.AddAsync(OVM.UserOrderHeader);
                await _context.SaveChangesAsync();
            }


            // Step 6: Validate and handle payment with Stripe
            if (OVM.UserOrderHeader.TotalAmount > 0)
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = "https://localhost:7018/Order/OrderSuccess/" + OVM.UserOrderHeader.Id,
                    CancelUrl = "https://localhost:7018/Order/OrderCancel/" + OVM.UserOrderHeader.Id,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"

                };

                foreach (var item in OVM.UserCartList)
                {
                    if (item.quantity > 0) // Ensure quantity is greater than zero
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Product.Price * 100), // Convert price to cents
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Name,
                                    Description = item.Product.Description
                                }
                            },
                            Quantity = item.quantity
                        };
                        options.LineItems.Add(sessionLineItem);
                    }
                }

                try
                {
                    // Create Stripe payment session
                    var service = new SessionService();
                    Session session =  service.Create(options); // Ensure you're using async method
                    Console.WriteLine("Stripe session created successfully: " + session);

                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Stripe error: {ex.Message}");
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                Console.WriteLine("Total amount is zero or not set.");
                return RedirectToAction("Error", "Home");
            }


            return RedirectToAction("Index", "Home");


        }

        public IActionResult OrderSuccess(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var removeCart = _context.UserCarts.Where(x => x.UserId.Contains(userId)).ToList();
            var orderProcess = _context.UserOrderHeaders.FirstOrDefault(x => x.Id == id);


            if (orderProcess != null)
            {
                if (orderProcess.PaymentState == "Not Paid")
                {
                    orderProcess.PaymentState = "Paid";
                    orderProcess.PaymentProcessDate = DateTime.Now;
                }
            }
            foreach (var item in removeCart)
            {
                OrderDetails orderdetails = new OrderDetails()
                {
                    OrderHeaderId = orderProcess.Id,
                    ProductId = (int)item.ProductId,
                    Count = item.quantity

                };
                _context.OrderDetails.Add(orderdetails);
            }
            ViewBag.OrderId = id;
            _context.UserCarts.RemoveRange(removeCart);
            _context.SaveChanges();
            var count = _context.UserCarts.Where(u => u.UserId.Contains(userId)).ToList().Count();
            HttpContext.Session.SetInt32(CartCount.sessionCount, count);


            return View();
        }
        public IActionResult OrderCancel(int id)
        {
            var orderProcessCancel = _context.UserOrderHeaders.FirstOrDefault(x => x.Id == id);
            _context.UserOrderHeaders.Remove(orderProcessCancel);
            _context.SaveChanges();

            return RedirectToAction("ShowCart", "Cart");
        }

        public IActionResult OrderHistory(string? Status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<UserOrderHeader> ListOfOrder = new List<UserOrderHeader>();
            if(Status != null && Status != "All")
            {
                if (User.IsInRole("Admin"))
                {
                    ListOfOrder = _context.UserOrderHeaders.Where(u => u.OrderState == Status).ToList();
                }
                else
                {
                    ListOfOrder = _context.UserOrderHeaders.Where(u => u.OrderState == Status && u.UserId.Contains(userId)).ToList();
                }
            }
            else
            {
                if (User.IsInRole("Admin"))
                {
                    ListOfOrder = _context.UserOrderHeaders.ToList();
                }
                else
                {
                    ListOfOrder = _context.UserOrderHeaders.Where(u => u.UserId.Contains(userId)).ToList();
                }
            }
            return View(ListOfOrder);
        }

        public IActionResult OrderDetails(int orderId)
        {
            var OrderDetailsVM = new OrderDetialsViewModel
            {
                UserOrderHeader = _context.UserOrderHeaders.FirstOrDefault(x => x.Id == orderId),
                OrderDetails = _context.OrderDetails
                    .Include(x => x.Product)
                    .Where(x => x.OrderHeaderId == orderId)
                    .ToList()
            };
            return View(OrderDetailsVM);
        }

    }
}
