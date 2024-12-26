using LinkHub.Data;
using LinkHub.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LinkHub.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LinkRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Link?> GetLinkAsync(int id)
        {
            return await _context.Links
                .Include(l => l.Category)
                .ThenInclude(c => c!.Page)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Link>> GetLinksAsync()
        {
            return await _context.Links.ToListAsync();
        }

        public async Task<List<Link>> GetLinksPerPageAsync(string page)
        {
            return await _context.Links
                .Where(l => l.Category != null && l.Category.Page != null && l.Category.Page.Name == page)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<List<Link>> GetLinksPerUserAsync(string userId)
        {
            return await _context.Links
                .Include(l => l.Category)
                .ThenInclude(c => c!.Page)
                .Where(c => _context.UserPagePermissions
                    .Any(upp => c.Category != null && upp.PageId == c.Category.PageId && upp.UserId == userId))
                .ToListAsync();
        }

        public async Task<List<Link>> GetLinksForHomePageAsync(string page)
        {
            var links = await _context.HomePages
                .Where(h => h.Page != null && h.Page.Name == page)
                .Select(h => h.Link!)
                .ToListAsync();

            return links ?? new List<Link>();
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public async Task<Link> Add(Link link)
        {
            _context.Links.Add(link);
            await _context.SaveChangesAsync();

            var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            Log.Information("O usuário {CurrentUser} adicionou o link {LinkName} (ID: {LinkId}) em {Timestamp}",
                    currentUser, link.Name, link.Id, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return link;
        }

        public async Task<Link> Update(Link link)
        {
            var linkDB = await GetLinkAsync(link.Id);

            if (linkDB == null) throw new Exception("Houve um erro na atualização do Serviço!");

            linkDB.Name = link.Name;
            linkDB.Description = link.Description;
            linkDB.CategoryId = link.CategoryId;
            linkDB.Url = link.Url;
            linkDB.FileName = link.FileName;

            _context.Links.Update(linkDB);
            await _context.SaveChangesAsync();

            var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            Log.Information("O usuário {CurrentUser} atualizou o link (ID: {LinkId}) em {Timestamp}",
                    currentUser, link.Id, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return linkDB;
        }

        public async Task<bool> DeleteAsync(Link link)
        {
            _context.Links.Remove(link);
            await _context.SaveChangesAsync();

            var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            Log.Information("O usuário {CurrentUser} deletou o link {LinkName} (ID: {LinkId}) em {Timestamp}",
                    currentUser, link.Name, link.Id, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return true;
        }
    }
}