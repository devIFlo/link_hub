using LinkHub.Data;
using LinkHub.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkHub.Repositories
{
    public class PageRepository : IPageRepository
    {
        private readonly ApplicationDbContext _context;

        public PageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Page> GetPageAsync(int id)
        {
            return await _context.Pages.FirstOrDefaultAsync(p => p.Id == id);
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
            Page pageDB = await GetPageAsync(page.Id);

            pageDB.Name = page.Name;
            pageDB.Description = page.Description;

            _context.Pages.Update(pageDB);
            await _context.SaveChangesAsync();

            return pageDB;
        }

        public async Task<bool> Delete(int id)
        {
            Page pageDB = await GetPageAsync(id);

            if (pageDB == null) throw new Exception("Houve um erro na deleção da Página!");

            _context.Pages.Remove(pageDB);
            _context.SaveChanges();

            return true;
        }
    }
}
