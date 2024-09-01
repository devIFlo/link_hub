using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        Task<Category> GetCategoryAsync(int id);
        Task<List<Category>> GetCategoriesPerUserAsync(string userId);
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
    }
}
