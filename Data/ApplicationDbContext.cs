using Microsoft.EntityFrameworkCore;
using MVCandKAFKA3.Models;

namespace MVCandKAFKA3.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

   
}
