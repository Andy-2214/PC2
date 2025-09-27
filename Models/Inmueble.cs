using System.ComponentModel.DataAnnotations;

namespace PC2.Models
{
    public class Inmueble
    {
        public int Id { get; set; }

        [Required]
        public string Codigo { get; set; } = string.Empty; 

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public string? Imagen { get; set; }

        [Required]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        public string Ciudad { get; set; } = string.Empty;

        [Required]
        public string Direccion { get; set; } = string.Empty;

        [Range(1, 20, ErrorMessage = "Dormitorios debe ser mayor a 0")]
        public int Dormitorios { get; set; }

        [Range(1, 20, ErrorMessage = "Baños debe ser mayor a 0")]
        public int Banos { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Metros cuadrados debe ser mayor a 0")]
        public int MetrosCuadrados { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        public bool Activo { get; set; } = true;
    }
}
