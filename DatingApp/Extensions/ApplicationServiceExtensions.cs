using AspNetCoreRateLimit;
using DatingApp.Data;
using DatingApp.Helpers;
using DatingApp.Services;
using DatingApp.Services.Interfaces;
using DatingApp.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Extensions;

public static class ApplicationServiceExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string connStr;

            if (env == "Development")
            {
                connStr = builder.Configuration.GetConnectionString("Postgres");
            }
            else
            {
                var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                connUrl = connUrl?.Replace("postgres://", string.Empty);
                var pgUserPass = connUrl?.Split("@")[0];
                var pgHostPortDb = connUrl?.Split("@")[1];
                var pgHostPort = pgHostPortDb?.Split("/")[0];
                var pgDb = pgHostPortDb?.Split("/")[1];
                var pgUser = pgUserPass?.Split(":")[0];
                var pgPass = pgUserPass?.Split(":")[1];
                var pgHost = pgHostPort?.Split(":")[0];
                var pgPort = pgHostPort?.Split(":")[1];

                connStr =
                    $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;TrustServerCertificate=True";
            }

            options.UseNpgsql(
                connStr,
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            );
        });

        builder.Services.AddOptions();
        builder.Services.AddMemoryCache();

        builder.Services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.HttpStatusCode = 429;
            options.RealIpHeader = "X-Real-IP";
            options.ClientIdHeader = "X-ClientId";
            options.GeneralRules = new List<RateLimitRule>
            {
                new()
                {
                    Endpoint = "*",
                    Period = "30s",
                    Limit = 15
                },
                new()
                {
                    Endpoint = "post:/api/account/register",
                    Period = "3h",
                    Limit = 2
                },
                new()
                {
                    Endpoint = "post:/api/users/add-photo",
                    Period = "3h",
                    Limit = 5
                }
            };
        });

        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        builder.Services.AddSingleton<PresenceTracker>();
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddScoped<LogUserActivity>();
        builder.Services.Configure<CloudinarySettings>(
            builder.Configuration.GetSection("CloudinarySettings")
        );
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<ITokenService, TokenService>();

        return builder;
    }
}