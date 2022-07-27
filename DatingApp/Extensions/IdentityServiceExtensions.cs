using System.Text;
using DatingApp.Data;
using DatingApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Extensions;

public static class IdentityServiceExtensions
{
    public static WebApplicationBuilder AddIdentityServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentityCore<AppUser>(opt => { opt.Password.RequireNonAlphanumeric = false; })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"])
                    ),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        return builder;
    }
}