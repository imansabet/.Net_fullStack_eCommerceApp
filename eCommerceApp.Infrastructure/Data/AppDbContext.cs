using eCommerceApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

}
