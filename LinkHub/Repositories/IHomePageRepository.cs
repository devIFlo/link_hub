using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface IHomePageRepository
    {
        Task<List<HomePage>> GetHomePagePerLinkAsync(int linkId);
        Task<List<int>> GetPagesPerLinkAsync(int linkId);
        Task<bool> Update(int linkId, List<int> pageIds);
        Task<bool> DeleteAllHomePage(int linkId);
    }
}