using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class InicioSesion
{
    public int Secuencia { get; set; }

    public decimal? NumeroTarjeta { get; set; }

    public decimal? CodigoCaja { get; set; }

    public decimal? CodigoCliente { get; set; }

    public decimal? CodigoTitular { get; set; }

    public DateTime? FechaHora { get; set; }
}
