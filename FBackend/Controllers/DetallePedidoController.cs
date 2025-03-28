using FBackend.Models.DTOs.PedidosDtos;
using FBackend.Services;
using FBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBackend.Controllers
{
    [Route("api/Detalle")]
    [ApiController]
    public class DetallePedidoController : ControllerBase
    {
        private readonly IDetallePedidoService _detallePedidoService;

        public DetallePedidoController(IDetallePedidoService detallePedidoService)
        {
            _detallePedidoService = detallePedidoService;
        }

        [HttpGet("{pedidoId}")]
        public async Task<IActionResult> ObtenerDetallesPorPedido(Guid pedidoId)
        {
            try
            {
                var response = await _detallePedidoService.ObtenerDetallesPorPedido(pedidoId);

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
                return StatusCode(500, new { status = false, message = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPedidos()
        {
            try
            {
                var response = await _detallePedidoService.ObtenerDetallesPedidos();

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
                return StatusCode(500, new { status = false, message = e.Message });
            }
        }

    }
}
