using FBackend.Models.DTOs;
using FBackend.Models.Task;
using FBackend.Models.ValidationsDto;

namespace FBackend.Services.Interfaces
{
    public interface IPersonalizadoService
    {
        Task<ResponseDto<PersonalizadoDto>> CrearPedidoPersonalizado(PersonalizadoCreateDto model);
        Task<ResponseDto<PersonalizadoDto>> EditarPedidoPersonalizado(Guid id, PersonalizadoCreateDto model);
        Task<ResponseDto<PersonalizadoDto>> EliminarPedidoPersonalizado(Guid id);
        Task<ResponseDto<PersonalizadoDto>> ObtenerPedidoPersonalizado(Guid id);
        Task<ResponseDto<List<PersonalizadoDto>>> ObtenerPedidosPorCliente(string clienteId);
        Task<ResponseDto<List<PersonalizadoDto>>> ObtenerPedidosPersonalizados();
        Task<ResponseDto<PersonalizadoDto>> ActualizarEstado(Guid id, string nuevoEstado);
    }
}