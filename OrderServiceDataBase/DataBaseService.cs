using OrderServiceDataBase.Models;

namespace OrderServiceDataBase
{
    public class DataBaseService
    {
        private readonly DataBaseContext _db;

        public DataBaseService(DataBaseContext db)
        {
            _db = db;
        }

        public async Task<int> AddOrder(int sum, string clientName, CancellationToken ct)
        {
            var newOrder = new Order { Sum = sum, ClientName = clientName };
            _db.Orders.Add(newOrder);
            await _db.SaveChangesAsync(ct);

            if (ct.IsCancellationRequested)
            {
                _db.Orders.Remove(newOrder);
            }

            return newOrder.Id;
        }

        public Order? GetOrder(int id)
        {
            Order? res = _db.Orders.Find(id);
            return res;
        }

        public async Task DeleteOrder(int id)
        {
            _db.Orders.Remove(new Order() { Id = id });
            await _db.SaveChangesAsync();
        }
    }
}
