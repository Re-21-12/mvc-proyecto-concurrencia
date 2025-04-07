using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Bitacora
{
    public int NumTransaccion { get; set; }

    public string? NomTabla { get; set; }

    public string? NomCampo { get; set; }

    public string? NuevoValor { get; set; }

    public string? ValorAnterior { get; set; }

    public string? UsuarioTransaccion { get; set; }

    public DateTime? FechaTransaccion { get; set; }

    public string? TipoTransaccion { get; set; }

    public decimal? LlavePrimaria { get; set; }
}
