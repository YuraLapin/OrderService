using Mediator;
using OrderService.DataAccess.Postgres.Models;

namespace OrderService.WebApi.UseCases.Commands
{
    /// <summary>
    /// Mediator команда для записи заказа в БД
    /// </summary>
    /// <returns>
    /// Id добавленного заказа в виде Object при успехе
    /// Сообщение об ошибке в виде Object при ошибке
    /// </returns>
    /// <param name="Order">
    /// Объект добавляемого заказа
    /// </param>
    public sealed record class AddOrderCommand(Order Order): IRequest<Object>;
}
