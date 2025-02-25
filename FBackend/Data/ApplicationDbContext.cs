using FBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCitaOdon.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; } // Nueva tabla de Roles

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("UserRoles"));

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "Administrador del sistema" },
            new Role { Id = 2, Name = "Dentist", Description = "Dentista" },
            new Role { Id = 3, Name = "Patient", Description = "Paciente" }
        );
    }
}