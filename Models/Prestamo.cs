using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Prestamo
{
    public int CodigoPrestamo { get; set; }

    public decimal MontoInicial { get; set; }

    public decimal MontoPagado { get; set; }

    public decimal SaldoPendiente { get; set; }

    public DateTime FechaOtorgado { get; set; }

    public DateTime FechaVencimiento { get; set; }

    public string EstadoPrestamo { get; set; } = null!;

    public decimal MontoTotal { get; set; }

    public decimal Interes { get; set; }

    public decimal MesesPendiente { get; set; }

    public int CodigoCliente { get; set; }

    public virtual Cliente CodigoClienteNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
