using ApiCitaOdon.Data;
using AutoMapper;
using FBackend.Models;
using FBackend.Models.DTOs;
using FBackend.Models.Task;
using FBackend.Models.ValidationsDto;
using FBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FBackend.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoriaService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseDto<CategoriaDto>> CrearCategoria(CategoriaCreateDto model)
        {
            var categoria = new Categoria
            {
                Descripcion = model.Descripcion
            };

            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync();

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);

            return new ResponseDto<CategoriaDto>
            {
                Status = true,
                StatusCode = 201,
                Message = "Categoria creada con exito",
                Data = categoriaDto
            };
        }

        // Obtener todas las categorias
        public async Task<ResponseDto<List<CategoriaDto>>> ObtenerCategorias()
        {
            var categorias = await _context.Categoria.ToListAsync();
            var categoriasDto = _mapper.Map<List<CategoriaDto>>(categorias);

            return new ResponseDto<List<CategoriaDto>>
            {
                Status = true,
                StatusCode = 200,
                Message = "Categorias obtenidas con exito",
                Data = categoriasDto
            };
        }

        //editar categoria por id usando el mismo modelo de crear categoria
        public async Task<ResponseDto<CategoriaDto>> EditarCategoria(Guid id, CategoriaCreateDto model)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                return new ResponseDto<CategoriaDto>
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Categoria no encontrada"
                };
            }

            categoria.Descripcion = model.Descripcion;

            _context.Categoria.Update(categoria);
            await _context.SaveChangesAsync();

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);

            return new ResponseDto<CategoriaDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Categoria editada con exito",
                Data = categoriaDto
            };
        }

        //eliminar categoria por id
        public async Task<ResponseDto<CategoriaDto>> EliminarCategoria(Guid id)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                return new ResponseDto<CategoriaDto>
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Categoria no encontrada"
                };
            }

            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);

            return new ResponseDto<CategoriaDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Categoria eliminada con exito",
                Data = categoriaDto
            };
        }

    }
}
