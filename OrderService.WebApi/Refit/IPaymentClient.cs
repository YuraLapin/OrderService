using Refit;
using OrderService.WebApi.Models;

namespace OrderService.WebApi.Refit
{
    public interface IPaymentClient
    {
        [Post("/payments/create")]
        Task AddPayment(Payment payment, CancellationToken ct);
    }
}
