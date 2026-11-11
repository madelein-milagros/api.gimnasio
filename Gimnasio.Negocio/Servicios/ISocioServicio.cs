using Gimnasio.Entidades.DTOs;

namespace Gimnasio.Negocio.Servicios;

public interface ISocioServicio
{
    Task<List<SocioDto>> ObtenerTodosAsync();
    Task<SocioDto?> ObtenerPorIdAsync(int id);
    Task<List<SocioDto>?> ObtenerPorMembresiaAsync(int membresiaId);
    Task<List<SocioDto>> FiltrarAsync(string? nombre, int? membresiaId, bool? activo);
    Task<SocioDto> CrearAsync(SocioCrearDto dto);
    Task<bool> ActualizarAsync(int id, SocioActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}
