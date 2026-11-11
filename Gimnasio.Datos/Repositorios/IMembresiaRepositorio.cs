using Gimnasio.Entidades;

namespace Gimnasio.Datos.Repositorios;

public interface IMembresiaRepositorio
{
    Task<List<Membresia>> ObtenerTodasAsync();
    Task<Membresia?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null);
    Task<bool> TieneSociosAsync(int id);
    Task<Membresia> CrearAsync(Membresia membresia);
    Task ActualizarAsync(Membresia membresia);
    Task EliminarAsync(Membresia membresia);
}
