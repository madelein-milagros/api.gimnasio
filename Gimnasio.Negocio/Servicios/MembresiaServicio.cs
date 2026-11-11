using AutoMapper;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Entidades;
using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;

namespace Gimnasio.Negocio.Servicios;

public class MembresiaServicio : IMembresiaServicio
{
    private readonly IMembresiaRepositorio _membresiaRepositorio;
    private readonly IMapper _mapper;

    public MembresiaServicio(IMembresiaRepositorio membresiaRepositorio, IMapper mapper)
    {
        _membresiaRepositorio = membresiaRepositorio;
        _mapper = mapper;
    }

    public async Task<List<MembresiaDto>> ObtenerTodasAsync()
    {
        var membresias = await _membresiaRepositorio.ObtenerTodasAsync();
        return _mapper.Map<List<MembresiaDto>>(membresias);
    }

    public async Task<MembresiaDto?> ObtenerPorIdAsync(int id)
    {
        var membresia = await _membresiaRepositorio.ObtenerPorIdAsync(id);
        return membresia == null ? null : _mapper.Map<MembresiaDto>(membresia);
    }

    public async Task<MembresiaDto> CrearAsync(MembresiaCrearDto dto)
    {
        // Regla de negocio 1: No permitir nombres de membresía duplicados
        if (await _membresiaRepositorio.ExisteNombreAsync(dto.Nombre))
        {
            throw new ReglaNegocioException($"Ya existe una membresía registrada con el nombre '{dto.Nombre}'.");
        }

        var entidad = _mapper.Map<Membresia>(dto);
        var creada = await _membresiaRepositorio.CrearAsync(entidad);
        return _mapper.Map<MembresiaDto>(creada);
    }

    public async Task<bool> ActualizarAsync(int id, MembresiaActualizarDto dto)
    {
        var existente = await _membresiaRepositorio.ObtenerPorIdAsync(id);
        if (existente == null)
        {
            return false;
        }

        // Regla de negocio 1: No permitir actualizar con el nombre de otra membresía existente
        if (await _membresiaRepositorio.ExisteNombreAsync(dto.Nombre, id))
        {
            throw new ReglaNegocioException($"Ya existe otra membresía registrada con el nombre '{dto.Nombre}'.");
        }

        _mapper.Map(dto, existente);
        await _membresiaRepositorio.ActualizarAsync(existente);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var existente = await _membresiaRepositorio.ObtenerPorIdAsync(id);
        if (existente == null)
        {
            return false;
        }

        // Regla de negocio 2: No permitir eliminar una membresía que tiene socios registrados
        if (await _membresiaRepositorio.TieneSociosAsync(id))
        {
            throw new ReglaNegocioException("No se puede eliminar la membresía porque tiene socios registrados asociados.");
        }

        await _membresiaRepositorio.EliminarAsync(existente);
        return true;
    }
}
