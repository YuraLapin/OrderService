using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.WebApi.UseCases.Commands
{
    public sealed record class DeleteOrderCommand(long OrderId) : IRequest<string?>;
}
