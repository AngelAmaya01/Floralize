using FBackend.Models.DTOs;
using FBackend.Models.Task;
using FBackend.Models.ValidationsDto;

namespace FBackend.Services.Interfaces
{
    public interface IProveedorService
    {
        Task<ResponseDto<ProveedorDto>> CreateProveedor(ProveedorCreateDto proveedorCreateDto);
        Task<ResponseDto<ProveedorDto>> DeleteProveedor(Guid id);
        Task<ResponseDto<ProveedorDto>> GetProveedorById(Guid id);
        Task<ResponseDto<List<ProveedorDto>>> GetProveedores();
    }
}