using AutoMapper;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Entidades;
using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;

namespace Gimnasio.Negocio.Servicios;

public class SocioServicio : ISocioServicio
{
    private readonly ISocioRepositorio _socioRepositorio;
    private readonly IMembresiaRepositorio _membresiaRepositorio;
    private readonly IMapper _mapper;

    public SocioServicio(
        ISocioRepositorio socioRepositorio,
        IMembresiaRepositorio membresiaRepositorio,
        IMapper mapper)
    {
        _socioRepositorio = socioRepositorio;
        _membresiaRepositorio = membresiaRepositorio;
        _mapper = mapper;
    }

    public async Task<List<SocioDto>> ObtenerTodosAsync()
    {
        var socios = await _socioRepositorio.ObtenerTodosAsync();
        return _mapper.Map<List<SocioDto>>(socios);
    }

    public async Task<SocioDto?> ObtenerPorIdAsync(int id)
    {
        var socio = await _socioRepositorio.ObtenerPorIdAsync(id);
        return socio == null ? null : _mapper.Map<SocioDto>(socio);
    }

    public async Task<List<SocioDto>?> ObtenerPorMembresiaAsync(int membresiaId)
    {
        var membresia = await _membresiaRepositorio.ObtenerPorIdAsync(membresiaId);
        if (membresia == null)
        {
            return null;
        }

        var socios = await _socioRepositorio.ObtenerPorMembresiaAsync(membresiaId);
        return _mapper.Map<List<SocioDto>>(socios);
    }

    public async Task<List<SocioDto>> FiltrarAsync(string? nombre, int? membresiaId, bool? activo)
    {
        var socios = await _socioRepositorio.FiltrarAsync(nombre, membresiaId, activo);
        return _mapper.Map<List<SocioDto>>(socios);
    }

    public async Task<SocioDto> CrearAsync(SocioCrearDto dto)
    {
        // Regla de negocio 3: Edad mínima de inscripción (14 años)
        if (dto.Edad < 14)
        {
            throw new ReglaNegocioException("El socio debe tener al menos 14 años cumplidos para inscribirse en el gimnasio.");
        }

        // Regla de negocio 4: No permitir DNI duplicado
        if (await _socioRepositorio.ExisteDniAsync(dto.Dni))
        {
            throw new ReglaNegocioException($"Ya existe un socio registrado con el DNI '{dto.Dni}'.");
        }

        // Regla de negocio 5: La membresía asignada debe existir
        var membresia = await _membresiaRepositorio.ObtenerPorIdAsync(dto.MembresiaId);
        if (membresia == null)
        {
            throw new ReglaNegocioException($"La membresía con ID {dto.MembresiaId} no existe.");
        }

        var entidad = _mapper.Map<Socio>(dto);
        entidad.FechaInscripcion = DateTime.Now;
        entidad.Activo = true;

        var creado = await _socioRepositorio.CrearAsync(entidad);
        return _mapper.Map<SocioDto>(creado);
    }

    public async Task<bool> ActualizarAsync(int id, SocioActualizarDto dto)
    {
        var existente = await _socioRepositorio.ObtenerPorIdAsync(id);
        if (existente == null)
        {
            return false;
        }

        // Regla de negocio 3: Edad mínima de inscripción (14 años)
        if (dto.Edad < 14)
        {
            throw new ReglaNegocioException("El socio debe tener al menos 14 años cumplidos para estar inscrito en el gimnasio.");
        }

        // Regla de negocio 4: No permitir DNI duplicado con otro socio
        if (await _socioRepositorio.ExisteDniAsync(dto.Dni, id))
        {
            throw new ReglaNegocioException($"Ya existe otro socio registrado con el DNI '{dto.Dni}'.");
        }

        // Regla de negocio 5: La membresía asignada debe existir
        var membresia = await _membresiaRepositorio.ObtenerPorIdAsync(dto.MembresiaId);
        if (membresia == null)
        {
            throw new ReglaNegocioException($"La membresía con ID {dto.MembresiaId} no existe.");
        }

        _mapper.Map(dto, existente);
        await _socioRepositorio.ActualizarAsync(existente);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var existente = await _socioRepositorio.ObtenerPorIdAsync(id);
        if (existente == null)
        {
            return false;
        }

        await _socioRepositorio.EliminarAsync(existente);
        return true;
    }
}
