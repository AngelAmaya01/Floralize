using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.Task
{
    public class DetallePedidoDto
    {
        public Guid Id { get; set; }

        public Guid PedidoId { get; set; }

        public Guid ProductoId { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }
        public decimal Total { get; set; }

        public virtual Pedidos Pedidos { get; set; }
        public virtual Producto Producto { get; set; }
    }
}
