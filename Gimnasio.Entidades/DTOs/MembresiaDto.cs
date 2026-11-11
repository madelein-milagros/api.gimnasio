namespace Gimnasio.Entidades.DTOs;

public class MembresiaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal PrecioMensual { get; set; }
    public int DuracionMeses { get; set; }
    public int TotalSocios { get; set; }
}
