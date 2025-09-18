using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using OrderService.WebApi;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace OrderService.Test
{
    [TestFixture]
    public sealed class OrderServiceTests : IDisposable
    {
        private readonly PostgreSqlContainer _orderDbContainer = new PostgreSqlBuilder().Build();
        private IContainer _paymentAppContainer;
        private IContainer _paymentDbContainer;
        private IContainer _kafkaContainer;
        private IContainer _notificationAppContainer;
        private WebApplicationFactory<Program> _webApplicationFactory;
        private HttpClient _httpClient;

        [SetUp]
        public async Task SetUp()
        {
            var network = new NetworkBuilder().Build();

            var paymentAppImage = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "..")
                .WithDockerfile("PaymentService/dockerfile")
                .Build();
            _paymentAppContainer = new ContainerBuilder()
                .WithImage(paymentAppImage)
                .WithNetwork(network)
                .WithPortBinding(8080)
                .WithName("payment_app")
                .Build();

            _paymentDbContainer = new PostgreSqlBuilder()
                .WithNetwork(network)
                .WithDatabase("payment_db")
                .WithExposedPort(5434)
                .WithUsername("postgres")
                .WithPassword("123qwe")
                .WithName("payment_db")
                .Build();

            _kafkaContainer = new ContainerBuilder()
                .WithImage("apache/kafka:latest")
                .WithNetwork(network)
                .WithExposedPort(9092)
                .WithEnvironment("KAFKA_LISTENERS", "CONTROLLER://localhost:9091,HOST://0.0.0.0:9092,DOCKER://0.0.0.0:9093")
                .WithEnvironment("KAFKA_ADVERTISED_LISTENERS", "HOST://localhost:9092,DOCKER://kafka:9093")
                .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "CONTROLLER:PLAINTEXT,DOCKER:PLAINTEXT,HOST:PLAINTEXT")
                .WithEnvironment("KAFKA_NODE_ID", "1")
                .WithEnvironment("KAFKA_PROCESS_ROLES", "broker,controller")
                .WithEnvironment("KAFKA_CONTROLLER_LISTENER_NAMES", "CONTROLLER")
                .WithEnvironment("KAFKA_CONTROLLER_QUORUM_VOTERS", "1@localhost:9091")
                .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "DOCKER")
                .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
                .WithName("kafka")
                .Build();

            var notificationAppImage = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "..")
                .WithDockerfile("NotificationService/dockerfile")
                .Build();
            _notificationAppContainer = new ContainerBuilder()
                .WithImage(notificationAppImage)
                .WithNetwork(network)
                .WithExposedPort(8082)
                .WithName("notification_app")
                .Build();

            await paymentAppImage.CreateAsync().ConfigureAwait(false);
            await notificationAppImage.CreateAsync().ConfigureAwait(false);

            await _orderDbContainer.StartAsync();
            await _paymentAppContainer.StartAsync();
            await _paymentDbContainer.StartAsync();
            await _kafkaContainer.StartAsync();
            await _notificationAppContainer.StartAsync();

            int paymentAppPort = _paymentAppContainer.GetMappedPublicPort();

            var clientOptions = new WebApplicationFactoryClientOptions();
            clientOptions.AllowAutoRedirect = false;

            _webApplicationFactory = new CustomWebApplicationFactory(_orderDbContainer.GetConnectionString(), paymentAppPort);
            _httpClient = _webApplicationFactory.CreateClient(clientOptions);
        }

        [Test]
        [TestCase("1", "123@gmail.com", "2.0", "89504468003", "1")]
        [TestCase("-1", "123@gmail.com", "2.0", "89504468003", "ProductId не может быть отрицательным")]
        [TestCase("1", ";123", "2.0", "89504468003", "В почте клиента обнаружен недопустимый символ")]
        [TestCase("1", "123", "2.0", "89504468003", "Почта клиента не действительна")]
        [TestCase("1", "123@gmail.com", "-2.0", "89504468003", "Сумма заказа не может быть меньше нуля")]
        [TestCase("1", "123@gmail.com", "2.0", "123", "Номер телефона клиента не действителен")]
        public async Task OrderCreateTest(string productId, string emailClient, string price, string phoneNumber, string expected)
        {
            HttpResponseMessage res = await _httpClient.PostAsync($"/orders/create?productId={productId}&emailClient={emailClient}&price={price}&phoneNumber={phoneNumber}", null);
            string actual = await res.Content.ReadAsStringAsync();

            Assert.That(actual, Is.EqualTo(expected));
        }

        public void Dispose()
        {
            _webApplicationFactory.Dispose();

            _orderDbContainer.DisposeAsync();
            _paymentAppContainer.DisposeAsync();
            _paymentDbContainer.DisposeAsync();
            _kafkaContainer.DisposeAsync();
            _notificationAppContainer.DisposeAsync();
        }
    }
}
