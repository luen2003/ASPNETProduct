using IdentityServer.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký thêm dịch vụ
builder.Services.AddSingleton<Audience>();
builder.Services.AddSingleton<RSACrypt>();
builder.Services.AddSingleton<ISettings, OTPSettings>();
builder.Services.AddTransient<TokenExtension>();
builder.Services.AddSingleton<IOneWayConverter<long, byte[]>, HOTPValueLongToByteArrayConverter>();
builder.Services.AddTransient<ISecretGeneratorProvider, HMAC256UserIdSecretGeneratorProvider>();

var assembly = typeof(Program).Assembly;
builder.Services.AddCustomScopedServices(assembly, null, "IdentityServer.Services");

var app = builder.BuildApplication(builder.Configuration, assembly);
app.UseApplicationConfigure();
app.Run();
