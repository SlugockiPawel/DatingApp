using System.Text.Json;
using DatingApp.Enums;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data;

public static class Seed
{
    public static async Task ManageDataAsync(IHost host)
    {
        using var svcScope = host.Services.CreateScope();
        var svcProvider = svcScope.ServiceProvider;

        try
        {
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<AppRole>>();
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            //Migration: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

            await SeedUsersAsync(userManagerSvc, roleManagerSvc);
        }
        catch (Exception ex)
        {
            var logger = svcProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during migration/seeding data");

            Console.WriteLine($"{ex}: {ex.Message}");
            throw;
        }
    }

    private static async Task SeedUsersAsync(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager
    )
    {
        if (await userManager.Users.AnyAsync())
            return;

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

        if (users is null)
            return;

        await PopulateRoles(roleManager);

        foreach (var user in users)
        {
            user.UserName = user.UserName.ToLower();
            await userManager.CreateAsync(user, "Pa$$word");
            await userManager.AddToRoleAsync(user, nameof(Roles.Member));
        }

        var admin = new AppUser { UserName = "admin" };

        await userManager.CreateAsync(admin, "Pa$$word");
        await userManager.AddToRolesAsync(
            admin,
            new[] { nameof(Roles.Admin), nameof(Roles.Moderator) }
        );
    }

    private static async Task PopulateRoles(RoleManager<AppRole> roleManager)
    {
        var roles = new List<AppRole>
        {
            new() { Name = nameof(Roles.Member) },
            new() { Name = nameof(Roles.Admin) },
            new() { Name = nameof(Roles.Moderator) }
        };

        foreach (var role in roles)
            await roleManager.CreateAsync(role);
    }
}