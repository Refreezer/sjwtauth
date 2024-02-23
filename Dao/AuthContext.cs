using Microsoft.EntityFrameworkCore;
using SiJwtAuth.Dao.Models;

namespace SiJwtAuth.Dao;

public class AuthContext(DbContextOptions opt) : DbContext(opt)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .HasIndex(it => it.Username)
            .IncludeProperties(it => it.PasswordHash)
            .IsUnique();
    }
}