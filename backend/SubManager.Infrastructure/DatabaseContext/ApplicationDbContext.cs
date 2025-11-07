using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SubManager.Domain.Entities;
using SubManager.Domain.IdentityEntities;

namespace SubManager.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(a => a.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Subscription>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Subscription>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Streaming" },
                new Category { Id = 2, Name = "Music" },
                new Category { Id = 3, Name = "Cloud Storage" },
                new Category { Id = 4, Name = "Productivity" },
                new Category { Id = 5, Name = "Fitness" },
                new Category { Id = 6, Name = "Gaming" },
                new Category { Id = 7, Name = "Development" },
                new Category { Id = 8, Name = "Education" },
                new Category { Id = 9, Name = "Communication" },
                new Category { Id = 10, Name = "News Media" },
                new Category { Id = 11, Name = "Other" }
            );
        }
    }
}
