using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs;

public class LoginDto
{
    [Required]
    [MinLength(5), MaxLength(30)]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}