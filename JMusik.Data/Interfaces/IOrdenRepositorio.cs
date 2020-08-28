using JMusik.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JMusik.Data.Interfaces
{
    public interface IOrdenRepositorio : IRepositorioGenerico<Orden>
    {
        Task<IEnumerable<Orden>> ObtenerTodosConDetallesAsync();

        Task<Orden> ObtenerConDetallesAsync(int id);
    }
}