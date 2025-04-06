using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backenddb_c.Models;

public partial class InicioSesion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal? Secuencia { get; set; }

    public decimal? NumeroTarjeta { get; set; }

    public decimal? CodigoCaja { get; set; }

    public decimal? CodigoCliente { get; set; }

    public decimal? CodigoTitular { get; set; }

    public DateTime? FechaHora { get; set; }
}
