using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class Proveedores
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
