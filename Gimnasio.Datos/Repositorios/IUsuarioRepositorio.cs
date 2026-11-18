using Gimnasio.Entidades;

namespace Gimnasio.Datos.Repositorios;

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correo);
    Task<bool> ExisteCorreoAsync(string correo);
    Task<Usuario> AgregarAsync(Usuario usuario);
}
