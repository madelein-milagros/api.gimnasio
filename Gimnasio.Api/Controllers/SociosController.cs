using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SociosController : ControllerBase
{
    private readonly ISocioServicio _socioServicio;

    public SociosController(ISocioServicio socioServicio)
    {
        _socioServicio = socioServicio;
    }

    // 7. GET: api/socios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SocioDto>>> ObtenerTodos()
    {
        var socios = await _socioServicio.ObtenerTodosAsync();
        return Ok(socios);
    }

    // 8. GET con filtros por query string: api/socios/filtrar?nombre=...&membresiaId=...&activo=...
    [HttpGet("filtrar")]
    public async Task<ActionResult<IEnumerable<SocioDto>>> Filtrar(
        [FromQuery] string? nombre,
        [FromQuery] int? membresiaId,
        [FromQuery] bool? activo)
    {
        var socios = await _socioServicio.FiltrarAsync(nombre, membresiaId, activo);
        return Ok(socios);
    }

    // 9. GET: api/socios/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SocioDto>> ObtenerPorId(int id)
    {
        var socio = await _socioServicio.ObtenerPorIdAsync(id);
        if (socio == null)
        {
            return NotFound(new { mensaje = $"No se encontró el socio con ID {id}." });
        }

        return Ok(socio);
    }

    // 10. POST: api/socios
    [HttpPost]
    public async Task<ActionResult<SocioDto>> Crear([FromBody] SocioCrearDto dto)
    {
        try
        {
            var creado = await _socioServicio.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // 11. PUT: api/socios/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] SocioActualizarDto dto)
    {
        try
        {
            var actualizado = await _socioServicio.ActualizarAsync(id, dto);
            if (!actualizado)
            {
                return NotFound(new { mensaje = $"No se encontró el socio con ID {id}." });
            }

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // 12. DELETE: api/socios/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado = await _socioServicio.EliminarAsync(id);
        if (!eliminado)
        {
            return NotFound(new { mensaje = $"No se encontró el socio con ID {id}." });
        }

        return NoContent();
    }
}
