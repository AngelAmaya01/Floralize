using FBackend.Models.DTOs;
using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Models.Task;

namespace FBackend.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<ResponseDto<PedidosDto>> ActualizarEstado(Guid id, string nuevoEstado);
        Task<ResponseDto<PedidosDto>> CrearPedido(PedidoCreateDto model);
        Task<ResponseDto<List<PedidosDto>>> ObtenerPedidos();
        Task<ResponseDto<PedidosDto>> ObtenerPorId(Guid id);
        Task<ResponseDto<IEnumerable<PedidosDto>>> ObtenerTodos();
    }
}