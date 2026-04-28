using AngleSharp;
using AngleSharp.Dom;
using Dolarium.Models;
using Dolarium.Services;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

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
            var dolaresBancos = await _bancoService.GetDolaresBancosAsync();
            Console.WriteLine(dolaresBancos.Count);
            return Ok(dolaresBancos);
        }
    }
}
