using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs.PedidosDtos
{
    public class DetallePedidosCreateDtos
    {
        [Required]
        public Guid PedidoId { get; set; }

        [Required]
        public Guid ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que 0.")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor que 0.")]
        public decimal Total { get; set; }
    }
}
