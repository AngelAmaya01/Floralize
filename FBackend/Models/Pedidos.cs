using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class Pedido
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual User Cliente { get; set; }

        [Required]
        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;

        [Required]
        public string Estado { get; set; } = "Pendiente";

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; } = 0; // Se calculará automáticamente

        public virtual ICollection<DetallePedido> Detalles { get; set; }
    }

}
