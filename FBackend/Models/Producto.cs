using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class Producto
    {
        [Key]
        public Guid Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Required, StringLength(50)]
        public string Categoria { get; set; }

        public string ImagenUrl { get; set; }

        [Required]
        public int Stock { get; set; }

        [ForeignKey("Proveedor")]
        public Guid ProveedorId { get; set; }


        public virtual Proveedores Proveedor { get; set; }
    }
}
