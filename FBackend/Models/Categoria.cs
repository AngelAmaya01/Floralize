using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class Categoria
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(150)]
        public string Descripcion { get; set; }
     
    }
}
