using FBackend.Models.ValidationsDto;
using FBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBackend.Controllers
{
    [Route("api/categoria")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearCategoria([FromBody] CategoriaCreateDto model)
        {
            if (model == null)
            {
                return BadRequest("El modelo de categoria no puede ser nulo");
            }

            var response = await _categoriaService.CrearCategoria(model);
            return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            var response = await _categoriaService.ObtenerCategorias();
            return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCategoria(Guid id, [FromBody] CategoriaCreateDto model)
        {
            if (model == null)
            {
                return BadRequest("El modelo de categoria no puede ser nulo");
            }

            var response = await _categoriaService.EditarCategoria(id, model);
            return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCategoria(Guid id)
        {
            var response = await _categoriaService.EliminarCategoria(id);
            return response.Status ? Ok(response) : StatusCode(response.StatusCode, response);
        }
    }
}
