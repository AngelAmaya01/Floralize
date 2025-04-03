using FBackend.Models.DTOs;
using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Models.Task;

namespace FBackend.Services.Interfaces
{
    public interface IDetallePedidoService
    {
        Task<ResponseDto<List<DetallePedidoDto>>> ObtenerDetallesPedidos();
        Task<ResponseDto<List<DetallePedidoDto>>> ObtenerDetallesPorCliente(Guid clienteId);
        Task<ResponseDto<List<DetallePedidoDto>>> ObtenerDetallesPorPedido(Guid pedidoId);
        Task<ResponseDto<List<PedidosDto>>> ObtenerPedidosDetalles();
    }
}