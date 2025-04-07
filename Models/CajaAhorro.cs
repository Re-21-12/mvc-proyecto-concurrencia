using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class CajaAhorro
{
    public int CodigoCaja { get; set; }

    public string Descripcion { get; set; } = null!;

    public int CodigoCliente { get; set; }

    public decimal? SaldoCaja { get; set; }

    public virtual Cliente CodigoClienteNavigation { get; set; } = null!;

    public virtual ICollection<Tarjetum> Tarjeta { get; set; } = new List<Tarjetum>();
}
