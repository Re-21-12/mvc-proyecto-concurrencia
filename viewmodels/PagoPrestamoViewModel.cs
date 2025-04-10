using System.ComponentModel.DataAnnotations;

public class PagoPrestamoViewModel
{
    [Required(ErrorMessage = "El código del préstamo es requerido")]
    [Display(Name = "Código de Préstamo")]
    public int CodigoPrestamo { get; set; }

    [Required]
    public int CodigoCajero { get; set; }

    public int tarjetaCodigoCaja { get; set; }
    public int Numerotarjeta { get; set; }
}
// TODO: Arreglar regresar en vistas, arreglar extraer entrar a la vista