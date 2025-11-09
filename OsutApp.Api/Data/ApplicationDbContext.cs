using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OsutApp.Api.Models;

namespace OsutApp.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add unique constraint on BoardMember.Position
        modelBuilder.Entity<BoardMember>()
            .HasIndex(b => b.Position)
            .IsUnique();
    }

    public DbSet<MemberWhitelist> MemberWhitelists { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventSignup> EventSignups { get; set; }
    public DbSet<BoardMember> BoardMembers { get; set; }
}