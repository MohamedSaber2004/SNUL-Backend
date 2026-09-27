using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;
using Serilog;
using SNUL.Shared;
using SNUL.Shared.Common.Behaviors;
using SNUL.Shared.Common.Extensions;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Middlewares;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Localization;
using SNUL.Shared.OpenApi;
using SNUL.Shared.Persistance;
using SNUL.Shared.Persistance.Seeding;

namespace Product.Services.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environmentName))
            {
                environmentName = "Development";
            }

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                EnvironmentName = environmentName,
                ContentRootPath = AppContext.BaseDirectory
            });

var env = builder.Environment;

            builder.Configuration.Sources.Clear();
            builder.Configuration
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null) builder.Configuration.AddUserSecrets(appAssembly, optional: true);
            }

            builder.Configuration.AddEnvironmentVariables().AddCommandLine(args);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateBootstrapLogger();

            Log.Information("SNUL Product Microservice is starting up at {Time}", DateTime.Now);
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            var port = Environment.GetEnvironmentVariable("PORT")
                       ?? Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS");
            if (!string.IsNullOrEmpty(port))
            {
                builder.WebHost.UseUrls($"http://*:{port}");
            }

            builder.Services.AddControllers();
            builder.Services.AddJsonLocalization();
            builder.Services.AddSnulSharedDependencies(builder.Configuration);
            builder.Services.AddSnulIdentity(builder.Configuration);

            builder.Services.AddSnulJwtAuthentication(builder.Configuration);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            builder.Services.AddConfiguredOpenApi();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection")
                ?? builder.Configuration["DatabaseConnection"];

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                                   Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
            });

            app.UseCustomExceptionHandler();
            app.UseJsonLocalization();

            if (!app.Environment.IsEnvironment("Test") && !app.Environment.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            app.UseCors("AllowAll");
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/", () => Results.Redirect("/scalar/v1"));
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("Product Microservice API")
                       .WithTheme(ScalarTheme.Moon);
            });
            app.MapControllers();

            if (!app.Environment.IsEnvironment("Test"))
            {
                try
                {
                    using var scope = app.Services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SnulDbContext>();
                    await db.Database.MigrateAsync();
                    await CurrencySeeder.SeedAsync(db);
                    await WorldLocationSeeder.SeedAsync(db);

                    if (BogusDemoSeeder.ShouldSeedDemoData(app.Environment, app.Configuration, out _))
                    {
                        await BogusDemoSeeder.SeedDemoAsync(scope.ServiceProvider);
                    }
                }
                catch (Exception ex)
                {
                    var logger = app.Services.GetService<ILogger<Program>>();
                    logger?.LogError(ex, "Product startup migration/seeding failed. DB may be unreachable. This explains 500.30 startup failure.");
                    Console.Error.WriteLine($"[Product] Startup migration/seeding failed: {ex.GetType().Name}: {ex.Message}");
                }
            }

            await app.RunAsync();
        }
    }
}
