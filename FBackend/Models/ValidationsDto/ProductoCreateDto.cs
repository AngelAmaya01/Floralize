using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs.PedidosCreateDtos
{
    public class ProductoCreateDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no debe exceder los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Precio { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "La categoría no debe exceder los 50 caracteres.")]
        public string Categoria { get; set; }

        public IFormFile File { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser un número positivo.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El ID del proveedor es obligatorio.")]
        public Guid ProveedorId { get; set; }
    }
}
