using AngleSharp;
using AngleSharp.Dom;
using Dolarium.Models;

namespace Dolarium.Services
{
    public class DolarService
    {
        private static IBrowsingContext context;

        private const string URL = "https://dolarhoy.com/";

        private const string DOLAR_NAMES_SELECTOR = "a.titleText";
        private const string PRECIO_COMPRA_SELECTOR = "div.compra div.val";
        private const string PRECIO_VENTA_SELECTOR = "div.venta-wrapper div.val";

        public DolarService()
        {
            var config = Configuration.Default.WithDefaultLoader();
            context = BrowsingContext.New(config);
        }

        public async Task<List<string>> GetDolarNamesAsync()
        {
            var document = await context.OpenAsync(URL);
            return ExtractElements(document, DOLAR_NAMES_SELECTOR);
        }

        public async Task<List<Dolar>> GetDolarPricesAsync()
        {
            var document = await context.OpenAsync(URL);
            return ExtractDolaresFromDocument(document);
        }

        private List<Dolar> ExtractDolaresFromDocument(IDocument document)
        {
            var dolarNames = ExtractElements(document, DOLAR_NAMES_SELECTOR);
            var preciosCompra = ExtractElements(document, PRECIO_COMPRA_SELECTOR);
            var preciosVenta = ExtractElements(document, PRECIO_VENTA_SELECTOR);
            
            if (dolarNames.Count > 0) dolarNames.RemoveAt(0);
            if (preciosCompra.Count > 0) preciosCompra.RemoveAt(0);

            return BuildDolarList(dolarNames, preciosCompra, preciosVenta);
        }

        private List<Dolar> BuildDolarList(List<string> names, List<string> buys, List<string> sells)
        {
            var dolares = new List<Dolar>();

            for (int i = 0; i < names.Count; i++)
            {
                var buyPrice = ParsePrice(buys, i);
                var sellPrice = ParsePrice(sells, i);

                var dolar = new Dolar
                {
                    Name = names[i],
                    Buy = buyPrice,
                    Sell = sellPrice,
                    Spread = sellPrice - buyPrice,
                    Timestamp = GetCurrentUnixTimestamp(),
                    Variation = CalculateVariation(sellPrice, sellPrice)
                };

                dolares.Add(dolar);
            }

            return dolares;
        }

        private List<string> ExtractElements(IDocument document, string selector)
        {
            return document.QuerySelectorAll(selector)
                .Select(e => e.TextContent?.Trim() ?? "")
                .Where(e => !string.IsNullOrEmpty(e))
                .ToList();
        }

        private float ParsePrice(List<string> prices, int index)
        {
            if (index >= 0 && index < prices.Count)
            {
                var priceString = prices[index].Trim('$', ' ');

                if (float.TryParse(priceString, out float result))
                {
                    return result;
                }
            }
            return 0f;
        }

        private float CalculateVariation(float current, float previous)
        {
            if (previous == 0)
            {
                return 0;
            }

            return ((current - previous) / previous) * 100;
        }

        private int GetCurrentUnixTimestamp()
        {
            return (int)Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds());
        }
    }
}