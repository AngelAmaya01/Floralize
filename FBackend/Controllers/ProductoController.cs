using FBackend.Models.DTOs.PedidiosDtos;
using FBackend.Models.DTOs;
using FBackend.Models.DTOs.PedidosCreateDtos;
using FBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FBackend.Controllers
{
    [Route("api/producto")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromForm] ProductoCreateDto model)
        {
            if (model == null)
            {
                return BadRequest("No se ha enviado ningún dato");
            }

            if (model.File == null)
            {
                return BadRequest("No se ha enviado ninguna imagen");
            }

            try
            {
                var response = await _productoService.CrearP(model);

                if (response.Status)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos()
        {
            var response = await _productoService.ObtenerTodos();
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerProductoPorId(Guid id)
        {
            var response = await _productoService.ObtenerPorId(id);
            return response.Status ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(Guid id, [FromBody] ProductoUpdateDto model)
        {
            if (model == null) return BadRequest("No se ha enviado ningún dato");

            var response = await _productoService.Actualizar(id, model);
            return response.Status ? Ok(response) : NotFound(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(Guid id)
        {
            var response = await _productoService.Eliminar(id);
            return response.Status ? Ok(response) : NotFound(response);
        }


    }
}
