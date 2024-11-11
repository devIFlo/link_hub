using LinkHub.Data;
using LinkHub.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LinkHub.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CategoryRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Category?> GetCategoryAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Category>> GetCategoriesAsync()
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
                .Where(c => c.Page != null && c.Page.Name == pageName)
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

            if (categoryDB == null) throw new InvalidOperationException("A categoria não foi encontrada.");

            categoryDB.Name = category.Name;

            _context.Categories.Update(categoryDB);
            await _context.SaveChangesAsync();

            return categoryDB;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await GetCategoryAsync(id);

            if (category == null) throw new InvalidOperationException("A categoria não foi encontrada.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            Log.Information("O usuário {UserName} apagou a categoria '{CategoryName}' (ID: {CategoryId}) em {Timestamp}",
                userName, category.Name, category.Id, DateTime.UtcNow);

            return true;
        }
    }
}
