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
    public class ProveedorService : IProveedorService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ProveedorService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseDto<ProveedorDto>> CreateProveedor(ProveedorCreateDto proveedorCreateDto)
        {
            var proveedor = _mapper.Map<Proveedores>(proveedorCreateDto);
            await _context.Proveedores.AddAsync(proveedor);
            await _context.SaveChangesAsync();
            var proveedorDto = _mapper.Map<ProveedorDto>(proveedor);
            return new ResponseDto<ProveedorDto>
            {
                Status = true,
                StatusCode = 201,
                Message = "Proveedor creado con éxito",
                Data = proveedorDto
            };
        }

        public async Task<ResponseDto<ProveedorDto>> GetProveedorById(Guid id)
        {
            var proveedor = await _context.Proveedores.FirstOrDefaultAsync(x => x.Id == id);
            if (proveedor == null)
            {
                return new ResponseDto<ProveedorDto>
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Proveedor no encontrado",
                    Data = null
                };
            }
            var proveedorDto = _mapper.Map<ProveedorDto>(proveedor);
            return new ResponseDto<ProveedorDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Proveedor encontrado",
                Data = proveedorDto
            };
        }

        //traer todos los proveedores
        public async Task<ResponseDto<List<ProveedorDto>>> GetProveedores()
        {
            var proveedores = await _context.Proveedores.ToListAsync();
            var proveedoresDto = _mapper.Map<List<ProveedorDto>>(proveedores);
            return new ResponseDto<List<ProveedorDto>>
            {
                Status = true,
                StatusCode = 200,
                Message = "Proveedores encontrados",
                Data = proveedoresDto
            };
        }

        //eliminar proveedor
        public async Task<ResponseDto<ProveedorDto>> DeleteProveedor(Guid id)
        {
            var proveedor = await _context.Proveedores.FirstOrDefaultAsync(x => x.Id == id);
            if (proveedor == null)
            {
                return new ResponseDto<ProveedorDto>
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Proveedor no encontrado",
                    Data = null
                };
            }
            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();
            var proveedorDto = _mapper.Map<ProveedorDto>(proveedor);
            return new ResponseDto<ProveedorDto>
            {
                Status = true,
                StatusCode = 200,
                Message = "Proveedor eliminado",
                Data = proveedorDto
            };
        }

        
    }
}
