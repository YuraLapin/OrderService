namespace OrderService.DataAccess.Postgres.Models
{
    // <summary>
    // Модель заказа, хранящегося в БД
    // </summary>
    public class Order
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string EmailClient { get; set; }
        public decimal Price {  get; set; }
        public string PhoneNumber { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Order order)
            {
                return Id == order.Id && ProductId == order.ProductId &&
                    EmailClient == order.EmailClient && PhoneNumber == order.PhoneNumber;
            }

            return false;
        }
    }
}
