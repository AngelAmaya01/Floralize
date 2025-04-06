

using FBackend.Models;

namespace ApiCitaOdon.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendCustomOrderConfirmationAsync(Guid pedidoId, string clienteEmail);
        Task SendLowStockNotificationAsync(Guid productoId, int cantidadActual);
        Task SendOrderCompletedEmailAsync(Guid pedidoId, string clienteEmail, string tipoPedido, string detalles, decimal? total = null);
        Task SendOrderConfirmationAsync(Guid pedidoId, string clienteEmail);
        Task SendOrderReadyEmailAsync(Pedido pedido);
        Task SendPasswordResetEmailAsync(string to, string token);
        Task SendWelcomeEmailAsync(string to, string name);
    }
}
