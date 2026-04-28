using AngleSharp;
using AngleSharp.Dom;
using Dolarium.Models;
using Dolarium.Services;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Dolarium.DTOs;

namespace Dolarium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DolaresController : ControllerBase
    {
        private static DolarService _dolarService;
        private static BancoService _bancoService;

        public DolaresController(DolarService dolarService, BancoService bancoService)
        {
            _dolarService = dolarService;
            _bancoService = bancoService;
        }

        // GET: api/<DolaresController>

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var names = await _dolarService.GetDolarNamesAsync();

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
        public async Task<IActionResult> Prices()
        {
            try
            {
                var dolares = await _dolarService.GetDolarPricesAsync();

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

        [HttpGet("bancos")]
        public async Task<IActionResult> Bancos()
        {
            try
            {
                var dolares = await _bancoService.GetDolaresBancosAsync();

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
                
                var dolares = await _bancoService.GetDolaresBancosAsync();
                var dolarBanco = dolares.FirstOrDefault(d => d.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));

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
