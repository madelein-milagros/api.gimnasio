using Gimnasio.Datos.Models;
using Gimnasio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly GimnasioContext _context;

    public UsuarioRepositorio(GimnasioContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
    {
        var correoNormalizado = correo.Trim().ToLower();
        return await _context.Usuarios
            .Where(u => u.Correo.ToLower() == correoNormalizado)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExisteCorreoAsync(string correo)
    {
        var correoNormalizado = correo.Trim().ToLower();
        return await _context.Usuarios
            .Where(u => u.Correo.ToLower() == correoNormalizado)
            .AnyAsync();
    }

    public async Task<Usuario> AgregarAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }
}
