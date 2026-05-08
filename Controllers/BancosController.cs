using Dolarium.DTOs;
using Dolarium.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dolarium.Interfaces;

namespace Dolarium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BancosController(
        IBancoService bancoService
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Bancos(/* [FromBody] KeysDto dto */)
        {
            try
            {
                /* var result = await _keyService.IsKeyValidAsync(dto.Key);

                if (!result)
                {
                    return Unauthorized(new { error = "Clave inválida" });
                }

                await _keyService.IncrementKeyUsageAsync(dto.Key); */
                var dolares = await bancoService.GetDolaresBancosAsync();

                if (!dolares.Any())
                {
                    return NotFound(new
                    {
                        error = "No se obtuvieron cotizaciones de ningún banco"
                    });
                }

                return Ok(new
                {
                    timestamp = DateTime.UtcNow,
                    cantidad = dolares.Count,
                    cotizaciones = dolares
                });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error en operación: {ex.Message}");
                return BadRequest(new { error = ex.Message });
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

        [HttpGet("banco")]
        public async Task<IActionResult> Banco([FromBody] BancoDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest(new { error = "El nombre del banco es requerido" });
                }

                /* var result = await _keyService.IsKeyValidAsync(dto.Key);

                if (!result)
                {
                    return Unauthorized(new { error = "Clave inválida" });
                }

                await _keyService.IncrementKeyUsageAsync(dto.Key); */
                var dolares = await bancoService.GetDolaresBancosAsync();
                var dolarBanco = dolares.FirstOrDefault(d => d.Name.Replace(" ", "").ToLower().Equals(dto.Name.Replace(" ", "").ToLower(), StringComparison.OrdinalIgnoreCase));

                if (dolarBanco == null)
                {
                    return NotFound(new { error = $"No se encontró cotización para {dto.Name}" });
                }

                return Ok(dolarBanco);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
