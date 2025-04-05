using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FBackend.Models.Task
{
    public class PedidosDto
    {
        
        public Guid Id { get; set; }
        public string ClienteNombre { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }
        public List<DetallePedidoDto> Detalles { get; set; }
    }
}
