using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PC2.Models;

namespace PC2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

     
        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<Visita> Visitas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

      
            builder.Entity<Inmueble>()
                   .HasIndex(i => i.Codigo)
                   .IsUnique();

       
            builder.Entity<Visita>()
                   .HasOne<Inmueble>()
                   .WithMany()
                   .HasForeignKey(v => v.InmuebleId);

          
            builder.Entity<Reserva>()
                   .HasOne<Inmueble>()
                   .WithMany()
                   .HasForeignKey(r => r.InmuebleId);

        
            builder.Entity<Inmueble>().HasData(
                new Inmueble { Id = 1, Codigo = "DEP-001", Titulo = "Departamento céntrico", Tipo = "Departamento", Ciudad = "Lima", Direccion = "Av. Principal 123", Dormitorios = 3, Banos = 2, MetrosCuadrados = 85, Precio = 120000, Activo = true },
                new Inmueble { Id = 2, Codigo = "CAS-002", Titulo = "Casa con jardín", Tipo = "Casa", Ciudad = "Arequipa", Direccion = "Calle Los Olivos 456", Dormitorios = 4, Banos = 3, MetrosCuadrados = 150, Precio = 250000, Activo = true },
                new Inmueble { Id = 3, Codigo = "OFI-003", Titulo = "Oficina moderna", Tipo = "Oficina", Ciudad = "Cusco", Direccion = "Jr. Comercio 789", Dormitorios = 0, Banos = 1, MetrosCuadrados = 60, Precio = 95000, Activo = true }
            );
        }
    }
}