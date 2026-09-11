using Auth.Services.API.Infrastructure;
using Auth.Services.API.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;
using System.Text;
using SNUL.Shared;
using SNUL.Shared.Common.Behaviors;
using SNUL.Shared.Common.Interfaces;
using SNUL.Shared.Common.Middlewares;
using SNUL.Shared.Common.Options;
using SNUL.Shared.Domain.Models;
using SNUL.Shared.Localization;
using SNUL.Shared.Localization.Interfaces;
using SNUL.Shared.OpenApi;
using SNUL.Shared.Persistance;
using SNUL.Shared.Persistance.Seeding;


public class Program
{
    public static async Task Main(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


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

        Log.Information("SNUL Auth Microservice is starting up at {Time}", DateTime.Now);
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

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
        var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
        builder.Services.AddSingleton(jwtSettings);

        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));
        var emailSettings = builder.Configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>() ?? new EmailSettings();
        builder.Services.AddSingleton(emailSettings);

        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        var secretKey = !string.IsNullOrWhiteSpace(jwtSettings.Secret) && jwtSettings.Secret.Length >= 32
            ? jwtSettings.Secret
            : "V5B?*77+gzD_pk+2!%ORg<i)<D$DH+Xf.nECc?];2l;";

        var validIssuers = jwtSettings.GetAllValidIssuers().ToList();
        var validAudiences = jwtSettings.GetAllValidAudiences().ToList();

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = validIssuers.Count > 0,
            ValidIssuers = validIssuers.Count > 0 ? validIssuers : null,
            ValidateAudience = validAudiences.Count > 0,
            ValidAudiences = validAudiences.Count > 0 ? validAudiences : null,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
        };
        builder.Services.AddSingleton(tokenValidationParameters);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = tokenValidationParameters;
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var localizer = context.HttpContext.RequestServices.GetService<ILocalizationProvider>();
                    var localizedMessage = localizer?.GetLocalizedString(LocalizationKeys.ExceptionMessages.Unauthorized);

                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        isSuccess = false,
                        statusCode = StatusCodes.Status401Unauthorized,
                        message = localizedMessage,
                        errors = new[] { localizedMessage },
                        data = (object?)null
                    });

                    return context.Response.WriteAsync(result);
                }
            };
        });

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

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                await RoleSeeder.SeedRolesAsync(roleManager);
                await UserSeeder.SeedUsersAsync(userManager);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
                logger?.LogError(ex, "Auth startup seeding failed. DB may be unreachable. This will cause login/DB calls to return 500 until fixed.");
                Console.Error.WriteLine($"[Auth] Startup seeding failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

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
            options.WithTitle("SNUL Auth Microservice API")
                   .WithTheme(ScalarTheme.Moon);
        });
        app.MapControllers();

        app.Run();
    }
}
}
