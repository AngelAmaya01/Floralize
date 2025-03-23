using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class DetallePedido
    {
        [Key]
        public Guid Id { get; set; }

        [Required, ForeignKey("Pedido")]
        public Guid PedidoId { get; set; }

        [Required, ForeignKey("Producto")]
        public Guid ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public virtual Pedidos Pedidos { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
