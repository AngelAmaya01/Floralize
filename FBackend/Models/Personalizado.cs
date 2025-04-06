using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FBackend.Models
{
    public class Personalizado
    {
        public Guid Id { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;
        public string TipoFlor { get; set; }
        public string Cantidad { get; set; }
        public string IncluirPresente { get; set; }
        public string IncluirBase { get; set; }
        public string TipoPresente { get; set; }
        public string TipoBase { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string FotoReferenciaURL { get; set; }

        [ForeignKey("ClienteId")]
        public Guid UserId { get; set; }
        public virtual User ClienteId { get; set; }
    }
}
