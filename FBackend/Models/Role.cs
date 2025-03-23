using System.ComponentModel.DataAnnotations;

namespace FBackend.Models;

public class Role
{
    public Guid Id { get; set; } 

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}
