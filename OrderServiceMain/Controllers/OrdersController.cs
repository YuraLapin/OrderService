using Microsoft.AspNetCore.Mvc;
using OrderServiceDataBase;

namespace OrderService.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly DataBaseContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string? paymentAddress;

        public OrdersController(ILogger<OrdersController> logger, DataBaseContext db, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _db = db;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            paymentAddress = _configuration["Addresses:PaymentService"] ?? "";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("orders")]
        public async Task<IActionResult> AddOrder(int sum, string clientName)
        {
            _logger.LogWarning($"Order sum{sum} name{clientName}");

            _db.Orders.Add(new Order { Sum = sum, ClientName = clientName });
            await _db.SaveChangesAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, paymentAddress);
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            return Ok();
        }

        [HttpGet("orders/{id:int}")]
        public IActionResult GetOrder(int id)
        {
            Order? res = _db.Orders.Find(id);
            if (res != null)
            {
                _logger.LogWarning($"Order { id }");
                return Json(res);
            }
            _logger.LogWarning($"No order { id }");
            return BadRequest();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return Error();
        //}
    }
}
