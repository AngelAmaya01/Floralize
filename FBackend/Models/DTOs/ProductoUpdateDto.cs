namespace FBackend.Models.DTOs
{
    public class ProductoUpdateDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Categoria { get; set; }
        public int Stock { get; set; }
    }
}
