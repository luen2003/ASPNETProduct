using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotAPIGateway
{
    public class Startup
    {
        private object audienceConfig;

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationProviderKey = "IdentityApiKey";

            var audienceConfig = Configuration.GetSection("Audience");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Iss"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Aud"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(double.Parse(audienceConfig["Expires"])),
                RequireExpirationTime = true,
            };

            // NUGET - Microsoft.AspNetCore.Authentication.JwtBearer
            services.AddAuthentication()
             .AddJwtBearer("Bearer", x =>
             {
                 x.Authority = "http://localhost:62363"; // IDENTITY SERVER URL
                 //x.RequireHttpsMetadata = false;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateAudience = false
                 };
             });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "TestKey";
            })
            .AddJwtBearer("TestKey", x =>
            {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddOcelot();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("I am Portal API Gateway!");
                });
            });
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            await app.UseOcelot();
        }
    }
}
