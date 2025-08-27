using IdentityServer.Helpers;
using IdentityServer.Middlewares;
using Microsoft.Extensions.Hosting;
using IdentityServer.Extensions;
using Serilog;
using Serilog.Events;
using Scrutor;
using System.Reflection;
using IdentityServer.Models;
using Microsoft.OpenApi.Models;
using Response = IdentityServer.Models.Response;

namespace IdentityServer;
public static class Startup
{
    public static IApplicationBuilder UseApplicationConfigure(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseMiddleware<GlobalInternalServerExceptionMiddleware>();
        app.UseMiddleware<JwtMiddleware>();
        return app;
    }

    public static WebApplication BuildApplication(this WebApplicationBuilder builder, IConfiguration configuration, Assembly assembly)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        GlobalData.ProjectName = AssemblyHelper.GetProjectName(assembly);
        GlobalData.ControllerName = $"/{GlobalData.ProjectName}/v1/[controller]";
        builder.Services.AddSingleton<DapperContext>();
        builder.Services.AddScoped<ApplicationUser>();
        builder.Services.AddAutoMapper(assembly);
        builder.Services.AddScoped<LoggingHelper>();
        builder.Services.AddCustomSwagger();
        builder.Services.AddControllers(_ => MvcOptionsExtensions.UseDateOnlyTimeOnlyStringConverters())
                        .AddCustomJsonOptions()
                        .AddCustomBadRequest();
        builder.Host.AddSerilog(configuration);
        builder.Services.AddEndpointsApiExplorer(); // Config minimal api
        // Đăng ký dịch vụ mặc định cho repos 
        builder.Services
                .Scan(scan => scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.InNamespaces(
                                            $"{GlobalData.ProjectName}.Repositories.Interfaces",
                                            $"{GlobalData.ProjectName}.Repositories"))
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ServiceType))
                .AsMatchingInterface()
                .WithScopedLifetime());

        return builder.Build();
    }

    private static void AddSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        const string outputTemplate = "[{Level:u3}] ==> [{Timestamp:dd/MM/yyyy HH:mm:ss}] {Message:lj}{NewLine}{Exception}";
        var serilogSetting = new SerilogSetting();
        configuration.GetSection("LoggerSetting").Bind(serilogSetting);

        hostBuilder.UseSerilog((_, config) =>
        {
            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Service", GlobalData.ProjectName)
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.File(
                    path: "./Logs/log-.txt",
                    rollingInterval: serilogSetting.RollingInterval,
                    outputTemplate: outputTemplate,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: serilogSetting.Size,
                    retainedFileCountLimit: serilogSetting.LimitFiles)
                .ReadFrom
                .Configuration(configuration);
        });
    }

    public static IServiceCollection AddCustomScopedServices(
            this IServiceCollection services,
            Assembly assembly,
            Dictionary<string, string>? implementationValues = null,
            params string[] serviceValues)
    {
        if (implementationValues != null)
        {
            foreach (var pair in implementationValues)
            {
                services.Scan(scan => scan.FromAssemblies(assembly)
                        .AddClasses(classes => classes.InNamespaces(pair.Value, pair.Key))
                        .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ImplementationType))
                        .AsMatchingInterface()
                        .WithScopedLifetime());
            }
        }

        services.Scan(scan => scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.InNamespaces(serviceValues))
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ServiceType)));

        return services;
    }

    private static void AddCustomBadRequest(this IMvcBuilder controllers)
    {
        controllers.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var modelState = actionContext.ModelState;
                var errors = modelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();

                Response response = new()
                {
                    Code = StatusCodes.Status400BadRequest.ToString(),
                    Message = "Bad request",
                    Data = errors
                };
                return new BadRequestObjectResult(response);
            };
        });
    }

    private static void AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = GlobalData.ProjectName,
                Version = "1.0",
                Description = $"Service for {GlobalData.ProjectName} application",
            });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    private static IMvcBuilder AddCustomJsonOptions(this IMvcBuilder controllers)
    {
        controllers.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new DateTimeConverterTimeZone());
        });

        return controllers;
    }
}
