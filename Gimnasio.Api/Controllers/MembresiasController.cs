using Gimnasio.Entidades.DTOs;
using Gimnasio.Negocio.Excepciones;
using Gimnasio.Negocio.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembresiasController : ControllerBase
{
    private readonly IMembresiaServicio _membresiaServicio;
    private readonly ISocioServicio _socioServicio;

    public MembresiasController(
        IMembresiaServicio membresiaServicio,
        ISocioServicio socioServicio)
    {
        _membresiaServicio = membresiaServicio;
        _socioServicio = socioServicio;
    }

    // 1. GET: api/membresias (Público)
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MembresiaDto>>> ObtenerTodas()
    {
        var membresias = await _membresiaServicio.ObtenerTodasAsync();
        return Ok(membresias);
    }

    // 2. GET: api/membresias/{id} (Público)
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MembresiaDto>> ObtenerPorId(int id)
    {
        var membresia = await _membresiaServicio.ObtenerPorIdAsync(id);
        if (membresia == null)
        {
            return NotFound(new { mensaje = $"No se encontró la membresía con ID {id}." });
        }

        return Ok(membresia);
    }

    // 3. GET (Ruta anidada): api/membresias/{id}/socios (Cualquier usuario autenticado: Recepcionista o Gerente)
    [HttpGet("{id:int}/socios")]
    public async Task<ActionResult<IEnumerable<SocioDto>>> ObtenerSociosPorMembresia(int id)
    {
        var socios = await _socioServicio.ObtenerPorMembresiaAsync(id);
        if (socios == null)
        {
            return NotFound(new { mensaje = $"No se encontró la membresía con ID {id}." });
        }

        return Ok(socios);
    }

    // 4. POST: api/membresias (Solo rol Gerente)
    [Authorize(Roles = "Gerente")]
    [HttpPost]
    public async Task<ActionResult<MembresiaDto>> Crear([FromBody] MembresiaCrearDto dto)
    {
        try
        {
            var creada = await _membresiaServicio.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // 5. PUT: api/membresias/{id} (Solo rol Gerente)
    [Authorize(Roles = "Gerente")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] MembresiaActualizarDto dto)
    {
        try
        {
            var actualizado = await _membresiaServicio.ActualizarAsync(id, dto);
            if (!actualizado)
            {
                return NotFound(new { mensaje = $"No se encontró la membresía con ID {id}." });
            }

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    // 6. DELETE: api/membresias/{id} (Solo rol Gerente)
    [Authorize(Roles = "Gerente")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var eliminado = await _membresiaServicio.EliminarAsync(id);
            if (!eliminado)
            {
                return NotFound(new { mensaje = $"No se encontró la membresía con ID {id}." });
            }

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
}
