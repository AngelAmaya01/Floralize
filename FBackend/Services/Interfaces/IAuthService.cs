
using FBackend.Models;
using FBackend.Models.DTOs;

namespace ApiCitaOdon.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<string> LoginAsync(LoginDto model);
        Task<bool> RegisterAsync(RegisterDto model);
    }
}
