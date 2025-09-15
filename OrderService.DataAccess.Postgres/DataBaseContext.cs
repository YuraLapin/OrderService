using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.Models;

namespace OrderService.DataAccess.Postgres
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
