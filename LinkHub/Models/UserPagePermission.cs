using Microsoft.AspNetCore.Identity;

namespace LinkHub.Models
{
    public class UserPagePermission
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int PageId { get; set; }
        public Page Page { get; set; }

        public bool CanEdit { get; set; }
    }
}
