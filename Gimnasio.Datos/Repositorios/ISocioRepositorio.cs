using Gimnasio.Entidades;

namespace Gimnasio.Datos.Repositorios;

public interface ISocioRepositorio
{
    Task<List<Socio>> ObtenerTodosAsync();
    Task<Socio?> ObtenerPorIdAsync(int id);
    Task<List<Socio>> ObtenerPorMembresiaAsync(int membresiaId);
    Task<List<Socio>> FiltrarAsync(string? nombre, int? membresiaId, bool? activo);
    Task<bool> ExisteDniAsync(string dni, int? idExcluir = null);
    Task<Socio> CrearAsync(Socio socio);
    Task ActualizarAsync(Socio socio);
    Task EliminarAsync(Socio socio);
}
