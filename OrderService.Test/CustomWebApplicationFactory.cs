using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres;
using Microsoft.Extensions.DependencyInjection;
using OrderService.WebApi;

namespace OrderService.Test
{
    internal sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;
        private readonly string _paymentAppPort;

        public CustomWebApplicationFactory(string connectionString, int paymentAppPort)
        {
            _connectionString = connectionString;
            _paymentAppPort = connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<DataBaseContext>(options => options.UseNpgsql($"http://localhost:{_paymentAppPort}"));
            });

            builder.UseEnvironment("Test");
        }
    }
}
