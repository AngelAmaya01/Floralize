using ApiCitaOdon.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalAPI.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoleController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _context.Roles.ToListAsync();
        return Ok(roles);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var users = await _context.Users
            .Include(u => u.Roles)
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                Roles = u.Roles.Select(r => new { r.Id, r.Name })
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost("assign/{userId}")]
    public async Task<IActionResult> AssignRole(int userId, [FromBody] int roleId)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound("Usuario no encontrado");

        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
            return NotFound("Rol no encontrado");

        if (user.Roles.Any(r => r.Id == roleId))
            return BadRequest("El usuario ya tiene este rol");

        user.Roles.Add(role);
        await _context.SaveChangesAsync();

        return Ok("Rol asignado exitosamente");
    }

    [HttpDelete("remove/{userId}/{roleId}")]
    public async Task<IActionResult> RemoveRole(int userId, int roleId)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound("Usuario no encontrado");

        var role = user.Roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
            return NotFound("El usuario no tiene este rol");

        // Evitar que un usuario se quite a sí mismo el rol de Admin
        var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        if (userId == currentUserId && role.Name == "Admin")
        {
            return BadRequest("No puedes quitarte a ti mismo el rol de Administrador");
        }

        user.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return Ok("Rol removido exitosamente");
    }
}