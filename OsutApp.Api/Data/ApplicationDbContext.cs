using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OsutApp.Api.Models;

namespace OsutApp.Api.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MemberWhitelist> MemberWhitelists { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}