using JMusik.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JMusik.Data.Interfaces
{
    // Estamos usando el patron de diseño repositorio, por ende, debemos separa la logica del acceso a datos, construyento un intermediario. Con el fin de que los controladores queden lo mas limpios posibles
    // Se crean interfaces para luego ser implementados por los respositorios como lo indica el patron de diseño
    public interface IProductoRepositorio
    {
        Task<List<Producto>> ObtenerProductosAsync();

        Task<Producto> ObtenerProductoAsync(int id);

        Task<Producto> Agregar(Producto producto);

        Task<bool> Actualizar(Producto producto);

        Task<bool> Eliminar(int id);
    }
}