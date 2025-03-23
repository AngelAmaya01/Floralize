using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class Proveedores
    {
        public Guid Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Telefono { get; set; }
        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Direccion { get; set; }
    }
}
