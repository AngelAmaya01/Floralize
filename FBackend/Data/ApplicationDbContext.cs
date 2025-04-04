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
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<Inventario> Inventario { get; set; }
    public DbSet<DetallePedido> DetallePedidos { get; set; }
    public DbSet<Personalizado> Personalizados { get; set; }
    public DbSet<Categoria> Categoria { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("UserRoles"));

        // Relación uno a muchos entre Pedido y DetallePedido
        modelBuilder.Entity<DetallePedido>()
            .HasOne(dp => dp.Pedido)
            .WithMany(p => p.Detalles)
            .HasForeignKey(dp => dp.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);


        //Gestion de roles
        modelBuilder.Entity<Role>().HasData(
         new Role { Id = Guid.Parse("a3b7f98d-4e1c-41c2-8e7b-5a6f3d9a8b2c"), Name = "Admin", Description = "Administrador del sistema" },
         new Role { Id = Guid.Parse("f4c2e3d1-987a-4b6c-a5f8-d3e2c7b9a1d4"), Name = "Gerente", Description = "Gerencia/Supervisor" },
         new Role { Id = Guid.Parse("b8d7a9c3-5e4f-4c2b-9d6a-7f1e3d8c2a4b"), Name = "Vendedor", Description = "Gestion de ventas" },
         new Role { Id = Guid.Parse("d5e1f4a7-8c9b-6b2d-3a7c-5f4e1d2b9a8c"), Name = "Inventario", Description = "Encargado de inventario" },
         new Role { Id = Guid.Parse("c9b2d6a3-7f4e-1d8c-5a7b-3e2f4d9c1b8a"), Name = "User", Description = "Usuario" }
        );
    }
}