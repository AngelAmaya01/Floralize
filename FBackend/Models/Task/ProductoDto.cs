using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.DTOs.PedidiosDtos
{
    public class ProductoDto
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public decimal Precio { get; set; }

        public string Categoria { get; set; }

        public string ImagenUrl { get; set; }

        public int Stock { get; set; }

        public Guid ProveedorId { get; set; }

        //public virtual Proveedores Proveedores { get; set; }
    }
}
