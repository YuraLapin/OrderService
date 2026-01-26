namespace OrderService.DataAccess.Postgres.Models
{
    /// <summary>
    /// Модель заказа, хранящегося в БД
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Идентификатор заказанного продукта
        /// </summary>
        public long ProductId { get; set; }
        /// <summary>
        /// Почта клиента
        /// </summary>
        public string EmailClient { get; set; }
        /// <summary>
        /// Сумма заказа
        /// </summary>
        public decimal Price {  get; set; }
        /// <summary>
        /// Тел. номер клиента
        /// </summary>
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
