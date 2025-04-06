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
        public async Task<IActionResult> ActualizarEstado(Guid id, [FromBody] string estado)
        {
            try
            {
                // Validación adicional del estado
                if (string.IsNullOrWhiteSpace(estado))
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "El estado no puede estar vacío"
                    });
                }

                var response = await _personalizadoService.ActualizarEstado(id, estado);

                if (!response.Status)
                {
                    return StatusCode(response.StatusCode, response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = $"Error interno: {ex.Message}"
                });
            }
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