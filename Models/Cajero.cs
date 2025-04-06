using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Cajero
{
    public decimal CodigoCajero { get; set; }

    public string Ubicacion { get; set; } = null!;

    public decimal Saldo { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    public virtual ICollection<Operacion> Operacions { get; set; } = new List<Operacion>();
}
