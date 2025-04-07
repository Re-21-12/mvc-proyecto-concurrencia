using System.ComponentModel.DataAnnotations;

public class PagoPrestamoViewModel
{
    [Required(ErrorMessage = "El código del préstamo es requerido")]
    [Display(Name = "Código de Préstamo")]
    public int CodigoPrestamo { get; set; }

    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero")]
    [Display(Name = "Monto a Pagar")]
    public decimal Monto { get; set; }

    [Required]
    public int CodigoCajero { get; set; }
}