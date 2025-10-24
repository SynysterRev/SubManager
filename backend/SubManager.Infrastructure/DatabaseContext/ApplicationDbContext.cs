using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SubManager.Domain.Entities;
using SubManager.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(a => a.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Subscription>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(a => a.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
