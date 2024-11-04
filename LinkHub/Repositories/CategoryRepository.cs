using LinkHub.Data;
using LinkHub.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkHub.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Category>> GetCategories()
        {
            return await _context.Categories.Include(c => c.Page).ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesPerUserAsync(string userId)
        {
            return await _context.Categories
                .Include(c => c.Page)
                .Where(c => _context.UserPagePermissions
                    .Any(upp => upp.PageId == c.PageId && upp.UserId == userId))
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesPerPageAsync(int pageId)
        {
            return await _context.Categories
                .Include(c => c.Page)
                .Where(c => c.PageId == pageId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesPerPageAsync(string pageName)
        {
            return await _context.Categories
                .Include(c => c.Page)
                .Where(c => c.Page.Name == pageName)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            var categoryDB = await GetCategoryAsync(category.Id);

            categoryDB.Name = category.Name;

            _context.Categories.Update(categoryDB);
            await _context.SaveChangesAsync();

            return categoryDB;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await GetCategoryAsync(id);

            if (category == null) 
                throw new InvalidOperationException("A categoria não foi encontrada.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
