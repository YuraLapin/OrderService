using Microsoft.EntityFrameworkCore;

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
