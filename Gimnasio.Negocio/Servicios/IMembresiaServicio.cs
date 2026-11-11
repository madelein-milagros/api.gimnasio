using Gimnasio.Entidades.DTOs;

namespace Gimnasio.Negocio.Servicios;

public interface IMembresiaServicio
{
    Task<List<MembresiaDto>> ObtenerTodasAsync();
    Task<MembresiaDto?> ObtenerPorIdAsync(int id);
    Task<MembresiaDto> CrearAsync(MembresiaCrearDto dto);
    Task<bool> ActualizarAsync(int id, MembresiaActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}
