using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using OrderService.WebApi;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Refit;
using OrderService.DataAccess.Postgres.Models;
using OrderService.Test.Refit;
using DotNet.Testcontainers.Networks;
using DotNet.Testcontainers.Images;

namespace OrderService.Test
{
    // <summary>
    // Тесты для сервиса заказов
    // </summary>
    [TestFixture]
    public sealed class OrderServiceTests
    {
        private readonly PostgreSqlContainer _orderDbContainer = new PostgreSqlBuilder().Build();
        private IContainer _paymentAppContainer;
        private IContainer _paymentDbContainer;
        private WebApplicationFactory<Program> _webApplicationFactory;
        private IOrderApi _orderApi;

        // <summary>
        // Разворачивание контейнеров с необходимыми сервисами
        // </summary>
        [OneTimeSetUp]
        public async Task Setup()
        {
            INetwork network = new NetworkBuilder().Build();

            IFutureDockerImage paymentAppImage = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "..")
                .WithDockerfile("PaymentService/dockerfile")
                .Build();
            _paymentAppContainer = new ContainerBuilder()
                .WithImage(paymentAppImage)
                .WithNetwork(network)
                .WithPortBinding(8080, true)
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

            await paymentAppImage.CreateAsync().ConfigureAwait(false);

            await _orderDbContainer.StartAsync();
            await _paymentAppContainer.StartAsync();
            await _paymentDbContainer.StartAsync();

            // Порт, по которому можно обратиться к сервису оплаты
            int paymentAppPort = _paymentAppContainer.GetMappedPublicPort();

            _webApplicationFactory = new CustomWebApplicationFactory(_orderDbContainer.GetConnectionString(), paymentAppPort);
            HttpClient httpClient = _webApplicationFactory.CreateClient();
            _orderApi = RestService.For<IOrderApi>(httpClient);
        }

        // <summary>
        // Тесты для создания заказа
        // </summary>
        [Test]
        [TestCase(1, "123@gmail.com", 2.0, "89504468003", "OK")]
        [TestCase(-1, "123@gmail.com", 2.0, "89504468003", "BadRequest")]
        [TestCase(1, ";123", 2.0, "89504468003", "BadRequest")]
        [TestCase(1, "123", 2.0, "89504468003", "BadRequest")]
        [TestCase(1, "123@gmail.com", -2.0, "89504468003", "BadRequest")]
        [TestCase(1, "123@gmail.com", 2.0, "123", "BadRequest")]
        public async Task CreateOrderTest(long productId, string emailClient, decimal price, string phoneNumber, string expected)
        {
            Order newOrder = new Order()
            {
                ProductId = productId,
                EmailClient = emailClient,
                Price = price,
                PhoneNumber = phoneNumber,
            };

            ApiResponse<long> res = await _orderApi.AddOrder(newOrder);
            string actual = res.StatusCode.ToString();

            Assert.That(actual, Is.EqualTo(expected));
        }

        // <summary>
        // Тесты создания, а затем получения созданного заказа
        // </summary>
        [Test]
        [TestCase(1, "123@gmail.com", 2.0, "89504468003")]
        [TestCase(999, "123123123123@gmail.com", 1312321321.2321, "89504468003")]
        public async Task GetOrderTest(long productId, string emailClient, decimal price, string phoneNumber)
        {
            Order expected = new Order()
            {
                ProductId = productId,
                EmailClient = emailClient,
                Price = price,
                PhoneNumber = phoneNumber,
            };

            ApiResponse<long> addOrderRes = await _orderApi.AddOrder(expected);
            long addedId = addOrderRes.Content;
            expected.Id = addedId;

            ApiResponse<Order> getOrderRes = await _orderApi.GetOrder(addedId);
            Order actual= getOrderRes.Content;

            Assert.That(actual, Is.EqualTo(expected));
        }

        // <summary>
        // Тесты получения несуществующего заказа
        // </summary>
        [Test]
        [TestCase(-1)]
        [TestCase(92929)]
        public async Task GetWrongOrderTest(long orderId)
        {
            string expected = "BadRequest";

            ApiResponse<Order> res = await _orderApi.GetOrder(orderId);
            string actual = res.StatusCode.ToString();

            Assert.That(actual, Is.EqualTo(expected));
        }

        // <summary>
        // Тесты удаления
        // </summary>
        [Test]
        public async Task DeleteOrderTest()
        {
            Order newOrder = new Order()
            {
                ProductId = 1,
                EmailClient = "123@gmail.com",
                Price = 1.0M,
                PhoneNumber = "89504468003"
            };

            // Создание заказа
            ApiResponse<long> addOrderRes = await _orderApi.AddOrder(newOrder);
            long addedId = addOrderRes.Content;

            // Удаление созданного заказа
            ApiResponse<string> deleteOrderRes = await _orderApi.DeleteOrder(addedId);
            string expected = "OK";
            string actual = deleteOrderRes.StatusCode.ToString();
            Assert.That(actual, Is.EqualTo(expected));

            // Удаление уже удаленного заказа
            deleteOrderRes = await _orderApi.DeleteOrder(addedId);
            expected = "BadRequest";
            actual = deleteOrderRes.StatusCode.ToString();
            Assert.That(actual, Is.EqualTo(expected));

            // Удаление заказа с отрицательным Id
            deleteOrderRes = await _orderApi.DeleteOrder(-2);
            expected = "BadRequest";
            actual = deleteOrderRes.StatusCode.ToString();
            Assert.That(actual, Is.EqualTo(expected));
        }

        // <summary>
        // Сворачивание контейнеров
        // </summary>
        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _webApplicationFactory.DisposeAsync();

            await _orderDbContainer.DisposeAsync();
            await _paymentAppContainer.DisposeAsync();
            await _paymentDbContainer.DisposeAsync();
        }
    }
}
