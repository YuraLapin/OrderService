using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using OrderService.WebApi;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Newtonsoft.Json;
using OrderService.DataAccess.Postgres.Models;

namespace OrderService.Test
{
    // <summary>
    // Тесты для сервиса заказов
    // </summary>
    [TestFixture]
    public sealed class OrderServiceTests : IDisposable
    {
        private readonly PostgreSqlContainer _orderDbContainer = new PostgreSqlBuilder().Build();
        private IContainer _paymentAppContainer;
        private IContainer _paymentDbContainer;
        private WebApplicationFactory<Program> _webApplicationFactory;
        private HttpClient _httpClient;

        // <summary>
        // Разворачивание контейнеров с зависимыми сервисами
        // </summary>
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var network = new NetworkBuilder().Build();

            var paymentAppImage = new ImageFromDockerfileBuilder()
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

            var clientOptions = new WebApplicationFactoryClientOptions();
            clientOptions.AllowAutoRedirect = false;

            _webApplicationFactory = new CustomWebApplicationFactory(_orderDbContainer.GetConnectionString(), paymentAppPort);
            _httpClient = _webApplicationFactory.CreateClient(clientOptions);
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
        public async Task OrderCreateTest(long productId, string emailClient, decimal price, string phoneNumber, string expected)
        {
            HttpResponseMessage res = await _httpClient.PostAsync($"/orders/create?productId={productId}&emailClient={emailClient}&price={price}&phoneNumber={phoneNumber}", null);
            string actual = res.StatusCode.ToString();

            Assert.That(actual, Is.EqualTo(expected));
        }

        // <summary>
        // Тесты создания, а затем получения созданного заказа
        // </summary>
        [Test]
        [TestCase(1, "123@gmail.com", 2.0, "89504468003")]
        [TestCase(999, "123123123123@gmail.com", 1312321321.2321, "89504468003")]
        public async Task OrderGetTest(long productId, string emailClient, decimal price, string phoneNumber)
        {
            HttpResponseMessage res = await _httpClient.PostAsync($"/orders/create?productId={productId}&emailClient={emailClient}&price={price}&phoneNumber={phoneNumber}", null);
            string addedId = await res.Content.ReadAsStringAsync();

            Order expected = new Order()
            {
                Id = long.Parse(addedId),
                ProductId = productId,
                EmailClient = emailClient,
                Price = price,
                PhoneNumber = phoneNumber,
            };

            res = await _httpClient.GetAsync($"/orders/{addedId}");
            string resString = await res.Content.ReadAsStringAsync();
            Order actual = JsonConvert.DeserializeObject<Order>(resString);

            Assert.That(actual, Is.EqualTo(expected));
        }

        // <summary>
        // Тесты получения несуществующего заказа
        // </summary>
        [Test]
        [TestCase(-1)]
        [TestCase(92929)]
        public async Task OrderGetWrongTest(long orderId)
        {
            string expected = "BadRequest";

            HttpResponseMessage res = await _httpClient.GetAsync($"/orders/{orderId}");
            string actual = res.StatusCode.ToString();

            Assert.That(actual, Is.EqualTo(expected));
        }

        // <summary>
        // Тесты удаления
        // </summary>
        [Test]
        public async Task OrderDeleteTest()
        {
            // Создание заказа
            HttpResponseMessage res = await _httpClient.PostAsync($"/orders/create?productId=1&emailClient=123@gmail.com&price=1.0&phoneNumber=89504468003", null);
            string addedId = await res.Content.ReadAsStringAsync();

            // Удаление созданного заказа
            res = await _httpClient.DeleteAsync($"/orders/{addedId}");
            string expected = "OK";
            string actual = res.StatusCode.ToString();
            Assert.That(actual, Is.EqualTo(expected));

            // Удалене уже удаленного заказа
            res = await _httpClient.DeleteAsync($"/orders/{addedId}");
            expected = "BadRequest";
            actual = res.StatusCode.ToString();
            Assert.That(actual, Is.EqualTo(expected));

            // Удаление заказа с отрицательным Id
            res = await _httpClient.DeleteAsync($"/orders/{-2}");
            expected = "BadRequest";
            actual = res.StatusCode.ToString();
            Assert.That(actual, Is.EqualTo(expected));
        }

        // <summary>
        // Сворачивание контейнеров
        // </summary>
        [OneTimeTearDown]
        public void Dispose()
        {
            _webApplicationFactory.Dispose();

            _orderDbContainer.DisposeAsync();
            _paymentAppContainer.DisposeAsync();
            _paymentDbContainer.DisposeAsync();
        }
    }
}
