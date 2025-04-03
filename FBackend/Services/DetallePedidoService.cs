using ApiCitaOdon.Data;
using AutoMapper;
using FBackend.Models;
using FBackend.Models.DTOs;
using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Models.Task;
using FBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FBackend.Services
{
    public class DetallePedidoService : IDetallePedidoService

    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DetallePedidoService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<ResponseDto<List<DetallePedidoDto>>> ObtenerDetallesPorPedido(Guid pedidoId)
        {
            try
            {
                var detalles = await _context.DetallePedidos
                    .Where(d => d.PedidoId == pedidoId)
                    .Include(d => d.Producto) // Incluye la información del producto si es necesario
                    .ToListAsync();

                var detallesDto = _mapper.Map<List<DetallePedidoDto>>(detalles);

                return new ResponseDto<List<DetallePedidoDto>>
                {
                    Status = true,
                    Message = "Detalles del pedido obtenidos correctamente",
                    Data = detallesDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<List<DetallePedidoDto>>
                {
                    Status = false,
                    Message = $"Error al obtener detalles del pedido: {e.Message}",
                    Data = null
                };
            }
        }

        //obtener todos los pedidos con el detalle
        public async Task<ResponseDto<List<PedidosDto>>> ObtenerPedidosDetalles()
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Detalles) // Cargar los detalles del pedido
                    .ThenInclude(d => d.Producto) // Opcional, si quieres incluir el producto
                    .ToListAsync();

                var pedidosDto = _mapper.Map<List<PedidosDto>>(pedidos);

                return new ResponseDto<List<PedidosDto>>
                {
                    Status = true,
                    Message = "Lista de pedidos obtenida correctamente",
                    Data = pedidosDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<List<PedidosDto>>
                {
                    Status = false,
                    Message = $"Error al obtener los pedidos: {e.Message}",
                    Data = null
                };
            }
        }

        public async Task<ResponseDto<List<DetallePedidoDto>>> ObtenerDetallesPedidos()
        {
            try
            {
                var detalles = await _context.DetallePedidos
                    .Include(d => d.Producto) // Incluye la relación con Producto
                    .ToListAsync();

                var detallesDto = detalles.Select(d => new DetallePedidoDto
                {
                    Id = d.Id,
                    PedidoId = d.PedidoId,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Total = d.Total,
                    ProductoNombre = d.Producto.Nombre // Trae el nombre del producto
                }).ToList();

                return new ResponseDto<List<DetallePedidoDto>>
                {
                    Status = true,
                    Message = "Lista de detalles de pedidos obtenida correctamente",
                    Data = detallesDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<List<DetallePedidoDto>>
                {
                    Status = false,
                    Message = $"Error al obtener los detalles de pedidos: {e.Message}",
                    Data = null
                };
            }
        }

        //obtener un detalle por id de usurio
        public async Task<ResponseDto<List<DetallePedidoDto>>> ObtenerDetallesPorCliente(Guid clienteId)
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Where(p => p.ClienteId == clienteId) // Filtrar pedidos del cliente
                    .SelectMany(p => p.Detalles) // Obtener los detalles de esos pedidos
                    .Include(d => d.Producto) // Incluir el producto
                    .ToListAsync();

                var detallesDto = pedidos.Select(d => new DetallePedidoDto
                {
                    Id = d.Id,
                    PedidoId = d.PedidoId,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Total = d.Total,
                    ProductoNombre = d.Producto.Nombre // Agregar nombre del producto
                }).ToList();

                return new ResponseDto<List<DetallePedidoDto>>
                {
                    Status = true,
                    Message = "Detalles de pedido obtenidos correctamente",
                    Data = detallesDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<List<DetallePedidoDto>>
                {
                    Status = false,
                    Message = $"Error al obtener los detalles del pedido: {e.Message}",
                    Data = null
                };
            }
        }



    }
}
