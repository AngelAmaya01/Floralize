using FBackend.Models.DTOs;
using FBackend.Models.DTOs.PedidiosDtos;
using FBackend.Models.DTOs.PedidosCreateDtos;

namespace FBackend.Services.Interfaces
{
    public interface IProductoService
    {
        Task<ResponseDto<ProductoDto>> Actualizar(Guid id, ProductoUpdateDto model);
        Task<ResponseDto<ProductoDto>> CrearP(ProductoCreateDto model);
        Task<ResponseDto<bool>> Eliminar(Guid id);
        Task<ResponseDto<ProductoDto>> ObtenerPorId(Guid id);
        Task<ResponseDto<IEnumerable<ProductoDto>>> ObtenerTodos();
    }
}