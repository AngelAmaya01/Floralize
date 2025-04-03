using ApiCitaOdon.Data;
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

        public PersonalizadoService(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
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
                    FotoReferenciaURL = uploadResult.SecureUrl.AbsoluteUri,
                    UserId = model.UserId

                };

                _context.Personalizados.Add(personalizado);
                await _context.SaveChangesAsync();

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
                var personalizados = await _context.Personalizados.ToListAsync();
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
