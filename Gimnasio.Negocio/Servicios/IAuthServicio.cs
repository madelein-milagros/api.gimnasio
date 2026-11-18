using Gimnasio.Entidades.DTOs;

namespace Gimnasio.Negocio.Servicios;

public interface IAuthServicio
{
    Task RegistrarAsync(RegistroDto dto);
    Task<TokenDto?> LoginAsync(LoginDto dto);
}
