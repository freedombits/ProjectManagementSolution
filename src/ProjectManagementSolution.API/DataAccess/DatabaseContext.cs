using Microsoft.EntityFrameworkCore;
using ProjectManagementSolution.API.Features.Users;

namespace ProjectManagementSolution.API.DataAccess;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserRefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; } = null!;

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}
