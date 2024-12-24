using LinkHub.Data;
using LinkHub.Models;
using LinkHub.ViewModels;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LinkHub.Repositories
{
    public class HomePageRepository : IHomePageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomePageRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<HomePage>> GetHomePagePerLinkAsync(int linkId)
        {
            return await _context.HomePages
                .Where(h => h.LinkId == linkId)
                .ToListAsync();
        }

        public async Task<List<int>> GetPagesPerLinkAsync(int linkId)
        {
            return await _context.HomePages
                .Where(h => h.LinkId == linkId)
                .Select(h => h.PageId)
                .ToListAsync();
        }

        public async Task<bool> Update(int linkId, List<int> pagesIds)
        {
            var existingHomePages = await GetHomePagePerLinkAsync(linkId);
                        
            var newHomePages = new List<HomePage>();

            foreach (var pageId in pagesIds)
            {
                var existingHomePage = existingHomePages
                    .FirstOrDefault(x => x.PageId == pageId);

                if (existingHomePage == null)
                {
                    newHomePages.Add(new HomePage
                    {
                        PageId = pageId,
                        LinkId = linkId
                    });
                }
            }                     

            var homePagesToRemove = existingHomePages
                .Where(x => x.PageId != null && !pagesIds.Contains(x.PageId))
                .ToList();            

            _context.HomePages.RemoveRange(homePagesToRemove);
            _context.HomePages.AddRange(newHomePages);

            await _context.SaveChangesAsync();

            //var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            //Log.Information("O usuário {CurrentUser} alterou as permissões da página (ID: {PageId}) em {Timestamp}",
            //        currentUser, pageId, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return true;
        }

        public async Task<bool> DeleteAllHomePage(int linkId)
        {
            var existingHomePage = _context.HomePages
                .Where(p => p.LinkId == linkId) ?? throw new Exception("Houve um erro ao remover as páginas!");

            _context.HomePages.RemoveRange(existingHomePage);
            await _context.SaveChangesAsync();

            //var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            //Log.Information("O usuário {CurrentUser} deletou todas as permissões da página (ID: {PageId}) em {Timestamp}",
            //        currentUser, pageId, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return true;
        }
    }
}