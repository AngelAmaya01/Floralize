using FBackend.Models.DTOs;
using FBackend.Models.Task;
using FBackend.Models.ValidationsDto;

namespace FBackend.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<ResponseDto<CategoriaDto>> CrearCategoria(CategoriaCreateDto model);
        Task<ResponseDto<CategoriaDto>> EditarCategoria(Guid id, CategoriaCreateDto model);
        Task<ResponseDto<CategoriaDto>> EliminarCategoria(Guid id);
        Task<ResponseDto<List<CategoriaDto>>> ObtenerCategorias();
    }
}