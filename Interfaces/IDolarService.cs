using AngleSharp.Dom;
using Dolarium.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dolarium.Interfaces
{
    public interface IDolarService
    {
        Task<List<string>> GetDolarNamesAsync();

        Task<List<Dolar>> GetDolarPricesAsync();
        
    }
}
