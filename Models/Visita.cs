using System.ComponentModel.DataAnnotations;

namespace PC2.Models
{
    public class Visita
    {
        public int Id { get; set; }

        [Required]
        public int InmuebleId { get; set; } 

        [Required]
        public string UsuarioId { get; set; } = string.Empty;
        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        [DateGreaterThan("FechaInicio", ErrorMessage = "La fecha fin debe ser mayor que la fecha inicio")]
        public DateTime FechaFin { get; set; }

        [Required]
        public string Estado { get; set; } = "Solicitada";
        public string? Notas { get; set; }
    }

    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                return new ValidationResult($"Propiedad {_comparisonProperty} no encontrada.");

            var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

            if (currentValue.HasValue && comparisonValue.HasValue && currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}