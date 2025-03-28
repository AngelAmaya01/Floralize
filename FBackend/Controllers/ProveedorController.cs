using FBackend.Models.ValidationsDto;
using FBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FBackend.Controllers
{
    [Route("api/proveedor")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedorController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProveedor([FromBody] ProveedorCreateDto proveedorCreateDto)
        {
            var response = await _proveedorService.CreateProveedor(proveedorCreateDto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProveedorById(Guid id)
        {
            var response = await _proveedorService.GetProveedorById(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetProveedores()
        {
            var response = await _proveedorService.GetProveedores();
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(Guid id)
        {
            var response = await _proveedorService.DeleteProveedor(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
