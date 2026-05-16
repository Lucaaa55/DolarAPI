using Dolarium.Models;

namespace Dolarium.Interfaces
{
    public interface IIndiceService
    {
        Task<List<Indice>> GetAllIndexes();
    }
}
