using System.ComponentModel.DataAnnotations;

public class LoginTarjetaViewModel
{
    [Required(ErrorMessage = "El número de tarjeta es requerido")]
    [Display(Name = "Número de Tarjeta")]
    public int NumeroTarjeta { get; set; }

    [Required(ErrorMessage = "El PIN es requerido")]
    [DataType(DataType.Password)]
    [Display(Name = "PIN")]
    [Range(1000, 9999, ErrorMessage = "El PIN debe ser de 4 dígitos")]
    public int Pin { get; set; }

    public decimal? Saldo { get; set; } // Saldo de la tarjeta
    public decimal? SaldoCaja { get; set; } // Saldo de la tarjeta

    public int CajeroId { get; set; } // Para redirigir después del login
}