using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.ValidationsDto
{
    public class CategoriaCreateDto
    {
        [Required]
        [StringLength(150, ErrorMessage = "La descripción no debe exceder los 150 caracteres.")]
        public string Descripcion { get; set; }
    }
}
