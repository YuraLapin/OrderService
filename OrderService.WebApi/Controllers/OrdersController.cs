using Microsoft.AspNetCore.Mvc;
using OrderServiceDataBase;
using OrderServiceDataBase.Models;
using OrderServiceMain.Refit;
using OrderServiceMain.Utility;

namespace OrderService.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IConfiguration _configuration;
        private readonly DataBaseService _dbService;
        private readonly IPaymentClient _paymentClient;
        private readonly InputChecker _inputChecker;

        public OrdersController
        (
            ILogger<OrdersController> logger,
            DataBaseService dbService,
            IConfiguration configuration,
            IPaymentClient paymentClient,
            InputChecker inputChecker
        )
        {
            _logger = logger;
            _configuration = configuration;
            _dbService = dbService;
            _paymentClient = paymentClient;
            _inputChecker = inputChecker;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("orders")]
        public async Task<IActionResult> AddOrder(int sum, string clientName, CancellationToken ct)
        {
            //_logger.LogWarning($"Order sum{sum} name{clientName}");

            string? errorMessage = _inputChecker.CheckOrder(sum, clientName);
            if (errorMessage != null) return BadRequest(errorMessage);

            var newId = await _dbService.AddOrder(sum, clientName, ct);

            if (ct.IsCancellationRequested) return StatusCode(499);

            await _paymentClient.AddPayment(newId, ct);

            if (ct.IsCancellationRequested)
            {
                await _dbService.DeleteOrder(newId);
                return StatusCode(499);
            }

            return Ok();
        }

        [HttpGet("orders/{id:int}")]
        public async Task<IActionResult> GetOrder(int id, CancellationToken ct)
        {
            string? errorMessage = _inputChecker.CheckOrderId(id);
            if (errorMessage != null) return BadRequest(errorMessage);

            Order? res = _dbService.GetOrder(id);
            if (res == null)
            {
                //_logger.LogWarning($"No order { id }");
                return BadRequest("Заказа с заданым id не существует");
            }

            bool isComplete = await _paymentClient.GetPayment(id, ct);

            if (ct.IsCancellationRequested) return StatusCode(499);

            //_logger.LogWarning($"Order { id }");
            return Json(new CompletableOrder(res, isComplete));
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return Error();
        //}
    }
}
