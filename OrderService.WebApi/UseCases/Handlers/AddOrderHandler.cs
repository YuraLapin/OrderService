using Mediator;
using FluentValidation;
using OrderService.DataAccess.Postgres.Models;
using OrderService.DataAccess.Postgres;
using OrderService.WebApi.Refit;
using OrderService.WebApi.UseCases.Commands;
using OrderService.WebApi.Models;

namespace OrderService.WebApi.UseCases.Handlers
{
    public class AddOrderHandler
    (
        DataBaseContext db,
        IPaymentClient paymentClient,
        IValidator<Order> validator
    ) : IRequestHandler<AddOrderCommand, Object>
    {
        public async ValueTask<Object> Handle(AddOrderCommand command, CancellationToken ct)
        {
            var validationResult = await validator.ValidateAsync(command.Order, ct);
            if (!validationResult.IsValid) return validationResult.ToString();

            var newOrder = command.Order;
            db.Orders.Add(newOrder);
            await db.SaveChangesAsync(ct);

            await paymentClient.AddPayment(new Payment() { OrderId = newOrder.Id, Price = newOrder.Price }, ct);

            return newOrder.Id;
        }
    }
}
