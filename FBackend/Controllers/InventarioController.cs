using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;
        public InventarioController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        //Agregar producto
        [HttpPost]
        public async Task<IActionResult> AgregarProducto([FromBody] InventarioCreateDtos model)
        {
            var response = await _inventarioService.agregarProducto(model);
            if (response.Status)
            {
                return StatusCode(201, response);
            }
            return BadRequest(response);
        }

        // Obtener todos los productos
        [HttpGet]
        public async Task<IActionResult> ObtenerProductos()
        {
            var response = await _inventarioService.ObtenerProductos();
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // editar producto por id
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProducto(Guid id, [FromBody] InventarioCreateDtos model)
        {
            var response = await _inventarioService.EditarProducto(id, model);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        // eliminar producto por id
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(Guid id)
        {
            var response = await _inventarioService.EliminarProducto(id);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
