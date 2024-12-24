using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface ILinkRepository
    {
        Task<List<Link>> GetLinksAsync();
        Task<Link?> GetLinkAsync(int id);
        Task<List<Link>> GetLinksPerPageAsync(string page);
        Task<List<Link>> GetLinksPerUserAsync(string userId);
        Task<List<Link>> GetLinksForHomePageAsync(string page);
        List<Category> GetCategories();
        Task<Link> Add(Link link);
        Task<Link> Update(Link link);
        Task<bool> DeleteAsync(Link link);
    }
}