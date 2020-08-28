using JMusik.Data.Interfaces;
using JMusik.Models;
using System.Threading.Tasks;

namespace JMusik.Data.Contratos
{
    public interface IUsuarioRepositorio : IRepositorioGenerico<Usuario>
    {
        Task<bool> CambiarContrasena(Usuario usuario);

        Task<bool> CambiarPerfil(Usuario usuario);

        Task<bool> ValidarContrasena(Usuario usuario);

        // Metodo para la sesion del usuario, usamos tuplas por que se requiere que si el resulado es correcto, devolver el usuario
        Task<(bool resultado, Usuario usuario)> ValidarDatosLogin(Usuario datosLoginUsuario);
    }
}