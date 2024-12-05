using LinkHub.Data;
using LinkHub.Models;
using LinkHub.ViewModels;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LinkHub.Repositories
{
    public class UserPagePermissionRepository : IUserPagePermissionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserPagePermissionRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserPagePermission>> GetPermissionPerPageAsync(int pageId)
        {
            return await _context.UserPagePermissions
                .Where(p => p.PageId == pageId)
                .ToListAsync();
        }

        public async Task<List<string>> GetUsersPerPageAsync(int pageId)
        {
            return await _context.UserPagePermissions
                .Where(x => x.PageId == pageId)
                .Select(x => x.UserId ?? "")
                .ToListAsync();
        }

        public async Task<bool> Update(int pageId, List<string> userIds)
        {
            var existingPermissions = await GetPermissionPerPageAsync(pageId);
                        
            var newPermissions = new List<UserPagePermission>();

            foreach (var userId in userIds)
            {
                var existingPermission = existingPermissions
                    .FirstOrDefault(x => x.UserId == userId);

                if (existingPermission == null)
                {
                    newPermissions.Add(new UserPagePermission
                    {
                        UserId = userId,
                        PageId = pageId
                    });
                }
            }                     

            var permissionsToRemove = existingPermissions
                .Where(x => x.UserId != null && !userIds.Contains(x.UserId))
                .ToList();            

            _context.UserPagePermissions.RemoveRange(permissionsToRemove);
            _context.UserPagePermissions.AddRange(newPermissions);

            await _context.SaveChangesAsync();

            var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            Log.Information("O usuário {CurrentUser} alterou as permissões da página (ID: {PageId}) em {Timestamp}",
                    currentUser, pageId, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return true;
        }

        public async Task<bool> DeleteAllPagePermissions(int pageId)
        {
            var existingPermission = _context.UserPagePermissions
                .Where(p => p.PageId == pageId) ?? throw new Exception("Houve um erro ao remover as permissões!");

            _context.UserPagePermissions.RemoveRange(existingPermission);
            await _context.SaveChangesAsync();

            var currentUser = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            Log.Information("O usuário {CurrentUser} deletou todas as permissões da página (ID: {PageId}) em {Timestamp}",
                    currentUser, pageId, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            return true;
        }
    }
}