namespace FBackend.Models.Task
{
    public class PersonalizadoDto
    {
        public Guid Id { get; set; }
        public DateTime FechaPedido { get; set; }
        public string TipoFlor { get; set; }
        public string nombreCliente { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Cantidad { get; set; }
        public string IncluirPresente { get; set; }
        public string IncluirBase { get; set; }
        public string TipoPresente { get; set; }
        public string TipoBase { get; set; }
        public string Estado { get; set; }
        public string FotoReferenciaURL { get; set; }

        public Guid UserId { get; set; }
    }
}
