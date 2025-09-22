namespace OrderService.WebApi.Models
{
    /// <summary>
    /// Объект оплаты для отправки в
    /// Payment Service
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public long OrderId { get; set; }
        /// <summary>
        /// Сумма заказа
        /// </summary>
        public decimal Price { get; set; }
    }
}
