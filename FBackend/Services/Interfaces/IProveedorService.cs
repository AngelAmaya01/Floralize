using FBackend.Models.DTOs;
using FBackend.Models.Task;
using FBackend.Models.ValidationsDto;

namespace FBackend.Services.Interfaces
{
    public interface IProveedorService
    {
        Task<ResponseDto<ProveedorDto>> CreateProveedor(ProveedorCreateDto proveedorCreateDto);
        Task<ResponseDto<ProveedorDto>> DeleteProveedor(int id);
        Task<ResponseDto<ProveedorDto>> GetProveedorById(int id);
        Task<ResponseDto<List<ProveedorDto>>> GetProveedores();
    }
}