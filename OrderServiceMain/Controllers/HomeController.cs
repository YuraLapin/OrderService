using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using OrderService.DataBase;

namespace OrderService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataBaseContext _db;

        public HomeController(ILogger<HomeController> logger, DataBaseContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("orders")]
        public async Task<IActionResult> Orders(int sum, string clientName)
        {
            _logger.LogWarning($"sum{sum} name{clientName}");

            _db.Orders.Add(new Order { Sum = sum, ClientName = clientName });
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("orders/{id:int}")]
        public IActionResult Orders(int id)
        {
            

            return Ok();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return Error();
        //}
    }
}
