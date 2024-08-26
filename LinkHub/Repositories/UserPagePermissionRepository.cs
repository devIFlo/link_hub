using LinkHub.Data;
using LinkHub.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkHub.Repositories
{
    public class UserPagePermissionRepository : IUserPagePermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public UserPagePermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserPagePermission> Add(UserPagePermission userPagePermission)
        {
            _context.UserPagePermissions.Add(userPagePermission);
            await _context.SaveChangesAsync();

            return userPagePermission;
        }

        public async Task<bool> Delete(string userId, int pageId)
        {
            var userPagePermission = await GetUserPagePermissionAsync(userId, pageId);

            if (userPagePermission == null) throw new Exception("Houve um erro ao remover as permissões!");

            _context.UserPagePermissions.Remove(userPagePermission);
            _context.SaveChanges();

            return true;
        }

        public async Task<UserPagePermission?> GetUserPagePermissionAsync(string userId, int pageId)
        {
            return await _context.UserPagePermissions
                .FirstOrDefaultAsync(x => x.UserId == userId && x.PageId == pageId);
        }
    }
}