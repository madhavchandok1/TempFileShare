using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TempFileShare.Contracts.Models;

namespace TempFileShare.DataAccessLayer
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Files> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure User - Session Relationship (One-to-One)
            _ = modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Session)
                .WithOne(u => u.User)
                .HasForeignKey<Session>(k => k.UserId);

            //Configure Session - Files Relationship (One-to-Many)
            _ = modelBuilder.Entity<Session>()
                .HasMany(s => s.Files)
                .WithOne(f => f.Session)
                .HasForeignKey(f => f.SessionId);

            List<IdentityRole> roles =
            [
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                }
            ];

            _ = modelBuilder.Entity<IdentityRole>()
                .HasData(roles);
        }
    }
}
