using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.WebApi.UseCases.Commands
{
    // <summary>
    // Mediator команда для удаления заказа из БД
    // </summary>
    // <returns>
    // null при успехе
    // Строка с сообщением об ошибке при ошибке
    // </returns>
    // <param name="OrderId">
    // Id удаляемого заказа
    // </param>
    public sealed record class DeleteOrderCommand(long OrderId) : IRequest<string?>;
}
