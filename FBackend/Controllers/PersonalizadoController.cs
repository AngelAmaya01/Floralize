using ApiCitaOdon.Data;
using FBackend.Models.ValidationsDto;
using FBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FBackend.Controllers.PedidoController;

namespace FBackend.Controllers
{
    [Route("api/personalido")]
    [ApiController]
    public class PersonalizadoController : ControllerBase
    {
        private readonly IPersonalizadoService _personalizadoService;
        private readonly ApplicationDbContext _context;

        public PersonalizadoController(IPersonalizadoService personalizadoService,
            ApplicationDbContext context)
        {
            _personalizadoService = personalizadoService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CrearPedidoPersonalizado([FromForm] PersonalizadoCreateDto model)
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
                var response = await _personalizadoService.CrearPedidoPersonalizado(model);

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

        [HttpPut("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] CambiarEstadoDto model)
        {
            // Cambiar de _context.Pedidos a _context.Personalizados
            var pedido = await _context.Personalizados.FindAsync(id);

            if (pedido == null)
            {
                return NotFound(new { status = false, message = "Pedido personalizado no encontrado" });
            }

            pedido.Estado = model.Estado;
            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "Estado actualizado correctamente" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPedidoPersonalizado(Guid id)
        {
            try
            {
                var response = await _personalizadoService.ObtenerPedidoPersonalizado(id);

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

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> ObtenerPedidosPorCliente(Guid clienteId)
        {
            var response = await _personalizadoService.ObtenerPedidosPorCliente(clienteId.ToString()); // ✅ Conversión a string
            return StatusCode(response.StatusCode, response);
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerPedidosPersonalizados()
        {
            try
            {
                var response = await _personalizadoService.ObtenerPedidosPersonalizados();

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

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarPedidoPersonalizado(Guid id, [FromForm] PersonalizadoCreateDto model)
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
                var response = await _personalizadoService.EditarPedidoPersonalizado(id, model);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPedidoPersonalizado(Guid id)
        {
            try
            {
                var response = await _personalizadoService.EliminarPedidoPersonalizado(id);

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
    }
}