using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs.PedidosDtos
{
    public class InventarioCreateDtos
    {
        [Required]
        public Guid ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int Cantidad { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La ubicación no debe exceder los 100 caracteres.")]
        public string Ubicacion { get; set; }

        [Required]
        public DateTime FechaActualizacion { get; set; }
    }
}
