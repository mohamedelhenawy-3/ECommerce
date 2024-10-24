using DataBaseAccess;
using Microsoft.EntityFrameworkCore;
using ModelClasses;

namespace ECommerce.Repository.CategoryRepo
{
    public class CategoryRepositorycs : ICategoryRepository
    {
       private readonly AppDBContext context;

        public CategoryRepositorycs(AppDBContext _context)
        {
            context = _context;
        }

        public void AddCategory(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
        }

        public List<Category> GetAllCategory()
        {
            List<Category> List = context.Categories.ToList();
            return List;
        }

        public Category? CategoryById(int id)
        {
            Category? category = context.Categories.FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return null; // or you can return a specific value like -1 to indicate not found
            }

            return category;
        }

        public void RemoveCategory(int id)
        {
            Category category = context.Categories.FirstOrDefault(c => c.Id == id);
            context.Categories.Remove(category);
            context.SaveChanges();
        }

        public void UpdateCategory(Category category, int id)
        {
            if (category != null)
            {
                Category OldCategory = context.Categories.FirstOrDefault(c => c.Id == id);
                if (OldCategory != null)
                {
                    OldCategory.Name = category.Name;
                    context.SaveChanges();
                }
            }
        }
    }
}
