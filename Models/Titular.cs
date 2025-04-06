using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Titular
{
    public decimal CodigoTitular { get; set; }

    public string PrimerNombre { get; set; } = null!;

    public string SegundoNombre { get; set; } = null!;

    public string? TercerNombre { get; set; }

    public string PrimerApellido { get; set; } = null!;

    public string SegundoApellido { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public decimal Edad { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    public virtual Tarjetum? Tarjetum { get; set; }
}
