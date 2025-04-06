using FBackend.Services.Interfaces;
using ApiCitaOdon.Data;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FBackend.Models;
using FBackend.Models.DTOs;
using FBackend.Models.DTOs.PedidiosDtos;
using FBackend.Models.DTOs.PedidosCreateDtos;
using Microsoft.EntityFrameworkCore;
using Npgsql.BackendMessages;
using System.Security.Principal;
using AutoMapper;

public class ProductoService : IProductoService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly Cloudinary _cloudinary;

    public ProductoService(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        var cloudinarySettings = new Account(
            configuration["CloudinaryURL:CloudName"] ?? "",
            configuration["CloudinaryURL:ApiKey"] ?? "",
            configuration["CloudinaryURL:ApiSecret"] ?? ""
        );

        _cloudinary = new Cloudinary(cloudinarySettings);
    }

    public async Task<ResponseDto<ProductoDto>> CrearP(ProductoCreateDto model)
    {
        try
        {
            if (model.File == null || model.File.Length == 0)
            {
                return new ResponseDto<ProductoDto>
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
                return new ResponseDto<ProductoDto>
                {
                    Status = false,
                    StatusCode = 500,
                    Message = $"Error al subir la imagen: {uploadResult.Error.Message}"
                };
            }

            var producto = new Producto
            {
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Precio = model.Precio,
                Categoria = model.Categoria,
                ImagenUrl = uploadResult.SecureUrl?.ToString() ?? "",
                Stock = model.Stock,
                ProveedorId = model.ProveedorId
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var productoDto = _mapper.Map<ProductoDto>(producto);

            return new ResponseDto<ProductoDto>
            {
                Status = true,
                StatusCode = 201,
                Message = "Producto creado exitosamente",
                Data = productoDto
            };
        }
        catch (Exception e)
        {
            return new ResponseDto<ProductoDto>
            {
                Status = false,
                StatusCode = 500,
                Message = $"Error al guardar el producto: {e.InnerException?.Message ?? e.Message}"
            };
        }
    }
    //este es el valido

    public async Task<ResponseDto<IEnumerable<ProductoDto>>> ObtenerTodos()
    {
        var productos = await _context.Productos.ToListAsync();
        var productosDto = _mapper.Map<IEnumerable<ProductoDto>>(productos);

        return new ResponseDto<IEnumerable<ProductoDto>>
        {
            Status = true,
            StatusCode = 200,
            Message = "Lista de productos obtenida correctamente",
            Data = productosDto
        };
    }

    public async Task<ResponseDto<ProductoDto>> ObtenerPorId(Guid id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return new ResponseDto<ProductoDto>
            {
                Status = false,
                StatusCode = 404,
                Message = "Producto no encontrado"
            };
        }

        var productoDto = _mapper.Map<ProductoDto>(producto);
        return new ResponseDto<ProductoDto>
        {
            Status = true,
            StatusCode = 200,
            Message = "Producto obtenido correctamente",
            Data = productoDto
        };
    }
    public async Task<ResponseDto<ProductoDto>> Actualizar(Guid id, ProductoUpdateDto model)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return new ResponseDto<ProductoDto>
            {
                Status = false,
                StatusCode = 404,
                Message = "Producto no encontrado"
            };
        }

        producto.Nombre = model.Nombre;
        producto.Descripcion = model.Descripcion;
        producto.Precio = model.Precio;
        producto.Categoria = model.Categoria;
        producto.Stock = model.Stock;

        await _context.SaveChangesAsync();

        var productoDto = _mapper.Map<ProductoDto>(producto);

        return new ResponseDto<ProductoDto>
        {
            Status = true,
            StatusCode = 200,
            Message = "Producto actualizado correctamente",
            Data = productoDto
        };
    }

    public async Task<ResponseDto<bool>> Eliminar(Guid id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null)
        {
            return new ResponseDto<bool>
            {
                Status = false,
                StatusCode = 404,
                Message = "Producto no encontrado"
            };
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        return new ResponseDto<bool>
        {
            Status = true,
            StatusCode = 200,
            Message = "Producto eliminado correctamente",
            Data = true
        };
    }
}
