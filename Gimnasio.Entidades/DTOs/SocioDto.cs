namespace Gimnasio.Entidades.DTOs;

public class SocioDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string Dni { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Edad { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public bool Activo { get; set; }
    public int MembresiaId { get; set; }
    public string Membresia { get; set; } = null!;
    public decimal PrecioMensual { get; set; }
}
