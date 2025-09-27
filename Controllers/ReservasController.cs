using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC2.Data;
using PC2.Models;

[Authorize]
public class ReservasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ReservasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Create(int inmuebleId)
    {
        var inmueble = await _context.Inmuebles.FindAsync(inmuebleId);
        if (inmueble == null) return NotFound();

        var existeActiva = await _context.Reservas.AnyAsync(r =>
            r.InmuebleId == inmuebleId && r.FechaExpiracion > DateTime.Now);

        if (existeActiva)
        {
            TempData["Error"] = "Este inmueble ya tiene una reserva activa.";
            return RedirectToAction("Details", "Inmuebles", new { id = inmuebleId });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["Error"] = "Debes iniciar sesión para reservar.";
            return RedirectToAction("Login", "Account");
        }

        var reserva = new Reserva
        {
            InmuebleId = inmuebleId,
            UsuarioId = user.Id,
            FechaCreacion = DateTime.Now,
            FechaExpiracion = DateTime.Now.AddHours(48)
        };

        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Reserva realizada por 48 horas.";
        return RedirectToAction("Details", "Inmuebles", new { id = inmuebleId });
    }
}