using AutoMapper;
using FBackend.Models;
using FBackend.Models.DTOs.PedidiosDtos;
using FBackend.Models.DTOs.PedidosCreateDtos;
using FBackend.Models.Task;
using FBackend.Models.ValidationsDto;

namespace FBackend.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            MapsForTasks();
        }

        public void MapsForTasks()
        {
            CreateMap<ProveedorCreateDto, Proveedores>();
            CreateMap<Proveedores, ProveedorDto>();
            CreateMap<ProductoCreateDto, Producto>();
            CreateMap<Producto, ProductoDto>();
            
        }
    }
}
