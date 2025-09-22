using Confluent.Kafka;

namespace OrderService.WebApi.Services
{
    /// <summary>
    /// Сервис для отправки уведомлений в Kafka
    /// </summary>
    public class ProducerService
    {
        private readonly IConfiguration _configuration;
        private readonly IProducer<Null, string> _producer;

        public ProducerService(IConfiguration configuration)
        {
            _configuration = configuration;

            var producerconfig = new ProducerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<Null, string>(producerconfig).Build();
        }

        /// <summary>
        /// Отправляет сообщение в Kafka
        /// </summary>
        /// <param name="topic">
        /// Topic, в который отправляется сообщение
        /// </param>
        /// <param name="message">
        /// Сообщение для отправки
        /// </param>
        public void Produce(string topic, string message)
        {
            var kafkamessage = new Message<Null, string> { Value = message, };

            _producer.Produce(topic, kafkamessage);
        }
    }
}
