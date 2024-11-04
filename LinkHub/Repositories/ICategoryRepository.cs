using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryAsync(int id);
        Task<List<Category>> GetCategories();
        Task<List<Category>> GetCategoriesPerUserAsync(string userId);
        Task<List<Category>> GetCategoriesPerPageAsync(int pageId);
        Task<List<Category>> GetCategoriesPerPageAsync(string pageName);
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
    }
}
