using Mediator;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.UseCases.Commands;
using FluentValidation;

namespace OrderService.WebApi.UseCases.Handlers
{
    // <summary>
    // Обработчик для команды удаления заказа
    // </summary>
    public class DeleteOrderHandler(DataBaseContext db, IValidator<Order> validator) : IRequestHandler<DeleteOrderCommand, string?>
    {
        // <summary>
        // Удаляет заказ с заданным Id из БД
        // </summary>
        // <returns>
        // null при успехе
        // Строка с сообщением об ошибке при ошибке
        // </returns>
        // <param name="command">
        // Mediator команда с полем
        // OrderId - Id удаляемого заказа
        // </param>
        // <param name="ct">
        // Токен отмены
        // </param>
        public async ValueTask<string?> Handle(DeleteOrderCommand command, CancellationToken ct)
        {
            Order? toDelete = await db.Orders.FindAsync(command.OrderId, ct);
            if (toDelete == null) return "Не найдено заказа с заданным Id";

            db.Orders.Remove(toDelete);
            await db.SaveChangesAsync(ct);

            return null;
        }
    }
}
