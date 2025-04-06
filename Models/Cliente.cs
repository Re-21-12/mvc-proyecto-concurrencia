using System;
using System.Collections.Generic;

namespace backenddb_c.Models;

public partial class Cliente
{
    public int CodigoCliente { get; set; }

    public string PrimerNombre { get; set; } = null!;

    public string SegundoNombre { get; set; } = null!;

    public string? TercerNombre { get; set; }

    public string PrimerApellido { get; set; } = null!;

    public string SegundoApellido { get; set; } = null!;

    public decimal Edad { get; set; }

    public string Direccion { get; set; } = null!;

    public virtual ICollection<CajaAhorro> CajaAhorros { get; set; } = new List<CajaAhorro>();

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
