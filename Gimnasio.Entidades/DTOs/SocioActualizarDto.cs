using System.ComponentModel.DataAnnotations;

namespace Gimnasio.Entidades.DTOs;

public class SocioActualizarDto
{
    [Required(ErrorMessage = "El nombre completo del socio es obligatorio.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre completo debe tener entre 3 y 120 caracteres.")]
    public string NombreCompleto { get; set; } = null!;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    public string Dni { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(15, MinimumLength = 7, ErrorMessage = "El teléfono debe tener entre 7 y 15 caracteres.")]
    public string Telefono { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La edad es obligatoria.")]
    [Range(1, 100, ErrorMessage = "La edad debe ser un valor entre 1 y 100.")]
    public int Edad { get; set; }

    public bool Activo { get; set; } = true;

    [Required(ErrorMessage = "El identificador de la membresía es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un MembresiaId válido mayor a 0.")]
    public int MembresiaId { get; set; }
}
