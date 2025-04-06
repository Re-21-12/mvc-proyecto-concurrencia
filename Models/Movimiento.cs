using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Movimiento
{
    public decimal CodigoMovimiento { get; set; }

    public string TipoOperacion { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public decimal CodigoTitular { get; set; }

    public decimal CodigoCajero { get; set; }

    public decimal? CuentaDebitar { get; set; }

    public decimal? CuentaAcreditar { get; set; }

    public decimal Monto { get; set; }

    public virtual Cajero CodigoCajeroNavigation { get; set; } = null!;

    public virtual Titular CodigoTitularNavigation { get; set; } = null!;
}
