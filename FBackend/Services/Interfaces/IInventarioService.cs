using FBackend.Models.DTOs;
using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Models.Task;

namespace FBackend.Services.Interfaces
{
    public interface IInventarioService
    {
        Task<ResponseDto<InventarioDto>> agregarProducto(InventarioCreateDtos model);
        Task<ResponseDto<InventarioDto>> EditarProducto(Guid id, InventarioCreateDtos model);
        Task<ResponseDto<InventarioDto>> EliminarProducto(Guid id);
        Task<ResponseDto<List<InventarioDto>>> ObtenerProductos();
    }
}