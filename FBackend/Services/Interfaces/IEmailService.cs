
namespace ApiCitaOdon.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string to, string token);
        Task SendWelcomeEmailAsync(string to, string name);
    }
}
