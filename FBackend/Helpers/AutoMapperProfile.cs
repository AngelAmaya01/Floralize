using AutoMapper;
using FBackend.Models;
using FBackend.Models.DTOs.PedidiosDtos;
using FBackend.Models.DTOs.PedidosCreateDtos;
using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Models.Task;
using FBackend.Models.ValidationsDto;

namespace FBackend.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            MapsForTasks();
            // Mapea Pedido a PedidoDto
            CreateMap<Pedido, PedidosDto>()
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.Cliente))
                .ForMember(dest => dest.Detalles, opt => opt.MapFrom(src => src.Detalles));

            // Mapea DetallePedido a DetallePedidoDto
            CreateMap<DetallePedido, DetallePedidoDto>()
                .ForMember(dest => dest.ProductoId, opt => opt.MapFrom(src => src.Producto.Nombre));
        }

        public void MapsForTasks()
        {
            CreateMap<ProveedorCreateDto, Proveedores>();
            CreateMap<Proveedores, ProveedorDto>();
            CreateMap<ProductoCreateDto, Producto>();
            CreateMap<Producto, ProductoDto>();
            CreateMap<DetallePedidoCreateDto, DetallePedido>();
            CreateMap<DetallePedido, PedidosDto>();
            CreateMap<PedidoCreateDto, Pedido>();
            CreateMap<Pedido, PedidosDto>();
            CreateMap<PersonalizadoCreateDto, Personalizado>();
            CreateMap<Personalizado, PersonalizadoDto>();
            
        }
    }
}
