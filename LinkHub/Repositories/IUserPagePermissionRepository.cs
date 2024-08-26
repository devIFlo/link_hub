using LinkHub.Models;

namespace LinkHub.Repositories
{
    public interface IUserPagePermissionRepository
    {
        Task<UserPagePermission> Add(UserPagePermission userPagePermission);
        Task<bool> Delete(string userId, int pageId);
        Task<UserPagePermission?> GetUserPagePermissionAsync(string userId, int pageId);
    }
}