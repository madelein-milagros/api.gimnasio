using Gimnasio.Datos.Models;
using Gimnasio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class SocioRepositorio : ISocioRepositorio
{
    private readonly GimnasioContext _context;

    public SocioRepositorio(GimnasioContext context)
    {
        _context = context;
    }

    public async Task<List<Socio>> ObtenerTodosAsync()
    {
        return await _context.Socios
            .Include(s => s.Membresia)
            .OrderBy(s => s.Id)
            .ToListAsync();
    }

    public async Task<Socio?> ObtenerPorIdAsync(int id)
    {
        return await _context.Socios
            .Include(s => s.Membresia)
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Socio>> ObtenerPorMembresiaAsync(int membresiaId)
    {
        return await _context.Socios
            .Include(s => s.Membresia)
            .Where(s => s.MembresiaId == membresiaId)
            .OrderBy(s => s.NombreCompleto)
            .ToListAsync();
    }

    public async Task<List<Socio>> FiltrarAsync(string? nombre, int? membresiaId, bool? activo)
    {
        var consulta = _context.Socios
            .Include(s => s.Membresia)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            var termino = nombre.Trim().ToLower();
            consulta = consulta.Where(s => s.NombreCompleto.ToLower().Contains(termino));
        }

        if (membresiaId.HasValue)
        {
            consulta = consulta.Where(s => s.MembresiaId == membresiaId.Value);
        }

        if (activo.HasValue)
        {
            consulta = consulta.Where(s => s.Activo == activo.Value);
        }

        return await consulta
            .OrderBy(s => s.NombreCompleto)
            .ToListAsync();
    }

    public async Task<bool> ExisteDniAsync(string dni, int? idExcluir = null)
    {
        var dniNormalizado = dni.Trim();
        return await _context.Socios
            .Where(s => s.Dni == dniNormalizado && (!idExcluir.HasValue || s.Id != idExcluir.Value))
            .AnyAsync();
    }

    public async Task<Socio> CrearAsync(Socio socio)
    {
        _context.Socios.Add(socio);
        await _context.SaveChangesAsync();
        return await ObtenerPorIdAsync(socio.Id) ?? socio;
    }

    public async Task ActualizarAsync(Socio socio)
    {
        _context.Socios.Update(socio);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Socio socio)
    {
        _context.Socios.Remove(socio);
        await _context.SaveChangesAsync();
    }
}
