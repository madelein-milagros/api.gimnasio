using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Entidades;
using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gimnasio.Negocio.Servicios;

public class AuthServicio : IAuthServicio
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthServicio(
        IUsuarioRepositorio usuarioRepositorio,
        IMapper mapper,
        IConfiguration configuration)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task RegistrarAsync(RegistroDto dto)
    {
        // Regla de negocio: Correo único
        if (await _usuarioRepositorio.ExisteCorreoAsync(dto.Correo))
        {
            throw new ReglaNegocioException($"Ya existe un usuario registrado con el correo '{dto.Correo}'.");
        }

        var usuario = _mapper.Map<Usuario>(dto);
        usuario.Correo = dto.Correo.Trim();
        usuario.ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Clave);
        // El servidor asigna siempre el rol por defecto del negocio, nunca el cliente
        usuario.Rol = "Recepcionista";

        await _usuarioRepositorio.AgregarAsync(usuario);
    }

    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarioRepositorio.ObtenerPorCorreoAsync(dto.Correo);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.ClaveHash))
        {
            return null;
        }

        return GenerarToken(usuario);
    }

    private TokenDto GenerarToken(Usuario usuario)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("No se encontró la clave Jwt:Key en la configuración.");
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expireMinutes = int.TryParse(_configuration["Jwt:ExpireMinutes"], out var mins) ? mins : 60;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim("rol", usuario.Rol)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiracion = DateTime.UtcNow.AddMinutes(expireMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiracion,
            signingCredentials: credentials);

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiracion = expiracion
        };
    }
}
