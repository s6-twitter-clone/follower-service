using follower_service.Models;
using Microsoft.EntityFrameworkCore;

namespace follower_service.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Follower> Followers { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");

        modelBuilder.Entity<Follower>().ToTable("Follower")
            .HasOne(u => u.FollowedUser).WithMany(p => p.Followed).HasForeignKey(p => p.FollowedUserId);

        modelBuilder.Entity<Follower>().HasOne(u => u.FollowingUser).WithMany(p => p.Following).HasForeignKey(p => p.FollowingUserId);

    }
}