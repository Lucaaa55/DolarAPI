using AngleSharp;
using AngleSharp.Dom;
using Dolarium.Models;
using static System.Net.WebRequestMethods;

namespace Dolarium.Services
{
    public class BancoService
    {
        private static IBrowsingContext context;
        private List<Banco> bancos;
        private List<Dolar> dolares;

        private const string BANCO_CIUDAD_SELECTOR_COMPRA = "span#moneda_dolar_compra";
        private const string BANCO_CIUDAD_SELECTOR_VENTA = "span#moneda_dolar_venta";

        public BancoService()
        {
            var config = Configuration.Default.WithDefaultLoader();
            context = BrowsingContext.New(config);

            dolares = new List<Dolar>();
            bancos = new List<Banco>
            { 
                new Banco { Name = "Banco Nación", URL = "https://www.bna.com.ar/Personas", Selector = "tbody > tr > td" },
                // new Banco { Name = "Banco Ciudad", URL = "https://bancociudad.com.ar/institucional/", Selector = "" },
                new Banco { Name = "Banco Provincia", URL = "https://www.bancoprovincia.com.ar/mvc/productos/inversiones/dolares_bip/dolares_bip_info_gral", Selector = "div.cyvDolar_inc b > div.w3-col" },
                new Banco { Name = "Banco BBVA", URL = "https://www.bbva.com.ar/personas/productos/inversiones/cotizacion-moneda-extranjera.html", Selector = "table.tabla > tbody > tr > td" }
            };
        }

        public async Task<List<Dolar>> GetDolaresBancosAsync()
        {
            bancos.ForEach(async b =>
            {
                var document = await context.OpenAsync(b.URL);
                ExtractPreciosPorBancoFromDocument(document, b);
            });

            Console.WriteLine("Last: " + dolares.Count);

            return dolares;
        }

        private List<Dolar> ExtractPreciosPorBancoFromDocument(IDocument document, Banco banco)
        {
            /* if (banco.Name == "Banco Ciudad")
            {
                var precioCompra = ExtractElements(document, BANCO_CIUDAD_SELECTOR_COMPRA);
                var precioVenta = ExtractElements(document, BANCO_CIUDAD_SELECTOR_VENTA);
                
                Console.WriteLine($"Banco Ciudad - Compra: {precioCompra.FirstOrDefault()} - Venta: {precioVenta.FirstOrDefault()}");

                var precios = new List<string>();
                precios.Add(precioCompra.FirstOrDefault());
                precios.Add(precioVenta.FirstOrDefault());

                return BuildDolaresBancosList(banco, precios);
            }
            else
            {
                var precios = ExtractElements(document, banco.Selector);

                return BuildDolaresBancosList(banco, precios);
            } */

            var precios = ExtractElements(document, banco.Selector);

            return BuildDolaresBancosList(banco, precios);
        }

        private List<Dolar> BuildDolaresBancosList(Banco banco, List<string> precios)
        {
            var spread = precios.Count > 1 ? ParsePrice(precios[1]) - ParsePrice(precios[0]) : 0;

            var dolar = new Dolar
            {
                Name = banco.Name,
                Buy = precios.Count > 0 ? ParsePrice(precios[0]) : 0,
                Sell = precios.Count > 1 ? ParsePrice(precios[1]) : 0,
                Spread = spread,
                Variation = 0,
                Timestamp = GetCurrentUnixTimestamp(),
            };

            dolares.Add(dolar);
            Console.WriteLine(dolares.Count);
            return dolares;
        }

        private List<string> ExtractElements(IDocument document, string selector)
        {
            return document.QuerySelectorAll(selector)
                .Select(e => e.TextContent.Trim())
                .Where(s => float.TryParse(s, out _))
                .ToList();
        }

        private float ParsePrice(string price)
        {
            Console.WriteLine(price);
            var priceCleaned = price.Replace('$', ' ').Trim();

            if (float.TryParse(price, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            return 0;
        }

        private int GetCurrentUnixTimestamp()
        {
            return Math.Abs((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
    }
}
