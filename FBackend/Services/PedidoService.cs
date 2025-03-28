using ApiCitaOdon.Data;
using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Models.DTOs;
using FBackend.Models.Task;
using FBackend.Models;
using FBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace FBackend.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PedidoService(ApplicationDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        //crear pedido
        public async Task<ResponseDto<PedidosDto>> CrearPedido(PedidoCreateDto model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pedido = new Pedido
                {
                    ClienteId = model.ClienteId,
                    FechaPedido = DateTime.UtcNow,
                    Estado = "Pendiente",
                    Total = 0
                };

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                decimal totalPedido = 0;

                foreach (var item in model.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if (producto == null)
                    {
                        return new ResponseDto<PedidosDto>
                        {
                            Status = false,
                            StatusCode = 400,
                            Message = $"Producto con ID {item.ProductoId} no encontrado"
                        };
                    }

                    decimal subtotal = item.Cantidad * producto.Precio;
                    totalPedido += subtotal;

                    var detallePedido = new DetallePedido
                    {
                        PedidoId = pedido.Id,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.Precio,
                        Total = subtotal
                    };

                    _context.DetallePedidos.Add(detallePedido);
                }

                pedido.Total = totalPedido;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseDto<PedidosDto>
                {
                    Status = true,
                    StatusCode = 201,
                    Message = "Pedido creado exitosamente",
                    Data = new PedidosDto { Id = pedido.Id, FechaPedido = pedido.FechaPedido, Estado = pedido.Estado, Total = pedido.Total }
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return new ResponseDto<PedidosDto>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al crear pedido: {e.Message}"
                };
            }
        }

        //obtener todos los pedidos

        public async Task<ResponseDto<IEnumerable<PedidosDto>>> ObtenerTodos()
        {
            var pedidos = await _context.Pedidos.ToListAsync();
            var pedidosDto = pedidos.Select(p => new PedidosDto
            {
                Id = p.Id,
                ClienteId = p.ClienteId,
                FechaPedido = p.FechaPedido,
                Estado = p.Estado,
                Total = p.Total
            });

            return new ResponseDto<IEnumerable<PedidosDto>>
            {
                Status = true,
                StatusCode = 200,
                Message = "Lista de pedidos obtenida correctamente",
                Data = pedidosDto
            };
        }

        //obtener pedidos
        public async Task<ResponseDto<List<PedidosDto>>> ObtenerPedidos()
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



        //obtener pedido por id
        public async Task<ResponseDto<PedidosDto>> ObtenerPorId(Guid id)
        {
            var pedido = await _context.Pedidos.Include(p => p.Detalles).FirstOrDefaultAsync(p => p.Id == id);
            if (pedido == null)
            {
                return new ResponseDto<PedidosDto>
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Pedido no encontrado"
                };
            }

            var pedidoDto = new PedidosDto
            {
                Id = pedido.Id,
                ClienteId = pedido.ClienteId,
                FechaPedido = pedido.FechaPedido,
                Estado = pedido.Estado,
                Total = pedido.Total
            };

            return new ResponseDto<PedidosDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Pedido obtenido correctamente",
                Data = pedidoDto
            };
        }
    


    }
}
