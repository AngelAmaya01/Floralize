using FBackend.Models.DTOs;
using FBackend.Models.Task;
using FBackend.Models.ValidationsDto;

namespace FBackend.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<ResponseDto<CategoriaDto>> CrearCategoria(CategoriaCreateDto model);
        Task<ResponseDto<CategoriaDto>> EditarCategoria(int id, CategoriaCreateDto model);
        Task<ResponseDto<CategoriaDto>> EliminarCategoria(int id);
        Task<ResponseDto<List<CategoriaDto>>> ObtenerCategorias();
    }
}