using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backenddb_c.Models;

public partial class InicioSesion
{
    [Key] // Marca este campo como clave primaria temporal
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ← ¡Importante!

    public int Id { get; set; } // Campo adicional para scaffolding
    public int Secuencia { get; set; }

    public decimal? NumeroTarjeta { get; set; }

    public decimal? CodigoCaja { get; set; }

    public decimal? CodigoCliente { get; set; }

    public decimal? CodigoTitular { get; set; }

    public DateTime? FechaHora { get; set; }
}
