using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs.PedidosDtos
{
    public class InventarioCreateDtos
    {
        [Required]
        [StringLength(150, ErrorMessage = "El nombre no debe exceder los 150 caracteres.")]
        public string Nombre { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int Cantidad { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La ubicación no debe exceder los 100 caracteres.")]
        public string Ubicacion { get; set; }

        [Required(ErrorMessage = "El ID de la categoría es obligatorio.")]
        public Guid CategoriaId { get; set; }
    }
}
