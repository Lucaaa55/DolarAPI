using AngleSharp;
using Dolarium.Interfaces;
using Dolarium.Models;
using System.Resources;

namespace Dolarium.Services
{
    public class IndiceService : IIndiceService
    {
        private readonly IBrowsingContext context;

        private readonly List<Indice> indices;
        private readonly List<IndiceUrl> urls;

        public IndiceService()
        {
            var config = Configuration.Default.WithDefaultLoader();
            context = BrowsingContext.New(config);

            urls = InitializeIndicesUrls();
            indices = new List<Indice>();
        }

        public async Task<List<Indice>> GetAllIndexes()
        {
            urls.ForEach(async url =>
            {
                await ExtractDocumentValues(url);
            });

            return indices;
        }

        private async Task<Indice> ExtractDocumentValues(IndiceUrl url)
        {
            var indice = new Indice();

            var document = await context.OpenAsync(url.Url);
            var element = document.QuerySelectorAll(url.Selector);
            var content = element.Select(m => m.TextContent);

            indice.Tipo = url.Name;
            indice.Valor = ParseResult(content.FirstOrDefault());

            Console.WriteLine(indice.Valor);

            AddToList(indice);
            return indice;
        }

        private float ParseResult(string content)
        {
            return float.Parse(content);
        }

        private void AddToList(Indice indice)
        {
            indice.Date = GetCurrentTime();
            indices.Add(indice);
        }

        private int GetCurrentTime()
        {
            return (int)Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        private List<IndiceUrl> InitializeIndicesUrls()
        {
            return new List<IndiceUrl>
            {
                /* new IndiceUrl
                {
                    Url = "https://www.afascl.coop/afadiario/mercados-en-linea",
                    Selector = "div.badge badge-dark m-0 p-0 pt-2 w-100 > h4"
                }, */
                new IndiceUrl
                {
                    Name = "Riesgo pais",
                    Url = "https://www.rava.com/perfil/RIESGO%20PAIS",
                    Selector = "div#izqCotiza > p"
                },
                /* new IndiceUrl
                {
                    Name = "Inflacion mensual",
                    Url = "https://argenstats.com/indicadores/inflacion",
                    Selector = "data-slot"
                } */
            };
        }
    }
}
