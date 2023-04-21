using eastwest.Models;
using Microsoft.EntityFrameworkCore;

namespace eastwest.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public virtual DbSet<UserModel> Users { get; set; }
        public virtual DbSet<ProductModel> Products { get; set; }
        public virtual DbSet<ProductImageModel> ProductImages { get; set; }
        public virtual DbSet<LocationModel> Locations { get; set; }
        public virtual DbSet<ProductLocationModel> ProductLocations { get; set; }
    }
}
