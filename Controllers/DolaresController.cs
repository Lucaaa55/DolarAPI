using AngleSharp;
using AngleSharp.Dom;
using Calcu.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace Calcu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DolaresController : ControllerBase
    {
        private static AngleSharp.IConfiguration config = Configuration.Default.WithDefaultLoader();
        private string url = "https://dolarhoy.com/";

        private IBrowsingContext context = BrowsingContext.New(config);

        private string dolarPrecioCompraSelector = "div.compra div.val";
        private string dolarPrecioVentasSelector = "div.venta-wrapper div.val";

        // GET: api/<DolaresController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IDocument document = await context.OpenAsync(url);

            string dolarNames = "a.titleText";
            IHtmlCollection<IElement> elements = document.QuerySelectorAll(dolarNames);
            IEnumerable<string> names = elements.Select(element => element.TextContent);

            return Ok(names);
        }

        [HttpGet("prices")]
        public async Task<IActionResult> Prices()
        {
            List<Dolar> dolares = new List<Dolar>();

            List<string> preciosCompra = new List<string>();
            List<string> preciosVenta = new List<string>();

            IDocument document = await context.OpenAsync(url);

            string dolarNamesSelector = "a.titleText";
            IHtmlCollection<IElement> dolarElements = document.QuerySelectorAll(dolarNamesSelector);
            IEnumerable<string> dolarNames = dolarElements.Select(element => element.TextContent);

            foreach (string name in dolarNames)
            {
                Dolar dolar = new Dolar();
                dolar.Name = name;
                dolares.Add(dolar);
            }

            dolares.RemoveAt(0);

            IHtmlCollection<IElement> precioCompraElements = document.QuerySelectorAll(dolarPrecioCompraSelector);
            IHtmlCollection<IElement> precioVentaElements = document.QuerySelectorAll(dolarPrecioVentasSelector);

            IEnumerable<string> dolarPreciosCompra = precioCompraElements.Select(element => element.TextContent);
            IEnumerable<string> dolarPreciosVenta = precioVentaElements.Select(element => element.TextContent);

            foreach (string dolarPrecioCompra in dolarPreciosCompra)
            {
                preciosCompra.Add(dolarPrecioCompra);
            }

            preciosCompra.RemoveAt(0);

            foreach (string dolarPrecioVenta in dolarPreciosVenta)
            {
                preciosVenta.Add(dolarPrecioVenta);
            }

            for (int i = 0; i < dolares.Count; i++)
            {
                string buy = "";

                if (i > 0 && i < preciosCompra.Count)
                {
                    buy = preciosCompra[i].Trim('$');
                } else
                {
                    buy = "0";
                }

                string sell = preciosVenta[i].Trim('$');
                float buyFixed = float.Parse(buy);
                float sellFixed = float.Parse(sell);

                dolares[i].Buy = buyFixed;
                dolares[i].Sell = sellFixed;
                dolares[i].Spread = sellFixed - buyFixed;
                dolares[i].Timestamp = Math.Abs((int)DateTimeOffset.Now.ToUnixTimeMilliseconds());

                if (sellFixed != dolares[i].Sell)
                {
                    dolares[i].Variation = (sellFixed - dolares[i].Sell) / dolares[i].Sell * 100;
                } else
                {
                    dolares[i].Variation = 0;
                }
            }

            return Ok(dolares);
        }
    }
}
