using Microsoft.EntityFrameworkCore;

namespace OrderService.DataBase
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
