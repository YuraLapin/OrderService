using Mediator;
using FluentValidation;
using OrderService.DataAccess.Postgres.Models;
using OrderService.DataAccess.Postgres;
using OrderService.WebApi.Refit;
using OrderService.WebApi.UseCases.Commands;
using OrderService.WebApi.Models;
using OrderService.WebApi.Services;

namespace OrderService.WebApi.UseCases.Handlers
{
    /// <summary>
    /// Обработчик для команды добавления заказа
    /// </summary>
    public class AddOrderHandler
    (
        DataBaseContext db,
        IPaymentClient paymentClient,
        IValidator<Order> validator,
        ProducerService producer
    ) : IRequestHandler<AddOrderCommand, Object>
    {
        /// <summary>
        /// Сохраняет полученный заказ в БД,
        /// отправляет уведомление сервису уведомлений в Kafka,
        /// отправляет данные для резервирования оплаты в Payment Service
        /// </summary>
        /// <returns>
        /// Id добавленного заказа в виде Object при успехе
        /// Сообщение об ошибке в виде Object при ошибке
        /// </returns>
        /// <param name="command">
        /// Mediator команда с полем
        /// Order - объект добавляемого заказа
        /// </param>
        /// <param name="ct">
        /// Токен отмены
        /// </param>
        public async ValueTask<Object> Handle(AddOrderCommand command, CancellationToken ct)
        {
            var validationResult = await validator.ValidateAsync(command.Order, ct);
            if (!validationResult.IsValid) return validationResult.ToString();

            var newOrder = command.Order;
            db.Orders.Add(newOrder);
            await db.SaveChangesAsync(ct);

            producer.Produce("notification-topic", $"Заказ создан: Id = {newOrder.Id}; Price = {newOrder.Price}");

            // Отправка HTTP POST запроса Payment Service
            await paymentClient.AddPayment(new Payment() { OrderId = newOrder.Id, Price = newOrder.Price }, ct);

            return newOrder.Id;
        }
    }
}
