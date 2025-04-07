using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Operacion
{
    public int CodigoOperacion { get; set; }

    public string NombreOperacion { get; set; } = null!;

    public int CodigoCajero { get; set; }

    public virtual Cajero CodigoCajeroNavigation { get; set; } = null!;
}
