using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Pago
{
    public int NumeroPago { get; set; }

    public DateTime FechaPago { get; set; }

    public decimal MontoPago { get; set; }

    public int CodigoPrestamo { get; set; }

    public virtual Prestamo CodigoPrestamoNavigation { get; set; } = null!;
}
