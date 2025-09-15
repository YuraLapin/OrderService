using Mediator;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.UseCases.Commands;
using FluentValidation;

namespace OrderService.WebApi.UseCases.Handlers
{
    public class DeleteOrderHandler(DataBaseContext db, IValidator<Order> validator) : IRequestHandler<DeleteOrderCommand, string?>
    {
        public async ValueTask<string?> Handle(DeleteOrderCommand command, CancellationToken ct)
        {
            if (command.OrderId < 0) return "Id заказа не может быть меньше нуля";
            db.Orders.Remove(new Order() { Id = command.OrderId });
            await db.SaveChangesAsync(ct);
            return null;
        }
    }
}
