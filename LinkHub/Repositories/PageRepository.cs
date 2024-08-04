using LinkHub.Data;
using LinkHub.Models;

namespace LinkHub.Repositories
{
    public class PageRepository : IPageRepository
    {
        private readonly ApplicationDbContext _context;

        public PageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Page GetPage(int id)
        {
            return _context.Pages.FirstOrDefault(p => p.Id == id);
        }

        public List<Page> GetPages()
        {
            return _context.Pages.ToList();
        }

        public async Task<Page> Add(Page page)
        {
            _context.Pages.Add(page);
            await _context.SaveChangesAsync();

            return page;
        }

        public async Task<Page> Update(Page page)
        {
            Page pageDB =  GetPage(page.Id);

            pageDB.Name = page.Name;
            pageDB.Description = page.Description;

            _context.Pages.Update(pageDB);
            await _context.SaveChangesAsync();

            return pageDB;
        }

        public bool Delete(int id)
        {
            Page pageDB = GetPage(id);

            if (pageDB == null) throw new Exception("Houve um erro na deleção da Página!");

            _context.Pages.Remove(pageDB);
            _context.SaveChanges();

            return true;
        }
    }
}
