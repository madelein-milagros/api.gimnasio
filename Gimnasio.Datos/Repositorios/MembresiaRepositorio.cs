using Gimnasio.Datos.Models;
using Gimnasio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Repositorios;

public class MembresiaRepositorio : IMembresiaRepositorio
{
    private readonly GimnasioContext _context;

    public MembresiaRepositorio(GimnasioContext context)
    {
        _context = context;
    }

    public async Task<List<Membresia>> ObtenerTodasAsync()
    {
        return await _context.Membresias
            .Include(m => m.Socios)
            .OrderBy(m => m.Id)
            .ToListAsync();
    }

    public async Task<Membresia?> ObtenerPorIdAsync(int id)
    {
        return await _context.Membresias
            .Include(m => m.Socios)
            .Where(m => m.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null)
    {
        var nombreNormalizado = nombre.Trim().ToLower();
        return await _context.Membresias
            .Where(m => m.Nombre.ToLower() == nombreNormalizado && (!idExcluir.HasValue || m.Id != idExcluir.Value))
            .AnyAsync();
    }

    public async Task<bool> TieneSociosAsync(int id)
    {
        return await _context.Socios
            .Where(s => s.MembresiaId == id)
            .AnyAsync();
    }

    public async Task<Membresia> CrearAsync(Membresia membresia)
    {
        _context.Membresias.Add(membresia);
        await _context.SaveChangesAsync();
        return membresia;
    }

    public async Task ActualizarAsync(Membresia membresia)
    {
        _context.Membresias.Update(membresia);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Membresia membresia)
    {
        _context.Membresias.Remove(membresia);
        await _context.SaveChangesAsync();
    }
}
