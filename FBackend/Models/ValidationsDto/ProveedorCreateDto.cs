using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.ValidationsDto
{
    public class ProveedorCreateDto
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Telefono { get; set; }
        [Required]
        public string Correo { get; set; }
        [Required]
        public string Direccion { get; set; }
    }
}
