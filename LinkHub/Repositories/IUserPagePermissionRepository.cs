using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface IUserPagePermissionRepository
    {
        Task<List<UserPagePermission>> GetPermissionPerPageAsync(int pageId);
        Task<List<string>> GetUsersPerPageAsync(int pageId);
        Task<bool> Update(int pageId, List<string> userIds);
        Task<bool> DeleteAllPagePermissions(int pageId);
    }
}