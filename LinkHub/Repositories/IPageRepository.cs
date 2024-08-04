using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface IPageRepository
    {
        List<Page> GetPages();
        Page GetPage(int id);
        Task<Page> Add(Page page);
        Task<Page> Update(Page page);
        bool Delete(int id);
    }
}
