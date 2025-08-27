using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using ProductView.Services; // đổi WebClient.Services -> ProductView.Services

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpClientFactory - IdentityServer
builder.Services.AddHttpClient("IdentityServer", client =>
{
    var baseUrl = builder.Configuration["IdentityServer:BaseUrl"]!;
    client.BaseAddress = new Uri(baseUrl);
});

// HttpClientFactory - ServiceApi
builder.Services.AddHttpClient("ServiceApi", client =>
{
    var baseUrl = builder.Configuration["ServiceApi:BaseUrl"]!;
    client.BaseAddress = new Uri(baseUrl);
});

// DI services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<IProductService, ProductService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7092/");
});




var app = builder.Build();
builder.Services.AddSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
