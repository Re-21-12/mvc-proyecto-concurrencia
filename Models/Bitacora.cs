using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backenddb_c.Models;

public partial class Bitacora
{
    [Key] // Marca este campo como clave primaria temporal
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ← ¡Importante!
    public int Id { get; set; } // Campo adicional para scaffolding
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
