using Microsoft.AspNetCore.Mvc;
using Mediator;
using OrderService.DataAccess.Postgres.Models;
using OrderService.WebApi.UseCases.Commands;

namespace OrderService.WebApi.Controllers
{
    // <summary>
    // Контроллер для адреса /orders
    // </summary>
    [Route("orders")]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // <summary>
        // Добавляет заказ в БД, отправляет
        // данные в Payment Service для
        // резервирования оплаты
        // </summary>
        // <returns>
        // Id созданного заказа
        // </returns>
        // <param name="order">
        // Добавляемый заказ
        // </param>
        // <param name="ct">
        // Токен отмены
        // </param>
        [HttpPost("create")]
        public async Task<IActionResult> AddOrder([FromBody] Order order, CancellationToken ct)
        {
            Object res = await _mediator.Send(new AddOrderCommand(order), ct);

            if (res is string)
            {
                return BadRequest(res);
            }

            return Json((long)res);
        }

        // <summary>
        // Получает заказ из БД по его Id
        // </summary>
        // <returns>
        // Требуемый заказ
        // </returns>
        // <param name="orderId">
        // Id получаемого заказа
        // </param>
        // <param name="ct">
        // Токен отмены
        // </param>
        [HttpGet("{orderId:long}")]
        public async Task<IActionResult> GetOrder(long orderId, CancellationToken ct)
        {
            Object res = await _mediator.Send(new GetOrderCommand(orderId));

            if (res is string)
            {
                return BadRequest(res);
            }

            return Json((Order)res);
        }

        // <summary>
        // Удаляет заказ из БД по его Id
        // </summary>
        // <param name="orderId">
        // Id удаляемого заказа
        // </param>
        // <param name="ct">
        // Токен отмены
        // </param>
        [HttpDelete("{orderId:long}")]
        public async Task<IActionResult> DeleteOrder(long orderId, CancellationToken ct)
        {
            string? res = await _mediator.Send(new DeleteOrderCommand(orderId), ct);

            if (res is string)
            {
                return BadRequest(res);
            }

            return Ok();
        }
    }
}
