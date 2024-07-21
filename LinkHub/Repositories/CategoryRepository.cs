using LinkHub.Data;
using LinkHub.Models;

namespace LinkHub.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.FirstOrDefault(x => x.Id == id);
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public async Task<Category> Add(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category> Update(Category category)
        {
            Category categoryDB = GetCategory(category.Id);

            categoryDB.Name = category.Name;

            _context.Categories.Update(categoryDB);
            await _context.SaveChangesAsync();

            return categoryDB;
        }

        public bool Delete(int id)
        {
            Category categoryDB = GetCategory(id);

            if (categoryDB == null) throw new Exception("Houve um erro na deleção da Categoria!");

            _context.Categories.Remove(categoryDB);
            _context.SaveChanges();

            return true;
        }
    }
}
