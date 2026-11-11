using System;
using System.Collections.Generic;

namespace Gimnasio.Entidades;

public partial class Membresia
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal PrecioMensual { get; set; }

    public int DuracionMeses { get; set; }

    public virtual ICollection<Socio> Socios { get; set; } = new List<Socio>();
}
