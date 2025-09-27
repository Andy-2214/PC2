using System.ComponentModel.DataAnnotations;

namespace PC2.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int InmuebleId { get; set; } 

        [Required]
        public string UsuarioId { get; set; } = string.Empty; 

        [Required]
        public DateTime FechaExpiracion { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}