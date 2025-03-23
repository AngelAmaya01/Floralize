using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.Task
{
    public class InventarioDto
    {
        public Guid Id { get; set; }

        public int ProductoId { get; set; }

        public int Cantidad { get; set; }

        public string Ubicacion { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public virtual Producto Producto { get; set; }
    }
}
