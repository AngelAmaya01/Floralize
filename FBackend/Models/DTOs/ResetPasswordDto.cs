using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; }

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; }
}