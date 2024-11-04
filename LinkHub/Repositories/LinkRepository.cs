using LinkHub.Data;
using LinkHub.Models;
using LinkHub.Services;
using Microsoft.EntityFrameworkCore;

namespace LinkHub.Repositories
{
    public class LinkRepository : ILinkRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageStorage _imageStorage;

        public LinkRepository(ApplicationDbContext context, ImageStorage imageStorage)
        {
            _context = context;
            _imageStorage = imageStorage;
        }

        public async Task<Link> GetLinkAsync(int id)
        {
            return await _context.Links
                .Include(l => l.Category)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Link>> GetLinksAsync()
        {
            return await _context.Links.ToListAsync();
        }

        public async Task<List<Link>> GetLinksPerPageAsync(string page)
        {
            return await _context.Links
                .Where(l => l.Category.Page.Name == page)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<List<Link>> GetLinksPerUserAsync(string userId)
        {
            return await _context.Links
           .Include(c => c.Category)
           .ThenInclude(c => c.Page)
           .Where(c => _context.UserPagePermissions
               .Any(upp => upp.PageId == c.Category.PageId && upp.UserId == userId))
           .ToListAsync();
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public async Task<Link> Add(Link link)
        {
            _context.Links.Add(link);
            await _context.SaveChangesAsync();

            return link;
        }

        public async Task<Link> Update(Link link)
        {
            Link linkDB = await GetLinkAsync(link.Id);

            if (linkDB == null) throw new Exception("Houve um erro na atualização do Serviço!");

            linkDB.Name = link.Name;
            linkDB.Description = link.Description;
            linkDB.CategoryId = link.CategoryId;
            linkDB.Url = link.Url;
            linkDB.FileName = link.FileName;

            _context.Links.Update(linkDB);
            await _context.SaveChangesAsync();

            return linkDB;
        }

        public async Task<bool> DeleteAsync(Link link)
        {
            _context.Links.Remove(link);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}