using System.ComponentModel.DataAnnotations;

public class ExtraccionViewModel
{
    [Required(ErrorMessage = "El código del titular es requerido")]
    [Display(Name = "Código del Titular")]
    public int CodigoTitular { get; set; }

    [Required(ErrorMessage = "El código de cuenta es requerido")]
    [Display(Name = "Cuenta de Ahorro")]
    public int CodigoCuenta { get; set; }

    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero")]
    [Display(Name = "Monto a Extraer")]
    public decimal Monto { get; set; }

    [Required]
    public int CodigoCajero { get; set; }

    public int tarjetaCodigoCaja { get; set; }
    public int Numerotarjeta { get; set; }
}