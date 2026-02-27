using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Movie>(entity =>
            {
                entity.Property(e => e.Cost)
                    .HasPrecision(18, 2);
            });

            // Configure payments: unique reference, row version for optimistic concurrency
            builder.Entity<Payment>(entity =>
            {
                entity.HasIndex(p => p.Reference).IsUnique();
                entity.Property(p => p.RowVersion).IsRowVersion();

                // ensure amount precision if needed
                entity.Property(p => p.Amount).HasPrecision(18, 2);
            });
        }
    }
}
