using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.ValidationsDto
{
    public class PersonalizadoCreateDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "El tipo de flor no debe exceder los 50 caracteres.")]
        public string TipoFlor { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
        public string Cantidad { get; set; }
        //que solo pueda poner si o no
        [Required]
        [StringLength(2, ErrorMessage = "El campo IncluirPresente solo puede tener 3 caracteres.")]
        public string IncluirPresente { get; set; }
        //que solo pueda poner si o no
        [Required]
        [StringLength(2, ErrorMessage = "El campo IncluirBase solo puede tener 3 caracteres.")]
        public string IncluirBase { get; set; }

        public string TipoPresente { get; set; } 
        public string TipoBase { get; set; }

        public IFormFile File { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public Guid UserId { get; set; }
    }
}
