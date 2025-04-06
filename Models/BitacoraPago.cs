using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backenddb_c.Models;

public partial class BitacoraPago
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int NumTransaccion { get; set; }

    public decimal? CodigoPrestamo { get; set; }

    public decimal? MontoPago { get; set; }

    public DateTime? FechaPago { get; set; }

    public decimal? SaldoAnterior { get; set; }

    public decimal? SaldoNuevo { get; set; }

    public decimal? MesesPendiente { get; set; }

    public string? TipoTransaccion { get; set; }

    public string? UsuarioTransaccion { get; set; }

    public DateTime? FechaTransaccion { get; set; }
}
