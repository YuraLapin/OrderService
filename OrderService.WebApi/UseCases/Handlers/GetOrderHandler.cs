using Mediator;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.UseCases.Handlers
{
    // <summary>
    // Обработчик для команды получения заказа
    // </summary>
    public class GetOrderHandler(DataBaseContext db) : IRequestHandler<GetOrderCommand, Object>
    {
        // <summary>
        // Получает заказ с заданным Id из БД
        // </summary>
        // <returns>
        // Полученный из БД Order в виде Object при успехе
        // Строку сообщения об ошибке в виде Object при ошибке
        // </returns>
        // <param name="command">
        // Mediator команда с полем
        // OrderId - Id требуемого заказа
        // </param>
        // <param name="ct">
        // Токен отмены
        // </param>
        public async ValueTask<Object> Handle(GetOrderCommand command, CancellationToken ct)
        {
            Order? res = await db.Orders.FindAsync(command.OrderId, ct);

            if (res == null) return "Заказа с заданым id не существует";

            return res;
        }
    }
}
