using Mediator;

namespace OrderService.WebApi.UseCases.Commands
{
    /// <summary>
    /// Mediator команда для получения заказа из БД
    /// </summary>
    /// <returns>
    /// Полученный из БД Order в виде Object при успехе
    /// Строку сообщения об ошибке в виде Object при ошибке
    /// </returns>
    /// <param name="OrderId">
    /// Id требуемого заказа
    /// </param>
    public sealed record class GetOrderCommand(long OrderId) : IRequest<Object>;
}
