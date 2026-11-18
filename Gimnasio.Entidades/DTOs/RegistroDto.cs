using System.ComponentModel.DataAnnotations;

namespace Gimnasio.Entidades.DTOs;

public class RegistroDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "La clave es obligatoria.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La clave debe tener al menos 6 caracteres.")]
    public string Clave { get; set; } = null!;
}
