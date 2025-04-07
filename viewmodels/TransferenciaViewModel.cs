using System.ComponentModel.DataAnnotations;

public class TransferenciaViewModel
{
    [Required(ErrorMessage = "El código del titular es requerido")]
    [Display(Name = "Código del Titular")]
    public int CodigoTitular { get; set; }

    [Required(ErrorMessage = "La cuenta origen es requerida")]
    [Display(Name = "Cuenta Origen")]
    public int CodigoCuentaOrigen { get; set; }

    [Required(ErrorMessage = "La cuenta destino es requerida")]
    [Display(Name = "Cuenta Destino")]
    [NotEqual("CodigoCuentaOrigen", ErrorMessage = "No puede transferir a la misma cuenta")]
    public int CodigoCuentaDestino { get; set; }

    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero")]
    [Display(Name = "Monto a Transferir")]
    public decimal Monto { get; set; }

    [Required]
    public int CodigoCajero { get; set; }
}

// Atributo personalizado para validar que dos propiedades no sean iguales
public class NotEqualAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public NotEqualAttribute(string otherProperty)
    {
        _otherProperty = otherProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherProperty);

        if (otherPropertyInfo == null)
        {
            return new ValidationResult($"Propiedad {_otherProperty} no encontrada");
        }

        var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

        if (object.Equals(value, otherValue))
        {
            return new ValidationResult(ErrorMessage ??
                $"Este valor no puede ser igual al campo {_otherProperty}");
        }

        return ValidationResult.Success;
    }
}