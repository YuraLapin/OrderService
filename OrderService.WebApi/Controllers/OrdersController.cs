using Microsoft.AspNetCore.Mvc;
using Mediator;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IMediator _mediator;

        public OrdersController
        (
            ILogger<OrdersController> logger,
            IMediator mediator
        )
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> AddOrder(Order order, CancellationToken ct)
        {
            var res = await _mediator.Send(new AddOrderCommand(order), ct);

            if (res is string)
            {
                return BadRequest(res);
            }

            return Json((long)res);
        }

        [HttpGet("orders/{orderId:int}")]
        public async Task<IActionResult> GetOrder(int orderId, CancellationToken ct)
        {
            var res = await _mediator.Send(new GetOrderCommand(orderId));

            if (res is string)
            {
                return BadRequest(res);
            }

            return Json((Order)res);
        }

        [HttpDelete("orders/{orderId:int}")]
        public async Task<IActionResult> DeleteOrder(int orderId, CancellationToken ct)
        {
            var res = await _mediator.Send(new DeleteOrderCommand(orderId), ct);

            if (res is string)
            {
                return BadRequest(res);
            }

            return Ok();
        }
    }
}
