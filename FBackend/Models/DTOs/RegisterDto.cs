using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs;

public class RegisterDto
{
    [Required]
    [MinLength(5), MaxLength(30)]
    public string Username { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string? PhoneNumber { get; set; }
}