using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthServicio _authServicio;

    public AuthController(IAuthServicio authServicio)
    {
        _authServicio = authServicio;
    }

    // 1. POST: api/auth/registro (Público)
    [AllowAnonymous]
    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
    {
        try
        {
            await _authServicio.RegistrarAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new
            {
                mensaje = "Usuario registrado correctamente con el rol por defecto 'Recepcionista'."
            });
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // 2. POST: api/auth/login (Público)
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto dto)
    {
        var resultado = await _authServicio.LoginAsync(dto);
        if (resultado == null)
        {
            return Unauthorized(new { mensaje = "Credenciales inválidas (correo o clave incorrectos)." });
        }

        return Ok(resultado);
    }

    // 3. GET: api/auth/perfil (Solo autenticados - lee User.Claims del token, no de la BD)
    [Authorize]
    [HttpGet("perfil")]
    public ActionResult<PerfilDto> Perfil()
    {
        var usuarioId = User.Claims.FirstOrDefault(c =>
            c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        var correo = User.Claims.FirstOrDefault(c =>
            c.Type == JwtRegisteredClaimNames.Email || c.Type == ClaimTypes.Email)?.Value ?? string.Empty;

        var rol = User.Claims.FirstOrDefault(c =>
            c.Type == "rol" || c.Type == ClaimTypes.Role)?.Value ?? string.Empty;

        return Ok(new PerfilDto
        {
            UsuarioId = usuarioId,
            Correo = correo,
            Rol = rol
        });
    }
}
