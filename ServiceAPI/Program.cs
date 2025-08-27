using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiceAPI.Authentication;
using ServiceAPI.Context;
using ServiceAPI.Repositories;
using ServiceAPI.Repositories.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ================== DEPENDENCY INJECTION ==================
builder.Services.AddSingleton<DapperContext>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new DapperContext(configuration);
});

builder.Services.AddScoped<ApplicationUser>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ServiceAPI.Services.TransactionService>();
builder.Services.AddScoped<ServiceAPI.Services.CustomerService>();
builder.Services.AddScoped<ServiceAPI.Services.VehicleService>();

builder.Services.AddControllers();

// ================== JWT AUTHENTICATION ==================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    // Xử lý lỗi JWT trả về JSON chuẩn
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            // Ngăn response mặc định
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var error = context.AuthenticateFailure != null ? context.AuthenticateFailure.Message : "Token không hợp lệ hoặc chưa được cung cấp.";
            await context.Response.WriteAsJsonAsync(new
            {
                code = "401",
                message = "Bạn chưa được xác thực.",
                error
            });
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                code = "403",
                message = "Bạn không có quyền truy cập.",
                error = "Forbidden"
            });
        }
    };
});

// ================== SWAGGER ==================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ServiceAPI",
        Version = "1.0",
        Description = "Service for ServiceAPI application"
    });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token **_only_**",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// ================== GLOBAL EXCEPTION HANDLER ==================
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (feature != null)
        {
            var ex = feature.Error;
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                code = "99",
                message = "Có lỗi xảy ra khi xử lý yêu cầu.",
                error = ex.Message
            });
        }
    });
});

// ================== MIDDLEWARE PIPELINE ==================
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceAPI v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
