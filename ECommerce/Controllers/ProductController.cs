using DataBaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.ViewModels;


namespace ECommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDBContext _app;
        private readonly IWebHostEnvironment _webhost;
        public ProductController(AppDBContext app, IWebHostEnvironment webHost)
        {
            _app = app;
            _webhost = webHost;
        }
        public IActionResult Index()
        {
            var products = _app.Products.Include(c => c.Category)
                               .Include(x => x.ImgUrls).ToList();
            return View(products);
        }
  
        [HttpGet]
        public IActionResult Create()
        {
                ProductViewModel ProductViewModel = new ProductViewModel()
                {
                    Inventories = new Inventory(),
                    PImage = new PImages(),
                    CategoriesList = _app.Categories.ToList().Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }).ToList() // Convert to List for compatibility
                };
            return View(ProductViewModel);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ProductViewModel productvm)
        {
            // Initialize the image URL string
            string homeImgUrl = "";

            // Check if any images are uploaded
            if (productvm.Images != null && productvm.Images.Any())
            {
                foreach (var image in productvm.Images)
                {
                    if (image.FileName.Contains("Home"))
                    {
                        homeImgUrl = UploadFile(image);
                        break; 
                    }
                }   
            }
            productvm.Product.HomeImgUrl = homeImgUrl;
            await _app.AddAsync(productvm.Product);
            await _app.SaveChangesAsync();

           
            var newProduct = await _app.Products.Include(u => u.Category)
                                                .FirstOrDefaultAsync(s => s.Name == productvm.Product.Name);
            productvm.Inventories.Name = newProduct.Name;
            productvm.Inventories.Category = newProduct.Category.Name;
            await _app.AddAsync(productvm.Inventories);
            await _app.SaveChangesAsync();

            if (productvm.Images != null && productvm.Images.Any())
            {
                foreach (var image in productvm.Images)
                {
              
                    string Tempfile = image.FileName;
                    if (!image.FileName.Contains("Home"))
                    {
                        string stringfileName = UploadFile(image);
                        var addressImge = new PImages
                        {
                            ImageUrl = stringfileName,
                            ProductId = newProduct.Id,
                            ProductName = newProduct.Name
                        };
                        await _app.PImages.AddAsync(addressImge);
                      
                    }
                }


            }
            await _app.SaveChangesAsync();

            return RedirectToAction("Index", "Product");
        }
      [HttpGet]
        public IActionResult Edit(int Id)
        {
            // Initialize the ProductViewModel with the Product and Categories
            ProductViewModel productviewmodel = new ProductViewModel()
            {
                Product = _app.Products.FirstOrDefault(c => c.Id == Id),
                CategoriesList = _app.Categories.ToList().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
            };

            // Check if the Product is found
            if (productviewmodel.Product == null)
            {
                return NotFound(); // Handle the case where the product is not found
            }

            // Ensure ImgUrls is initialized before assignment
            productviewmodel.Product.ImgUrls = _app.PImages.Where(x => x.ProductId == Id).ToList() ?? new List<PImages>();

            return View(productviewmodel);
        }
        [HttpPost]
        public IActionResult Edit(ProductViewModel model)
        {
            // Find the existing product in the database
            var product = _app.Products.FirstOrDefault(p => p.Id == model.Product.Id);

            if (product == null)
            {
                return NotFound(); // If product is not found, return 404
            }

            // Update only fields that have been modified
            if (!string.IsNullOrWhiteSpace(model.Product.Name))
            {
                product.Name = model.Product.Name;
            }
            if (model.Product.Price != null)
            {
                product.Price = model.Product.Price;
            }
            if (!string.IsNullOrWhiteSpace(model.Product.Description))
            {
                product.Description = model.Product.Description;
            }
            if (model.Product.CategoryId != null)
            {
                product.CategoryId = model.Product.CategoryId;
            }

            // Handle new image uploads, if any
            if (model.Images != null && model.Images.Any())
            {
                foreach (var imageFile in model.Images)
                {
                    // Use the helper method to upload each file
                    string imageUrl = UploadFile(imageFile);

                    // Create a new PImage instance with required fields set
                    var newImage = new PImages
                    {
                        ProductId = product.Id,
                        ImageUrl = imageUrl,
                        ProductName = product.Name // Ensure ProductName is set to avoid NULL constraint issue
                    };
                    _app.PImages.Add(newImage); // Add new images to the database
                }
            }

            // Save changes to the database
            _app.SaveChanges();

            // Redirect to a confirmation page or back to the product list
            return RedirectToAction("Index"); // Change "Index" to your desired redirection action
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Delete(int id)
        {
            if (ModelState.IsValid) // Ensure the input is valid
            {
                Product product = _app.Products.FirstOrDefault(c => c.Id == id);
                // Fetch the category to check if it exists
                if (product != null)
                {
                    _app.Products.Remove(product);
                    _app.SaveChanges(); // Remove the category from the database
                    return RedirectToAction("Index"); // Redirect to the category list page
                }
                else
                {
                    ModelState.AddModelError("", "product not found."); // Add error if category doesn't exist
                }
            }

            return View("Home"); /// If model is invalid, return the form with validation messages
        }

        //// Helper method for uploading files
        private string UploadFile(IFormFile image)
        {
            string fileName = null;

            if (image != null)
            {
                // Get the path to the Images folder within wwwroot
                string uploadDirectory = Path.Combine(_webhost.WebRootPath, "Images");

                // Generate a unique file name using a GUID
                fileName = Guid.NewGuid().ToString() + "_" + image.FileName;

                // Combine the directory and file name to get the full path
                string filePath = Path.Combine(uploadDirectory, fileName);

                // Use a file stream to save the image to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }

            return fileName; // Return the file name to be saved in the database
        }

    }
}
