using ApiCitaOdon.Data;
using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Models.DTOs;
using FBackend.Models.Task;
using FBackend.Models;
using FBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ApiCitaOdon.Services.Interfaces;
using FluentEmail.Core;

namespace FBackend.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public PedidoService(ApplicationDbContext context, IMapper mapper, IEmailService emailService) 
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
        }

        //crear pedido
        public async Task<ResponseDto<PedidosDto>> CrearPedido(PedidoCreateDto model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cliente = await _context.Users.FindAsync(model.ClienteId);
                if (cliente == null)
                {
                    return new ResponseDto<PedidosDto>
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = "Cliente no encontrado"
                    };
                }

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
                    //verificar stock disponible
                    if (producto.Stock < item.Cantidad)
                        return new ResponseDto<PedidosDto>
                        {
                            Status = false,
                            StatusCode = 400,
                            Message = $"Stock insuficiente para el producto {producto.Nombre}"
                        };
                    //actualizar stock
                    producto.Stock -= item.Cantidad;

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

                // Enviar correo de confirmación
                await _emailService.SendOrderConfirmationAsync(pedido.Id, cliente.Email);

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

        public async Task<ResponseDto<PedidosDto>> ActualizarEstado(Guid id, string nuevoEstado)
        {
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Detalles)
                        .ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                {
                    return new ResponseDto<PedidosDto>
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = "Pedido no encontrado"
                    };
                }

                var estadoAnterior = pedido.Estado;
                pedido.Estado = nuevoEstado;

                await _context.SaveChangesAsync();

                // Enviar correo si el estado cambió a "Completado"
                if (nuevoEstado == "Completado" && estadoAnterior != "Completado")
                {
                    await _emailService.SendOrderReadyEmailAsync(pedido);
                }

                var pedidoDto = new PedidosDto
                {
                    Id = pedido.Id,
                    Estado = pedido.Estado,
                    FechaPedido = pedido.FechaPedido,
                    Total = pedido.Total,
                    // otras propiedades...
                    Detalles = pedido.Detalles?.Select(d => new DetallePedidoDto
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId,
                        ProductoNombre = d.Producto?.Nombre,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    }).ToList()
                };

                return new ResponseDto<PedidosDto>
                {
                    Status = true,
                    StatusCode = 200,
                    Message = $"Estado del pedido actualizado a {nuevoEstado}",
                    Data = pedidoDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<PedidosDto>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al actualizar estado: {e.Message}"
                };
            }
        }

        //obtener todos los pedidos
        public async Task<ResponseDto<IEnumerable<PedidosDto>>> ObtenerTodos()
        {
            var pedidos = await _context.Pedidos
             .Include(p => p.Cliente)
            .Select(p => new PedidosDto
            {
                Id = p.Id,
                FechaPedido = p.FechaPedido,
                Estado = p.Estado,
                Total = p.Total,
                ClienteNombre = p.Cliente.FirstName,
                Direccion = p.Cliente.Address,
            }).ToListAsync();

            var pedidosDto = _mapper.Map<List<PedidosDto>>(pedidos);

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
                    .Include(p => p.Cliente)  // Asegúrate de incluir el cliente
                    .Include(p => p.Detalles)
                        .ThenInclude(d => d.Producto)  // Incluir los productos relacionados
                    .Select(p => new PedidosDto
                    {
                        Id = p.Id,
                        FechaPedido = p.FechaPedido,
                        Estado = p.Estado,
                        Total = p.Total,
                        ClienteNombre = p.Cliente.FirstName + " " + p.Cliente.LastName,
                        Detalles = p.Detalles.Select(d => new DetallePedidoDto
                        {
                            Id = d.Id,
                            ProductoNombre = d.Producto.Nombre,
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario,
                            Total = d.Total
                        }).ToList()
                    })
                    .ToListAsync();

                return new ResponseDto<List<PedidosDto>>
                {
                    Status = true,
                    Message = "Lista de pedidos obtenida correctamente",
                    Data = pedidos
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
