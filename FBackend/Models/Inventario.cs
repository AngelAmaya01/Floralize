using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class Inventario
    {
        [Key]
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        [Required]
        public int Cantidad { get; set; }

        [Required, StringLength(100)]
        public string Ubicacion { get; set; }

        //relacion con categoria
        [ForeignKey("Categoria")]
        public Guid CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }
          
    }
}
