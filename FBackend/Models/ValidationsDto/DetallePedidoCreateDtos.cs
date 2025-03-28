using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs.PedidosDtos
{
    public class DetallePedidoCreateDto
    {
        public Guid ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

}
