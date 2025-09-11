using Microsoft.EntityFrameworkCore;
using OrderServiceDataBase;
using OrderServiceMain.Refit;
using Refit;
using OrderServiceMain.Utility;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

string connString = builder.Configuration["ConnectionStrings:Postgres"];
string paymentAddress = builder.Configuration["Addresses:PaymentService"];

// Add services to the container.
builder.Services.AddScoped<DataBaseService>();
builder.Services.AddSingleton<InputChecker>();
builder.Services.AddDbContext<DataBaseContext>(options => options.UseNpgsql(connString));
builder.Services.AddRefitClient<IPaymentClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri(paymentAddress));
builder.Services.AddControllersWithViews();

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
    pattern: "{controller=Orders}/{action=Index}/{id?}");

app.Run();
