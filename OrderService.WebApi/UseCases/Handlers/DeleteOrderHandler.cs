using Mediator;
using OrderService.DataAccess.Postgres;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.UseCases.Commands;
using Microsoft.AspNetCore.Mvc;
using OrderService.WebApi.Utility;

namespace OrderService.WebApi.UseCases.Handlers
{
    public class DeleteOrderHandler(DataBaseContext db, InputChecker inputChecker) : IRequestHandler<DeleteOrderCommand, string?>
    {
        public async ValueTask<string?> Handle(DeleteOrderCommand command, CancellationToken ct)
        {
            string? errorMessage = inputChecker.CheckId(command.OrderId);
            if (errorMessage != null) return errorMessage;
            db.Orders.Remove(new Order() { Id = command.OrderId });
            await db.SaveChangesAsync(ct);
            return null;
        }
    }
}
