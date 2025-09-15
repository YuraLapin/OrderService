using Mediator;
using OrderService.DataAccess.Postgres;
using OrderService.WebApi.Refit;
using OrderService.WebApi.UseCases.Commands;
using OrderService.WebApi.Utility;
using OrderService.WebApi.Models;

namespace OrderService.WebApi.UseCases.Handlers
{
    public class AddOrderHandler(DataBaseContext db, InputChecker inputChecker, IPaymentClient paymentClient): IRequestHandler<AddOrderCommand, Object>
    {
        public async ValueTask<Object> Handle(AddOrderCommand command, CancellationToken ct)
        {
            string? errorMessage = inputChecker.CheckOrder(command.Order);
            if (errorMessage != null) return errorMessage;

            var newOrder = command.Order;
            db.Orders.Add(newOrder);
            await db.SaveChangesAsync(ct);

            await paymentClient.AddPayment(new Payment() { OrderId = newOrder.Id, Price = newOrder.Price }, ct);

            return newOrder.Id;
        }
    }
}
