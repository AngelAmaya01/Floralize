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
    public DbSet<Role> Roles { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Proveedores> Proveedores { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Pedidos> Pedidos { get; set; }
    public DbSet<Inventario> Inventario { get; set; }
    public DbSet<DetallePedido> DetallePedidos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("UserRoles"));

        // Relación uno a muchos entre Pedido y DetallePedido
        modelBuilder.Entity<DetallePedido>()
            .HasOne(dp => dp.Pedidos)
            .WithMany(p => p.Detalles)
            .HasForeignKey(dp => dp.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);


        //Gestion de roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin", Description = "Administrador del sistema" },
            new Role { Id = 2, Name = "Gerente", Description = "Gerencia/Supervisor" },
            new Role { Id = 3, Name = "Vendedor", Description = "Gestion de ventas" },
            new Role { Id = 4, Name = "Inventario", Description = "Encargado de inventario" },
            new Role { Id = 5, Name = "User", Description = "Usuario" }
        );
    }
}