using System.ComponentModel.DataAnnotations;
using backenddb_c.Models;

public class CajeroViewModel
{

    public decimal? SaldoCaja { get; set; } // Saldo de la tarjeta
    public int CodigoCajero { get; set; }
    public int NumeroTarjeta { get; set; }


    public string Ubicacion { get; set; } = null!;

    public decimal Saldo { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    public virtual ICollection<Operacion> Operacions { get; set; } = new List<Operacion>();

}