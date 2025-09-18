using FluentValidation;
using Mediator;
using Refit;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres;
using OrderService.WebApi.Refit;
using OrderService.WebApi.Validators;
using OrderService.WebApi.Services;

namespace OrderService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
    
            string connString = builder.Configuration["ConnectionStrings:Postgres"];
            string paymentAddress = builder.Configuration["Addresses:PaymentService"];
    
            builder.Services.AddSingleton<ProducerService>();
            builder.Services.AddDbContext<DataBaseContext>(options => options.UseNpgsql(connString));
            builder.Services.AddRefitClient<IPaymentClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri(paymentAddress));
            builder.Services.AddControllersWithViews();
            builder.Services.AddValidatorsFromAssemblyContaining<OrderValidator>();
            builder.Services.AddMediator((MediatorOptions options) =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
            });
    
            var app = builder.Build();
    
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }
    
            app.UseHttpsRedirection();
            app.UseStaticFiles();
    
            app.UseRouting();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Orders}/{action}/{id?}");
    
            app.Run();
        }
    }
}
