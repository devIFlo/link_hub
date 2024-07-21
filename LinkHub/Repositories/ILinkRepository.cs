using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface ILinkRepository
    {
        List<Link> GetLinks();
        Link GetLink(int id);
        List<Category> GetCategories();
        Task<Link> Add(Link link);
        Task<Link> Update(Link link);
        bool Delete(int id);
    }
}