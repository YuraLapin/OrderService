using Refit;
using OrderService.DataAccess.Postgres.Models;

namespace OrderService.Test.Refit
{
    public interface IOrderApi
    {
        [Post("/orders/create")]
        Task<ApiResponse<long>> AddOrder(Order order);

        [Get("/orders/{orderId}")]
        Task<ApiResponse<Order>> GetOrder(long orderId);

        [Delete("/orders/{orderId}")]
        Task<ApiResponse<string>> DeleteOrder(long orderId);
    }
}
