using Microsoft.EntityFrameworkCore;
using SiJwtAuth.Data.Models;

namespace SiJwtAuth.Data;

public sealed class AuthContext : DbContext
{
    public AuthContext(DbContextOptions opt) : base(opt)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Tokens> Tokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .HasIndex(it => it.Username)
            .IncludeProperties(it => it.PasswordHash)
            .IsUnique();
    }
}