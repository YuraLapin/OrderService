using Mediator;
using OrderService.DataAccess.Postgres.Models;

namespace OrderService.WebApi.UseCases.Commands
{
    public sealed record class AddOrderCommand(Order Order): IRequest<Object>;
}
