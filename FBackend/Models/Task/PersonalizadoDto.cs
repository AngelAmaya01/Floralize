namespace FBackend.Models.Task
{
    public class PersonalizadoDto
    {
        public Guid Id { get; set; }

        public string TipoFlor { get; set; }
        public string Cantidad { get; set; }
        public string IncluirPresente { get; set; }
        public string IncluirBase { get; set; }
        public string TipoPresente { get; set; }
        public string TipoBase { get; set; }
        public string FotoReferenciaURL { get; set; }

        public Guid UserId { get; set; }
    }
}
