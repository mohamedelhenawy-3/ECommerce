using DataBaseAccess;
using ECommerce.Repository.CategoryRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses;


namespace ECommerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDBContext context;

        // **Use Dependency Injection for DBContext**

        private readonly ICategoryRepository _categoryrepo; 

        public CategoryController(AppDBContext db, ICategoryRepository categoryrepo)
        {
            context = db;
            _categoryrepo = categoryrepo;

        }

        [HttpGet]
        public IActionResult Category()
        {
            var categories = _categoryrepo.GetAllCategory();
            return View(categories);
        }


        [HttpGet]
        public IActionResult Create()
        {
            Category category = new Category();
            return View("Create", category);
        }
       
        [HttpPost]
        public IActionResult SaveChange(Category category) // The method should be named "Create" to match the view and form action
        {
           
                context.Categories.Add(category);
                context.SaveChanges();
                return RedirectToAction("Category"); // Redirect to the category list page
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Delete(int id)
        {
            if (ModelState.IsValid) // Ensure the input is valid
            {
                var category = _categoryrepo.CategoryById(id); // Fetch the category to check if it exists
                if (category != null)
                {
                    _categoryrepo.RemoveCategory(id); // Remove the category from the database
                    return RedirectToAction("Category"); // Redirect to the category list page
                }
                else
                {
                    ModelState.AddModelError("", "Category not found."); // Add error if category doesn't exist
                }
            }

            return View("Category"); /// If model is invalid, return the form with validation messages
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _categoryrepo.CategoryById(id); // Fetch the category by ID

            if (category != null)
            {
                return View(category); // Return the edit view with the category data
            }
            else
            {
                TempData["ErrorMessage"] = "Category not found."; // Store an error message temporarily
                return RedirectToAction("Category"); // Redirect to the category list page
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category,int id)
        {
            if (ModelState.IsValid) // Ensure the input is valid
            {
                _categoryrepo.UpdateCategory(category,id); // Update the category in the database
                return RedirectToAction("Category"); // Redirect to the category list page
            }
            return View(category); // If model is invalid, return the form with validation messages
        }




    }
}
