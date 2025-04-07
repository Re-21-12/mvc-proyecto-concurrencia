using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Tarjetum
{
    public int NumeroTarjeta { get; set; }

    public string Marca { get; set; } = null!;

    public DateTime FechaVencimiento { get; set; }

    public decimal Pin { get; set; }

    public int CodigoCaja { get; set; }

    public int CodigoTitular { get; set; }

    public virtual CajaAhorro CodigoCajaNavigation { get; set; } = null!;

    public virtual Titular CodigoTitularNavigation { get; set; } = null!;
}
