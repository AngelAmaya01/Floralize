using ApiCitaOdon.Data;
using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Services;
using FBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBackend.Controllers
{
    [Route("api/pedido")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ApplicationDbContext _context;

        public PedidoController(IPedidoService pedidoService, ApplicationDbContext context)
        {
            _pedidoService = pedidoService;
            _context = context;
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

        [HttpPut("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(Guid id, [FromBody] ActualizarEstadoRequest request)
        {
            try
            {
                var result = await _pedidoService.ActualizarEstado(id, request.Estado);

                if (!result.Status)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = false, Message = "Error interno del servidor" });
            }
        }

        public class ActualizarEstadoRequest
        {
            public string Estado { get; set; }
        }

        public class CambiarEstadoDto
        {
            public string Estado { get; set; }
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
