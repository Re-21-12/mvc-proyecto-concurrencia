using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Tarjetum
{
    public decimal NumeroTarjeta { get; set; }

    public string Marca { get; set; } = null!;

    public DateTime FechaVencimiento { get; set; }

    public decimal Pin { get; set; }

    public decimal CodigoCaja { get; set; }

    public decimal CodigoTitular { get; set; }

    public virtual CajaAhorro CodigoCajaNavigation { get; set; } = null!;

    public virtual Titular CodigoTitularNavigation { get; set; } = null!;
}
