using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OrderServiceDataBase.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int Sum { get; set; }
        [Required]
        public string ClientName { get; set; }
    }
}
