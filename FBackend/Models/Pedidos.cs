using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class Pedidos
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Cliente")]
        public int ClienteId { get; set; }

        [Required]
        public DateTime FechaPedido { get; set; }

        [Required, StringLength(20)]
        public string Estado { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public virtual User Cliente { get; set; }
        public virtual List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}
