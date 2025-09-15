using Microsoft.EntityFrameworkCore;
using Refit;
using OrderService.DataAccess.Postgres;
using OrderService.WebApi.Refit;
using Mediator;
using FluentValidation;
using OrderService.WebApi.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

string connString = builder.Configuration["ConnectionStrings:Postgres"];
string paymentAddress = builder.Configuration["Addresses:PaymentService"];

// Add services to the container.
builder.Services.AddDbContext<DataBaseContext>(options => options.UseNpgsql(connString));
builder.Services.AddRefitClient<IPaymentClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri(paymentAddress));
builder.Services.AddControllersWithViews();
builder.Services.AddValidatorsFromAssemblyContaining<OrderValidator>();
builder.Services.AddMediator((MediatorOptions options) =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Orders}/{action}/{id?}");

app.Run();
