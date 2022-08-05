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
        builder.Services.AddDbContext<ApplicationDbContext>(
            options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("Postgres"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                )
        );

        builder.Services.AddSingleton<PresenceTracker>();
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddScoped<LogUserActivity>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserServicePostgres>();
        builder.Services.Configure<CloudinarySettings>(
            builder.Configuration.GetSection("CloudinarySettings")
        );
        builder.Services.AddScoped<ILikeService, LikeService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IMessageService, MessageService>();

        return builder;
    }
}