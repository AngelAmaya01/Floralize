using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}