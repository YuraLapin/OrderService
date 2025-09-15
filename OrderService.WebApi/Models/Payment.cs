namespace OrderService.WebApi.Models
{
    public class Payment
    {
        public long OrderId { get; set; }
        public decimal Price { get; set; }
    }
}
