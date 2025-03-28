using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs.PedidosDtos
{
    public class PedidoCreateDto
    {
        public Guid ClienteId { get; set; }
        public List<DetallePedidoCreateDto> Detalles { get; set; }
    }

}
