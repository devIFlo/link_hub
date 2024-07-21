using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        Category GetCategory(int id);
        Task<Category> Add(Category category);
        Task<Category> Update(Category category);
        bool Delete(int id);
    }
}
