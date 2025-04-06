using ApiCitaOdon.Data;
using ApiCitaOdon.Services.Interfaces;
using AutoMapper;
using FBackend.Models;
using FBackend.Models.DTOs;
using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Models.Task;
using FBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FBackend.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly int _lowStockThreshold = 10;

        public InventarioService(ApplicationDbContext context,  IMapper mapper, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<ResponseDto<InventarioDto>> agregarProducto(InventarioCreateDtos model)
        {
            var inventario = new Inventario
            {
                Nombre = model.Nombre,
                Cantidad = model.Cantidad,
                Ubicacion = model.Ubicacion,
                CategoriaId = model.CategoriaId
            };

            _context.Inventario.Add(inventario);
            await _context.SaveChangesAsync();

            var inventarioDto = _mapper.Map<InventarioDto>(inventario);

            return new ResponseDto<InventarioDto>
            {
                Status = true,
                StatusCode = 201,
                Message = "Producto agregado con exito",
                Data = inventarioDto
            };
        }

        // Obtener todos los productos pero en vez de CategoriaId traer el nombre de la categoria
        public async Task<ResponseDto<List<InventarioDto>>> ObtenerProductos()
        {
            var inventarios = await _context.Inventario
                .Include(i => i.Categoria)
                .ToListAsync();

            var inventariosDto = _mapper.Map<List<InventarioDto>>(inventarios);

            return new ResponseDto<List<InventarioDto>>
            {
                Status = true,
                StatusCode = 200,
                Message = "Productos obtenidos con exito",
                Data = inventariosDto
            };
        }

        //editar producto por id usando el mismo modelo de crear producto
        public async Task<ResponseDto<InventarioDto>> EditarProducto(Guid id, InventarioCreateDtos model)
        {
            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario == null)
            {
                return new ResponseDto<InventarioDto>
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Producto no encontrado",
                    Data = null
                };
            }

            // Verificar si el stock bajó del umbral
            bool wasAboveThreshold = inventario.Cantidad > _lowStockThreshold;
            bool isNowBelowThreshold = model.Cantidad <= _lowStockThreshold;

            inventario.Nombre = model.Nombre;
            inventario.Cantidad = model.Cantidad;
            inventario.Ubicacion = model.Ubicacion;
            inventario.CategoriaId = model.CategoriaId;

            _context.Inventario.Update(inventario);
            await _context.SaveChangesAsync();

            // Enviar notificación si el stock bajó del umbral
            if (wasAboveThreshold && isNowBelowThreshold)
            {
                await _emailService.SendLowStockNotificationAsync(id, model.Cantidad);
            }

            var inventarioDto = _mapper.Map<InventarioDto>(inventario);

            return new ResponseDto<InventarioDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Producto editado con exito",
                Data = inventarioDto
            };
        }

        //eliminar producto por id
        public async Task<ResponseDto<InventarioDto>> EliminarProducto(Guid id)
        {
            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario == null)
            {
                return new ResponseDto<InventarioDto>
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Producto no encontrado",
                    Data = null
                };
            }

            _context.Inventario.Remove(inventario);
            await _context.SaveChangesAsync();

            var inventarioDto = _mapper.Map<InventarioDto>(inventario);

            return new ResponseDto<InventarioDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Producto eliminado con exito",
                Data = inventarioDto
            };
        }
    }
}
