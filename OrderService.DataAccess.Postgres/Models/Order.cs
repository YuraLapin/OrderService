using System.ComponentModel.DataAnnotations;

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
    }
}
