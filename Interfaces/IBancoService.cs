using Dolarium.Models;

namespace Dolarium.Interfaces
{
    public interface IBancoService
    {
        Task<List<Dolar>> GetDolaresBancosAsync();
    }
}
