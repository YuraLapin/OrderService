using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres;
using Microsoft.Extensions.DependencyInjection;
using OrderService.WebApi;
using OrderService.WebApi.Refit;
using Refit;
using System.Data.Common;

namespace OrderService.Test
{
    internal sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;
        private readonly int _paymentAppPort;

        public CustomWebApplicationFactory(string connectionString, int paymentAppPort)
        {
            _connectionString = connectionString;
            _paymentAppPort = paymentAppPort;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.Remove(services.SingleOrDefault(service => typeof(DbContextOptions<DataBaseContext>) == service.ServiceType));
                services.Remove(services.SingleOrDefault(service => typeof(DbConnection) == service.ServiceType));
                services.AddDbContext<DataBaseContext>(options => options.UseNpgsql(_connectionString));

                services.Remove(services.SingleOrDefault(service => typeof(IPaymentClient) == service.ServiceType));
                services.AddRefitClient<IPaymentClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri($"http://localhost:{_paymentAppPort}"));
            });

            builder.UseEnvironment("Test");
        }
    }
}
