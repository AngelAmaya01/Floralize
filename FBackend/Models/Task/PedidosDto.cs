using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.Task
{
    public class PedidosDto
    {
        public Guid Id { get; set; }

        public Guid ClienteId { get; set; }

        public DateTime FechaPedido { get; set; }

        public string Estado { get; set; }

        public decimal Total { get; set; }

        public virtual User User { get; set; }
        public virtual List<DetallePedidoDto> Detalles { get; set; } = new List<DetallePedidoDto>();
    }
}
