using ApiCitaOdon.Data;
using ApiCitaOdon.Services.Interfaces;
using FBackend.Models;
using FBackend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiCitaOdon.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(
        ApplicationDbContext context,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
        .Include(u => u.Roles)
        .Select(u => new User
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhoneNumber = u.PhoneNumber,
            CreatedAt = u.CreatedAt,
            Roles = u.Roles.Select(r => new Role
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            }).ToList()
        })
        .ToListAsync();
    }

    public async Task<string> LoginAsync(LoginDto model)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username == model.Username);

        if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Credenciales inválidas");
        }

        return GenerateJwtToken(user);
    }

    public async Task<bool> RegisterAsync(RegisterDto model)
    {
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            throw new InvalidOperationException("El correo electrónico ya está registrado");
        }

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            Address = model.Address,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            PasswordHash = HashPassword(model.Password)
        };

        // Asignar rol de paciente por defecto
        var patientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        if (patientRole != null)
        {
            user.Roles.Add(patientRole);
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);

        return true;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FirstName)
        };

        // Agregar roles como claims
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}