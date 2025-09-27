using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PC2.Data;
using PC2.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PC2.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InmueblesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? ciudad,
            string? tipo,
            decimal? precioMin,
            decimal? precioMax,
            int? dormitorios,
            int page = 1,
            int pageSize = 5
        )
        {
            var query = _context.Inmuebles
                .Where(i => i.Activo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(ciudad))
                query = query.Where(i => i.Ciudad == ciudad);

            if (!string.IsNullOrEmpty(tipo))
                query = query.Where(i => i.Tipo == tipo);

            if (precioMin.HasValue)
                query = query.Where(i => i.Precio >= precioMin.Value);

            if (precioMax.HasValue)
                query = query.Where(i => i.Precio <= precioMax.Value);

            if (dormitorios.HasValue)
                query = query.Where(i => i.Dormitorios >= dormitorios.Value);

            var totalItems = await query.CountAsync();
            var inmuebles = await query
                .OrderBy(i => (double)i.Precio)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        
            ViewData["Ciudad"] = ciudad;
            ViewData["Tipo"] = tipo;
            ViewData["PrecioMin"] = precioMin;
            ViewData["PrecioMax"] = precioMax;
            ViewData["Dormitorios"] = dormitorios;

            ViewBag.TotalItems = totalItems;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(inmuebles);
        }

public async Task<IActionResult> Details(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var inmueble = await _context.Inmuebles
        .FirstOrDefaultAsync(i => i.Id == id && i.Activo);

    if (inmueble == null)
    {
        return NotFound();
    }

    return View(inmueble);
}

    }

    
}