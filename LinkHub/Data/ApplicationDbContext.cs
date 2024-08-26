using LinkHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkHub.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Link> Links { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<LdapSettings> LdapSettings { get; set; }
        public DbSet<UserPagePermission> UserPagePermissions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Page)
                .WithMany()
                .HasForeignKey(c => c.PageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPagePermission>()
                .HasOne(upp => upp.User)
                .WithMany()
                .HasForeignKey(upp => upp.UserId);

            modelBuilder.Entity<UserPagePermission>()
                .HasOne(upp => upp.Page)
                .WithMany()
                .HasForeignKey(upp => upp.PageId);

            modelBuilder.Entity<UserPagePermission>()
                .HasIndex(upp => new { upp.UserId, upp.PageId })
                .IsUnique();
        }
    }
}