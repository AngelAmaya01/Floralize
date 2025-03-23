using System.ComponentModel.DataAnnotations;

namespace FBackend.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(5), MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();

    }
}
