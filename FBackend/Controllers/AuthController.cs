using ApiCitaOdon.Data;
using ApiCitaOdon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using ApiCitaOdon.Services.Interfaces;
using FBackend.Models.DTOs;
using FBackend.Models;

namespace ApiCitaOdon.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IAuthService _authService;

    public AuthController(
        ApplicationDbContext context,
        IConfiguration configuration,
        IEmailService emailService,
        IAuthService authService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
        _authService = authService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(new { status = true, data = users });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = false, message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        try
        {
            await _authService.RegisterAsync(model);
            return Ok("Usuario registrado exitosamente");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        //if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        //{
        //    return BadRequest("El correo electrónico ya está registrado");
        //}

        //var user = new User
        //{
        //    Email = model.Email,
        //    FirstName = model.FirstName,
        //    LastName = model.LastName,
        //    PhoneNumber = model.PhoneNumber,
        //    PasswordHash = HashPassword(model.Password),
        //};

        //_context.Users.Add(user);
        //await _context.SaveChangesAsync();
        //await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);

        //return Ok("Usuario registrado exitosamente");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var user = await _context.Users
            .Include(u => u.Roles) //incluye el rol al logearse
            .FirstOrDefaultAsync(u => u.Username == model.Username);

        if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
        {
            return Unauthorized("Credenciales inválidas");
        }

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });

        //var user = await _context.Users
        //   //incluye el rol al logearse
        //   .Include(u => u.Roles)
        //   .FirstOrDefaultAsync(u => u.Email == model.Email);

        //if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
        //{
        //    return Unauthorized("Credenciales inválidas");
        //}

        ////para saber que rol entra
        //var roles = user.Roles.Select(r => r.Name).ToList();
        //var token = GenerateJwtToken(user);
        //return Ok(new
        //{
        //    Token = token,
        //    // administracion del rol
        //    user.Id,
        //    user.Email,
        //    Roles = roles
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            return Ok("Si el correo existe, recibirás instrucciones para restablecer tu contraseña");
        }

        var token = GenerateResetToken();
        user.ResetPasswordToken = token;
        user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _context.SaveChangesAsync();
        await _emailService.SendPasswordResetEmailAsync(user.Email, token);

        return Ok("Se han enviado las instrucciones de recuperación a tu correo");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.ResetPasswordToken == model.Token);

        if (user == null || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
        {
            return BadRequest("Token inválido o expirado");
        }

        user.PasswordHash = HashPassword(model.NewPassword);
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiry = null;

        await _context.SaveChangesAsync();
        return Ok("Contraseña actualizada exitosamente");
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
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Obtiene los roles del usuario
        var roles = user.Roles.Select(r => r.Name).ToList();

        //var claims = new[]
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FirstName)
        };

        // Agregar los roles con las claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateResetToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
    }
}