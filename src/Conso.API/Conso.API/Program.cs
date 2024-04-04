using Azure.Identity;
using Conso.API.Authorization;
using Conso.API.Exceptions;
using Conso.API.Mapping;
using Conso.Core.DependencyInjections;
using Conso.Providers.DependencyInjections;
using Conso.Providers;
using Conso.Providers.Mapping;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Conso.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            if (builder.Environment.IsProduction())
            {
                ConfigureKeyVault(builder);
            }

            // Add services to the container.
            ConfigureAuth0(builder.Services, builder.Configuration);
            ConfigureServices(builder.Services, builder.Configuration, builder.Environment);
            ConfigureHealthChecks(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            Configure(app, app.Environment);

            app.Run();
        }

        private static void ConfigureKeyVault(WebApplicationBuilder builder)
        {
            var configMnager = builder.Configuration;

            var keyVaultUrl = configMnager["Conso:KeyVault:Url"]!;
            // var keyVaultClientId = configMnager["Conso:KeyVault::ClientId"];
            // var keyVaultClientSecret = configMnager["Conso:KeyVault::ClientSecret"];

            configMnager.AddAzureKeyVault(
                new Uri(keyVaultUrl),
                new DefaultAzureCredential());

        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddControllers();

            services.AddCore()
                .AddProviders(config);

            services.AddDbContext<ConsoDbContext>( options =>
                {
                    options.UseSqlServer(
                        config["Conso:ConnectionString"],
                        opts => opts.MigrationsHistoryTable("__EFMigrationHistory", "Conso")
                    );
                    if(env.IsDevelopment())
                    {
                        options.EnableSensitiveDataLogging();
                    }
                }
            );

            services.AddAutoMapper(
                typeof(ConsoApiProfiler),
                typeof(ConsoProvidersProfile)
                );
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = config["Conso:ApplicationInsights:ConnectionString"];
            });
        }

        private static void Configure(WebApplication app, IWebHostEnvironment env)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new GlobalExceptionHandler(logger).Invoke
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            //HealthCheck Middleware
            app.MapHealthChecks("/api/health", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }

        private static void ConfigureAuth0(IServiceCollection services, IConfiguration config)
        {
            var authority = config["Conso:Auth0:Authority"];
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = config["Conso:Auth0:Audience"];
                //options.TokenValidationParameters = new TokenValidationParameters
                //{
                //    NameClaimType = ClaimTypes.NameIdentifier
                //};
                options.SaveToken = true;
#if DEBUG
                options.RequireHttpsMetadata = false;
#endif
            });

            services.AddAuthorization(options =>
            {
                foreach (var permission in ConsoPermissions.Permissions)
                {
                    options.AddPolicy(permission,
                        policy => policy.Requirements.Add(
                            new HasScopeRequirement(permission, authority!)
                        )
                    );
                }
            });

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        }

        private static void ConfigureHealthChecks(IServiceCollection services, IConfiguration config)
        {
            services.AddHealthChecks()
                .AddSqlServer(
                    config["Conso:ConnectionStirng"]!,
                    healthQuery: "select 1",
                    name: "SQL Server",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["Feedback", "Database"]);
        }
    }
}
