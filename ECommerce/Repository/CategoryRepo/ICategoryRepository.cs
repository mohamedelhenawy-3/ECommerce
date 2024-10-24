using ModelClasses;

namespace ECommerce.Repository.CategoryRepo
{
    public interface ICategoryRepository
    {


        public List<Category> GetAllCategory();
        public Category? CategoryById(int id);
        public void AddCategory(Category category);
        public void RemoveCategory(int id);
        public void UpdateCategory(Category category, int id);

    }
}
