using Refit;

namespace OrderServiceMain.Refit
{
    public interface IPaymentClient
    {
        [Post("/payments")]
        Task AddPayment(int id, CancellationToken ct);

        [Get("/payments/{id}")]
        Task<bool> GetPayment(int id, CancellationToken ct);
    }
}
