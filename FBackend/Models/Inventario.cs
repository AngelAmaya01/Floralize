using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class Inventario
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Producto")]
        public int ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required, StringLength(100)]
        public string Ubicacion { get; set; }

        [Required]
        public DateTime FechaActualizacion { get; set; }

        public virtual Producto Producto { get; set; }
    }
}
