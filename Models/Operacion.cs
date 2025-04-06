using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Operacion
{
    public decimal CodigoOperacion { get; set; }

    public string NombreOperacion { get; set; } = null!;

    public decimal CodigoCajero { get; set; }

    public virtual Cajero CodigoCajeroNavigation { get; set; } = null!;
}
