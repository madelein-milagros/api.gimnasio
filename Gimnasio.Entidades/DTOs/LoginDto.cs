using System.ComponentModel.DataAnnotations;

namespace Gimnasio.Entidades.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "La clave es obligatoria.")]
    public string Clave { get; set; } = null!;
}
