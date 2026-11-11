using System.ComponentModel.DataAnnotations;

namespace Gimnasio.Entidades.DTOs;

public class MembresiaCrearDto
{
    [Required(ErrorMessage = "El nombre de la membresía es obligatorio.")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 80 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La descripción de la membresía es obligatoria.")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 250 caracteres.")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage = "El precio mensual es obligatorio.")]
    [Range(10.00, 2000.00, ErrorMessage = "El precio mensual debe estar entre 10.00 y 2000.00.")]
    public decimal PrecioMensual { get; set; }

    [Required(ErrorMessage = "La duración en meses es obligatoria.")]
    [Range(1, 24, ErrorMessage = "La duración en meses debe estar entre 1 y 24.")]
    public int DuracionMeses { get; set; }
}
