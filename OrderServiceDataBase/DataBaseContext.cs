using Microsoft.EntityFrameworkCore;
using OrderServiceDataBase.Models;

namespace OrderServiceDataBase
{
    public class DataBaseContext: DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options): base(options)
        {
            Database.EnsureCreated();
        }
    }
}
