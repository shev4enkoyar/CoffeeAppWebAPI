using CoffeeAppWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeAppWebApi.DataEf
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<CoffeeList> CoffeeList { get; set; }
        public DbSet<AmountUsedCoffee> AmountUsedCoffees { get; set; }
    }
}
