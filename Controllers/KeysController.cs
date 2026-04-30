using Microsoft.AspNetCore.Mvc;
using Dolarium.Services;
using Dolarium.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Dolarium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class KeysController : ControllerBase
    {
        private readonly KeyService _keyService;

        public KeysController(KeyService keyService)
        {
            _keyService = keyService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new string[] { "/create", "/status", "/delete" });
        }

        // GET: api/<KeysController>
        [HttpGet("create")]
        public async Task<IActionResult> CreateAsync()
        {
            var key = await _keyService.CreateKeyAsync(10);

            return Ok(key);
        }

        [HttpGet("status")]
        public async Task<IActionResult> StatusAsync(KeysDto dto)
        {
            var key = await _keyService.GetKeyAsync(dto.Key);

            return Ok(key);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync(KeysDto dto)
        {
            var result = await _keyService.DeleteKeyAsync(dto.Key);

            if (!result)
            {
                return Ok(new string[] { "Key no encontrada" });
            }

            return Ok(new string[] { "Key eliminada" });
        }
    }
}
