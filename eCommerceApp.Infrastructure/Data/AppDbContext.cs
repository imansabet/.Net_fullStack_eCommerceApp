using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RefreshToken> RefreshToken { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>()
            .HasData
            (
                new IdentityRole 
                {
                   Id = Guid.NewGuid().ToString(),
                   Name = "Admin",
                   NormalizedName = "ADMIN"
                },
                 new IdentityRole
                 {
                     Id = Guid.NewGuid().ToString(),
                     Name = "User",
                     NormalizedName = "USER"
                 }
            );
    }

}
