using Dolarium.Services;
using Microsoft.AspNetCore.Mvc;
using Dolarium.DTOs;
using Dolarium.Controllers;
using Dolarium.Interfaces;

namespace Dolarium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DolaresController(
        IDolarService dolarService
    ) : ControllerBase
    {
        // GET: api/<DolaresController>

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var names = await dolarService.GetDolarNamesAsync();

                return Ok(names);
            }
            catch (HttpRequestException e)
            {
                return StatusCode(503, $"Error al conectar con el sitio web: {e.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error al obtener los nombres de los dólares: {e.Message}");
            }
        }

        [HttpGet("prices")]
        public async Task<IActionResult> Prices(/* [FromBody] KeysDto dto*/)
        {
            try
            {
                /* var result = await _keyService.IsKeyValidAsync(dto.Key);

                if (!result)
                {
                    return Unauthorized(new { error = "Clave inválida" });
                }

                await _keyService.IncrementKeyUsageAsync(dto.Key); */
                var dolares = await dolarService.GetDolarPricesAsync();
                return Ok(dolares);
            }
            catch (HttpRequestException e)
            {
                return StatusCode(503, $"Error al conectar con el sitio web: {e.Message}");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error al obtener los nombres de los dólares: {e.Message}");
            }
        }

        
    }
}
