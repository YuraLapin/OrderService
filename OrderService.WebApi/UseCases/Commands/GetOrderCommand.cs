using Mediator;

namespace OrderService.WebApi.UseCases.Commands
{
    public sealed record class GetOrderCommand(long OrderId) : IRequest<Object>;
}
