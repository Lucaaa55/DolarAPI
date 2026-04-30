using AngleSharp;
using AngleSharp.Dom;
using Dolarium.Models;
using System.Globalization;

namespace Dolarium.Services
{
    public class BancoService
    {
        private readonly IBrowsingContext _context;
        private readonly List<Banco> _bancos;
        private const int REQUEST_TIMEOUT = 10000;

        public BancoService()
        {
            var config = Configuration.Default
                .WithDefaultLoader();

            _context = BrowsingContext.New(config);
            _bancos = InitializeBancos();
        }

        public async Task<List<Dolar>> GetDolaresBancosAsync()
        {
            var dolares = new List<Dolar>();
            var tasks = _bancos.Select(b => ExtraerDolarDelBancoAsync(b)).ToList();

            try
            {
                var resultados = await Task.WhenAll(tasks);
                dolares = resultados.Where(d => d != null).ToList();

                if (dolares.Count == 0)
                    throw new InvalidOperationException("No se obtuvieron cotizaciones de ningún banco");

                return dolares;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener cotizaciones: {ex.Message}", ex);
            }
        }

        private async Task<Dolar> ExtraerDolarDelBancoAsync(Banco banco)
        {
            var documento = await _context.OpenAsync(banco.URL);
            var precios = ExtraerElementos(documento, banco.Selector);

            if (precios.Count == 0)
            {
                return null;
            }

            return ConstructorDolar(banco, precios);
        }

        private Dolar ConstructorDolar(Banco banco, List<string> precios)
        {
            float compra = precios.Count > 0 ? ParsearPrecio(precios[0]) : 0;
            float venta = precios.Count > 1 ? ParsearPrecio(precios[1]) : compra;

            if (precios.Count == 1 && venta == 0)
            {
                venta = compra;
            }

            float spread = venta - compra;

            var dolar = new Dolar
            {
                Name = banco.Name,
                Buy = compra,
                Sell = venta,
                Spread = spread,
                Variation = 0,
                Timestamp = ObtenerTimestampActual()
            };

            return dolar;
        }

        private List<string> ExtraerElementos(IDocument documento, string selector)
        {
            return documento.QuerySelectorAll(selector)
                .Select(e => e.TextContent?.Trim() ?? "")
                .Where(texto => !string.IsNullOrEmpty(texto))
                .Where(texto => EsPrecioValido(texto))
                .ToList();
        }

        private bool EsPrecioValido(string texto)
        {
            return float.TryParse(
                texto.Replace("$", "").Replace(",", ".").Trim(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out _
            );
        }

        private float ParsearPrecio(string precio)
        {
            if (string.IsNullOrWhiteSpace(precio))
            {
                return 0;
            }

            string precioLimpio = precio
                .Replace("$", "")
                .Replace(",", ".")
                .Replace(" ", "")
                .Trim();

            if (float.TryParse(precioLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out float resultado))
            {
                return resultado;
            }

            return 0;
        }

        private int ObtenerTimestampActual()
        {
            return (int)Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        private List<Banco> InitializeBancos()
        {
            return new List<Banco>
            {
                new Banco
                {
                    Name = "Banco Nación",
                    Short = "BNA",
                    URL = "https://www.bna.com.ar/Personas",
                    Selector = "tbody > tr > td"
                },
                new Banco
                {
                    Name = "Banco Provincia",
                    Short = "BPBA",
                    URL = "https://www.bancoprovincia.com.ar/mvc/productos/inversiones/dolares_bip/dolares_bip_info_gral",
                    Selector = "b > div.w3-col"
                },
                new Banco
                {
                    Name = "Banco BBVA",
                    Short = "BBVA",
                    URL = "https://www.bbva.com.ar/personas/productos/inversiones/cotizacion-moneda-extranjera.html",
                    Selector = "table.tabla tbody tr td"
                }
            };
        }
    }
}