using Mediator;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.UseCases.Handlers
{
    public class GetOrderHandler(DataBaseContext db) : IRequestHandler<GetOrderCommand, Object>
    {
        public async ValueTask<Object> Handle(GetOrderCommand command, CancellationToken ct)
        {
            if (command.OrderId < 0) return "Id заказа не может быть меньше нуля";

            Order? res = await db.Orders.FindAsync(command.OrderId, ct);
            if (res == null)
            {
                return "Заказа с заданым id не существует";
            }
            return res;
        }
    }
}
