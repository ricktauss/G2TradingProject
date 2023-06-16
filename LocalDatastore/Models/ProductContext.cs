using Microsoft.EntityFrameworkCore;

namespace LocalDatastore.Models
{
    public class ProductContext : DbContext
    {

            public ProductContext(DbContextOptions<ProductContext> options)
                : base(options)
            {
            }

            public DbSet<Product> TodoItems { get; set; } = null!;

    }
}
