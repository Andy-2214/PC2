using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC2.Data;
using PC2.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PC2.Controllers
{
    [Authorize]
    public class VisitasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public VisitasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create(int inmuebleId)
        {
            var inmueble = await _context.Inmuebles.FindAsync(inmuebleId);
            if (inmueble == null) return NotFound();

            ViewBag.Inmueble = inmueble;
            return View(new Visita { InmuebleId = inmuebleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visita visita)
        {
            var inmueble = await _context.Inmuebles.FindAsync(visita.InmuebleId);
            if (inmueble == null) return NotFound();

            ViewBag.Inmueble = inmueble;

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            visita.UsuarioId = user.Id;

            if (visita.FechaInicio >= visita.FechaFin)
                ModelState.AddModelError("", "La fecha de inicio debe ser menor que la de fin.");

            if (visita.FechaInicio.Hour < 8 || visita.FechaFin.Hour > 19)
                ModelState.AddModelError("", "Las visitas deben programarse entre 08:00 y 19:00.");

            bool existeSolapada = await _context.Visitas
                .Where(v => v.InmuebleId == visita.InmuebleId && v.Estado != "Cancelada")
                .AnyAsync(v =>
                    (visita.FechaInicio < v.FechaFin && visita.FechaInicio >= v.FechaInicio) ||
                    (visita.FechaFin > v.FechaInicio && visita.FechaFin <= v.FechaFin) ||
                    (visita.FechaInicio <= v.FechaInicio && visita.FechaFin >= v.FechaFin)
                );

            if (existeSolapada)
                ModelState.AddModelError("", "Ya existe una visita en ese rango de tiempo.");

            if (!ModelState.IsValid)
                return View(visita);

            visita.Estado = "Solicitada";
            _context.Add(visita);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Visita agendada con éxito.";
            return RedirectToAction("Details", "Inmuebles", new { id = visita.InmuebleId });
        }
    }
}