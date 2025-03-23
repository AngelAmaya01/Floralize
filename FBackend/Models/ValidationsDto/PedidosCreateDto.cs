using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs.PedidosDtos
{
    public class PedidosCreateDto
    {
        [Required]
        public Guid ClienteId { get; set; }

        [Required]
        public DateTime FechaPedido { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "El estado no debe exceder los 20 caracteres.")]
        public string Estado { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser un valor positivo.")]
        public decimal Total { get; set; }

        public List<DetallePedidosCreateDtos> Detalles { get; set; } = new List<DetallePedidosCreateDtos>();
    }
}
