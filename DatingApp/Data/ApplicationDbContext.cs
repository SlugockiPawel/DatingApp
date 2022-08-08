using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data;

public class ApplicationDbContext
    : IdentityDbContext<
        AppUser,
        AppRole,
        Guid,
        IdentityUserClaim<Guid>,
        AppUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<AppUser>()
            .HasMany(user => user.UserRoles)
            .WithOne(uRole => uRole.User)
            .HasForeignKey(uRole => uRole.UserId)
            .IsRequired();

        modelBuilder
            .Entity<AppRole>()
            .HasMany(r => r.UserRoles)
            .WithOne(uRole => uRole.Role)
            .HasForeignKey(uRole => uRole.RoleId)
            .IsRequired();

        modelBuilder.Entity<AppUserLike>().HasKey(k => new { k.SourceUserId, k.LikedUserId });

        modelBuilder
            .Entity<AppUserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<AppUserLike>()
            .HasOne(l => l.LikedUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(l => l.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesRecieved)
            .OnDelete(DeleteBehavior.Restrict);
    }
}