namespace OrderService.WebApi.Models
{
    /// <summary>
    /// Объект оплаты для отправки в
    /// Payment Service
    /// </summary>
    public class Payment
    {
        public long OrderId { get; set; }
        public decimal Price { get; set; }
    }
}
