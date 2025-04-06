using ApiCitaOdon.Data;
using ApiCitaOdon.Services;
using ApiCitaOdon.Services.Interfaces;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FBackend.Models;
using FBackend.Models.DTOs;
using FBackend.Models.DTOs.PedidiosDtos;
using FBackend.Models.Task;
using FBackend.Models.ValidationsDto;
using FBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FBackend.Services
{
    public class PersonalizadoService : IPersonalizadoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly IEmailService _emailService;

        public PersonalizadoService(ApplicationDbContext context, 
            IConfiguration configuration, 
            IMapper mapper,
            IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _emailService = emailService;
            var cloudinarySettings = new Account(
                configuration["CloudinaryURL:CloudName"] ?? "",
                configuration["CloudinaryURL:ApiKey"] ?? "",
                configuration["CloudinaryURL:ApiSecret"] ?? ""
            );
            _cloudinary = new Cloudinary(cloudinarySettings);
        }

        public async Task<ResponseDto<PersonalizadoDto>> CrearPedidoPersonalizado(PersonalizadoCreateDto model)
        {
            try
            {
                var cliente = await _context.Users.FindAsync(model.UserId);
                if (cliente == null)
                {
                    return new ResponseDto<PersonalizadoDto>
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = "Cliente no encontrado"
                    };
                }

                if (model.File == null || model.File.Length == 0)
                {
                    return new ResponseDto<PersonalizadoDto>
                    {
                        Status = false,
                        StatusCode = 400,
                        Message = "El archivo de imagen es inválido"
                    };
                }

                using var stream = model.File.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(model.File.FileName, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    return new ResponseDto<PersonalizadoDto>
                    {
                        Status = false,
                        StatusCode = 500,
                        Message = $"Error al subir la imagen: {uploadResult.Error.Message}"
                    };
                }

                var personalizado = new Personalizado
                {
                    TipoFlor = model.TipoFlor,
                    Cantidad = model.Cantidad,
                    IncluirPresente = model.IncluirPresente,
                    IncluirBase = model.IncluirBase,
                    TipoPresente = model.TipoPresente,
                    TipoBase = model.TipoBase,
                    Estado = "Pendiente",
                    FotoReferenciaURL = uploadResult.SecureUrl.AbsoluteUri,
                    UserId = model.UserId
                };

                _context.Personalizados.Add(personalizado);
                await _context.SaveChangesAsync();

                // Enviar correo de confirmación
                await _emailService.SendCustomOrderConfirmationAsync(personalizado.Id, cliente.Email);

                var personalizadoDto = _mapper.Map<PersonalizadoDto>(personalizado);

                return new ResponseDto<PersonalizadoDto>
                {
                    Status = true,
                    StatusCode = 201,
                    Message = "Producto Personalizado creado exitosamente",
                    Data = personalizadoDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<PersonalizadoDto>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al guardar el producto: {e.InnerException?.Message ?? e.Message}"
                };
            }
        }

        //obtener todos los pedidos personalizados por id de usuario
        public async Task<ResponseDto<List<PersonalizadoDto>>> ObtenerPedidosPorCliente(string clienteId)
        {
            try
            {
                if (!Guid.TryParse(clienteId, out Guid parsedClienteId))
                {
                    return new ResponseDto<List<PersonalizadoDto>>
                    {
                        Status = false,
                        StatusCode = 400,
                        Message = "Formato de ID de cliente inválido"
                    };
                }

                var pedidos = await _context.Personalizados
                    .Where(p => p.UserId == parsedClienteId) // ✅ Cambio aquí
                    .ToListAsync();

                var pedidosDto = _mapper.Map<List<PersonalizadoDto>>(pedidos);

                return new ResponseDto<List<PersonalizadoDto>>
                {
                    Status = true,
                    StatusCode = 200,
                    Message = "Lista de pedidos personalizados",
                    Data = pedidosDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<List<PersonalizadoDto>>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al obtener los pedidos personalizados: {e.InnerException?.Message ?? e.Message}"
                };
            }
        }

        //obtener un pedido personalizado por id
        public async Task<ResponseDto<PersonalizadoDto>> ObtenerPedidoPersonalizado(Guid id)
        {
            try
            {
                var personalizado = await _context.Personalizados.FirstOrDefaultAsync(x => x.Id == id);

                if (personalizado == null)
                {
                    return new ResponseDto<PersonalizadoDto>
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = "Pedido personalizado no encontrado"
                    };
                }

                var personalizadoDto = _mapper.Map<PersonalizadoDto>(personalizado);

                return new ResponseDto<PersonalizadoDto>
                {
                    Status = true,
                    StatusCode = 200,
                    Message = "Pedido personalizado encontrado",
                    Data = personalizadoDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<PersonalizadoDto>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al obtener el pedido personalizado: {e.InnerException?.Message ?? e.Message}"
                };
            }
        }

        //obtener todos los pedidos personalizados
        public async Task<ResponseDto<List<PersonalizadoDto>>> ObtenerPedidosPersonalizados()
        {
            try
            {
                var personalizados = await _context.Personalizados
                .Include(p => p.ClienteId)
               .Select(p => new PersonalizadoDto
               {
                   Id = p.Id,
                   nombreCliente = p.ClienteId.FirstName,
                   Direccion = p.ClienteId.Address,
                   Telefono = p.ClienteId.PhoneNumber,
                   TipoFlor = p.TipoFlor,
                   Cantidad = p.Cantidad,
                   IncluirPresente = p.IncluirPresente,
                   IncluirBase = p.IncluirBase,
                   TipoPresente = p.TipoPresente,
                   TipoBase = p.TipoBase,
                   Estado = p.Estado,
                   FotoReferenciaURL = p.FotoReferenciaURL,
                   UserId = p.UserId
               }).ToListAsync();
                var personalizadosDto = _mapper.Map<List<PersonalizadoDto>>(personalizados);

                return new ResponseDto<List<PersonalizadoDto>>
                {
                    Status = true,
                    StatusCode = 200,
                    Message = "Lista de pedidos personalizados",
                    Data = personalizadosDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<List<PersonalizadoDto>>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al obtener los pedidos personalizados: {e.InnerException?.Message ?? e.Message}"
                };
            }
        }

        //editar un pedido personalizado
        public async Task<ResponseDto<PersonalizadoDto>> EditarPedidoPersonalizado(Guid id, PersonalizadoCreateDto model)
        {
            try
            {
                var personalizado = await _context.Personalizados.FirstOrDefaultAsync(x => x.Id == id);

                if (personalizado == null)
                {
                    return new ResponseDto<PersonalizadoDto>
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = "Pedido personalizado no encontrado"
                    };
                }

                if (model.File != null)
                {
                    using var stream = model.File.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(model.File.FileName, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        return new ResponseDto<PersonalizadoDto>
                        {
                            Status = false,
                            StatusCode = 500,
                            Message = $"Error al subir la imagen: {uploadResult.Error.Message}"
                        };
                    }

                    personalizado.FotoReferenciaURL = uploadResult.SecureUrl.AbsoluteUri;
                }

                personalizado.TipoFlor = model.TipoFlor;
                personalizado.Cantidad = model.Cantidad;
                personalizado.IncluirPresente = model.IncluirPresente;
                personalizado.IncluirBase = model.IncluirBase;

                _context.Personalizados.Update(personalizado);
                await _context.SaveChangesAsync();

                var personalizadoDto = _mapper.Map<PersonalizadoDto>(personalizado);

                return new ResponseDto<PersonalizadoDto>
                {
                    Status = true,
                    StatusCode = 200,
                    Message = "Pedido personalizado actualizado exitosamente",
                    Data = personalizadoDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<PersonalizadoDto>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al actualizar el pedido personalizado: {e.InnerException?.Message ?? e.Message}"
                };
            }
        }

        public async Task<ResponseDto<PersonalizadoDto>> ActualizarEstado(Guid id, string nuevoEstado)
        {
            try
            {
                var pedido = await _context.Personalizados
                    .Include(p => p.ClienteId)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                {
                    return new ResponseDto<PersonalizadoDto>
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = "Pedido no encontrado"
                    };
                }

                // Validar y normalizar el estado
                nuevoEstado = nuevoEstado?.Trim();
                var estadosValidos = new[] { "Pendiente", "En Proceso", "Completado", "Cancelado" };

                if (string.IsNullOrEmpty(nuevoEstado) || !estadosValidos.Contains(nuevoEstado))
                {
                    return new ResponseDto<PersonalizadoDto>
                    {
                        Status = false,
                        StatusCode = 400,
                        Message = "Estado no válido. Use: Pendiente, En Proceso, Completado o Cancelado"
                    };
                }

                var estadoAnterior = pedido.Estado;
                pedido.Estado = nuevoEstado;
                pedido.FechaPedido = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Enviar correo si el estado cambió a "Completado"
                if (nuevoEstado == "Completado" && estadoAnterior != "Completado" && pedido.ClienteId != null)
                {
                    await _emailService.SendCustomOrderConfirmationAsync(pedido.Id, pedido.ClienteId.Email);
                }

                var pedidoDto = new PersonalizadoDto
                {
                    Id = pedido.Id,
                    TipoFlor = pedido.TipoFlor,
                    nombreCliente = pedido.ClienteId?.FirstName + " " + pedido.ClienteId?.LastName,
                    Direccion = pedido.ClienteId?.Address,
                    Telefono = pedido.ClienteId?.PhoneNumber,
                    Cantidad = pedido.Cantidad,
                    IncluirPresente = pedido.IncluirPresente,
                    IncluirBase = pedido.IncluirBase,
                    TipoPresente = pedido.TipoPresente,
                    TipoBase = pedido.TipoBase,
                    Estado = pedido.Estado,
                    FotoReferenciaURL = pedido.FotoReferenciaURL,
                    UserId = pedido.UserId,
                    FechaPedido = pedido.FechaPedido
                };

                return new ResponseDto<PersonalizadoDto>
                {
                    Status = true,
                    StatusCode = 200,
                    Message = $"Estado actualizado a {nuevoEstado}",
                    Data = pedidoDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<PersonalizadoDto>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al actualizar estado: {e.Message}"
                };
            }
        }

        //eliminar un pedido personalizado
        public async Task<ResponseDto<PersonalizadoDto>> EliminarPedidoPersonalizado(Guid id)
        {
            try
            {
                var personalizado = await _context.Personalizados.FirstOrDefaultAsync(x => x.Id == id);

                if (personalizado == null)
                {
                    return new ResponseDto<PersonalizadoDto>
                    {
                        Status = false,
                        StatusCode = 404,
                        Message = "Pedido personalizado no encontrado"
                    };
                }

                _context.Personalizados.Remove(personalizado);
                await _context.SaveChangesAsync();

                var personalizadoDto = _mapper.Map<PersonalizadoDto>(personalizado);

                return new ResponseDto<PersonalizadoDto>
                {
                    Status = true,
                    StatusCode = 200,
                    Message = "Pedido personalizado eliminado exitosamente",
                    Data = personalizadoDto
                };
            }
            catch (Exception e)
            {
                return new ResponseDto<PersonalizadoDto>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al eliminar el pedido personalizado: {e.InnerException?.Message ?? e.Message}"
                };
            }
        }
    }
}
