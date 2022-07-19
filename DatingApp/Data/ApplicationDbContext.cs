using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<AppUserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }
}