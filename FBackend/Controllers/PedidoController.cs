using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBackend.Controllers
{
    [Route("api/pedido")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearPedido([FromBody] PedidoCreateDto model)
        {
            if (model == null || model.Detalles == null || !model.Detalles.Any())
            {
                return BadRequest("El pedido debe contener al menos un producto.");
            }

            var response = await _pedidoService.CrearPedido(model);
            return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        


        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var response = await _pedidoService.ObtenerTodos();
            return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(Guid id)
        {
            var response = await _pedidoService.ObtenerPorId(id);
            return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
        }

    }
}
