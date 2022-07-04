using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public static class Seed
    {
        public static async Task ManageDataAsync(IHost host)
        {
            using IServiceScope svcScope = host.Services.CreateScope();
            IServiceProvider svcProvider = svcScope.ServiceProvider;

            try
            {
                //Service: An instance of RoleManager
                ApplicationDbContext dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
                //Migration: This is the programmatic equivalent to Update-Database
                await dbContextSvc.Database.MigrateAsync();

                await SeedUsersAsync(dbContextSvc);
            }
            catch (Exception ex)
            {
                var logger = svcProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration/seeding data");

                Console.WriteLine($"{ex}: {ex.Message}");
                throw;
            }
        }


        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.Name = user.Name.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$word"));
                user.PasswordSalt = hmac.Key;

                await context.Users.AddAsync(user);
            }

            await context.SaveChangesAsync();
        }
    }
}