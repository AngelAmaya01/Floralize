
using FBackend.Models.DTOs;

namespace ApiCitaOdon.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDto model);
        Task<bool> RegisterAsync(RegisterDto model);
    }
}
