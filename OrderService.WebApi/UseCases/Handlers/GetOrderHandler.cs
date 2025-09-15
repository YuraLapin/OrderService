using Mediator;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.UseCases.Commands;
using OrderService.WebApi.Utility;

namespace OrderService.WebApi.UseCases.Handlers
{
    public class GetOrderHandler(DataBaseContext db, InputChecker inputChecker) : IRequestHandler<GetOrderCommand, Object>
    {
        public async ValueTask<Object> Handle(GetOrderCommand command, CancellationToken ct)
        {
            string? errorMessage = inputChecker.CheckId(command.OrderId);
            if (errorMessage != null) return errorMessage;

            Order? res = db.Orders.Find(command.OrderId);
            if (res == null)
            {
                return "Заказа с заданым id не существует";
            }
            return res;
        }
    }
}
