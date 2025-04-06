using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backenddb_c.Models;

public partial class CajaAhorro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal CodigoCaja { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal CodigoCliente { get; set; }

    public decimal? SaldoCaja { get; set; }

    public virtual Cliente CodigoClienteNavigation { get; set; } = null!;

    public virtual ICollection<Tarjetum> Tarjeta { get; set; } = new List<Tarjetum>();
}
