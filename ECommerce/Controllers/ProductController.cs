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
                    // Check if the file name contains "Home" (additional logic can be added here if needed)
                    if (image.FileName.Contains("Home"))
                    {
                        homeImgUrl = UploadFile(image);
                        break; // Assuming only the first image is used as the Home image
                    }
                }

                   
            }

            // Assign the HomeImgUrl to the Product
            productvm.Product.HomeImgUrl = homeImgUrl;

            // Add the new product to the database
            await _app.AddAsync(productvm.Product);
            await _app.SaveChangesAsync();

            // Retrieve the newly created product along with its category
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
                    // Check if the file name contains "Home" (additional logic can be added here if needed)
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
