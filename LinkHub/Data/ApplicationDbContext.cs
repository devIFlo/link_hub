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
        public DbSet<LogEntry> Logs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Link>()
               .HasOne(c => c.Category)
               .WithMany()
               .HasForeignKey(c => c.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Page)
                .WithMany()
                .HasForeignKey(c => c.PageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPagePermission>()
                .HasOne(upp => upp.User)
                .WithMany()
                .HasForeignKey(upp => upp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPagePermission>()
                .HasOne(upp => upp.Page)
                .WithMany()
                .HasForeignKey(upp => upp.PageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPagePermission>()
                .HasIndex(upp => new { upp.UserId, upp.PageId })
                .IsUnique();

			modelBuilder.Entity<LogEntry>(entity =>
			{
				entity.Property(e => e.Message).HasColumnType("TEXT").IsRequired(false);
				entity.Property(e => e.Level).HasColumnType("INTEGER").IsRequired(false);  
				entity.Property(e => e.Timestamp).HasColumnType("TIMESTAMP").IsRequired(false);
				entity.Property(e => e.Exception).HasColumnType("TEXT").IsRequired(false);
				entity.Property(e => e.Properties).HasColumnType("JSONB").IsRequired(false);
				entity.Property(e => e.UserName).HasColumnType("TEXT").IsRequired(false);
			});
		}
	}
}