using LinkHub.Data;
using LinkHub.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LinkHub.Repositories
{
    public class PageRepository : IPageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PageRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Page?> GetPageAsync(int id)
        {
            return await _context.Pages.FirstOrDefaultAsync(p => p.Id == id);
        }

        public List<Page> GetPages()
        {
            return _context.Pages.ToList();
        }

        public async Task<List<Page>> GetPagePerUserAsync(string userId)
        {
            return await _context.Pages
                .Where(p => _context.UserPagePermissions
                    .Any(upp => upp.PageId == p.Id && upp.UserId == userId))
                .ToListAsync();
        }

        public async Task<Page> Add(Page page)
        {
            _context.Pages.Add(page);
            await _context.SaveChangesAsync();

            var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            Log.Information("O usuário {CurrentUser} adicionou a página {PageName} (ID: {PageId}) em {Timestamp}",
                    currentUser, page.Name, page.Id, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return page;
        }

        public async Task<Page> Update(Page page)
        {
            var pageDB = await GetPageAsync(page.Id);

            if (pageDB == null) throw new Exception("Ocorreu um erro ao atualizar a página!");

            pageDB.Name = page.Name;
            pageDB.Description = page.Description;

            _context.Pages.Update(pageDB);
            await _context.SaveChangesAsync();

            var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            Log.Information("O usuário {CurrentUser} alterou a página (ID: {PageId}) em {Timestamp}",
                    currentUser, page.Id, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return pageDB;
        }

        public async Task<bool> Delete(int id)
        {
            var page = await GetPageAsync(id);

            if (page == null) throw new Exception("Ocorreu um erro ao remover a página!");

            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();

            var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            Log.Information("O usuário {CurrentUser} deletou a página {PageName} (ID: {PageId}) em {Timestamp}",
            currentUser, page.Name, page.Id, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return true;
        }
    }
}
