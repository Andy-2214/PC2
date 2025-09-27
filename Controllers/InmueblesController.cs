using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PC2.Data;
using PC2.Models;
using System.Text.Json;

namespace PC2.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;

        public InmueblesController(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
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
       
            HttpContext.Session.SetString("Filtro_Ciudad", ciudad ?? "");
            HttpContext.Session.SetString("Filtro_Tipo", tipo ?? "");
            HttpContext.Session.SetString("Filtro_PrecioMin", precioMin?.ToString() ?? "");
            HttpContext.Session.SetString("Filtro_PrecioMax", precioMax?.ToString() ?? "");
            HttpContext.Session.SetString("Filtro_Dormitorios", dormitorios?.ToString() ?? "");

            
            string cacheKey = $"Inmuebles_{ciudad}_{tipo}_{precioMin}_{precioMax}_{dormitorios}_{page}_{pageSize}";

            List<Inmueble> inmuebles;
            int totalItems;

          
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedResult = JsonSerializer.Deserialize<CacheResult>(cachedData)!;
                inmuebles = cachedResult.Inmuebles;
                totalItems = cachedResult.TotalItems;
            }
            else
            {
              
                var query = _context.Inmuebles.Where(i => i.Activo).AsQueryable();

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

                totalItems = await query.CountAsync();
                inmuebles = await query
                    .OrderBy(i => (double)i.Precio)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

             
                var cacheResult = new CacheResult { Inmuebles = inmuebles, TotalItems = totalItems };
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cacheResult), options);
            }

           
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
            if (id == null) return NotFound();

            var inmueble = await _context.Inmuebles
                .FirstOrDefaultAsync(i => i.Id == id && i.Activo);

            if (inmueble == null) return NotFound();

      
            HttpContext.Session.SetInt32("UltimoInmuebleId", inmueble.Id);
            HttpContext.Session.SetString("UltimoInmuebleTitulo", inmueble.Titulo);

            return View(inmueble);
        }

       
        private class CacheResult
        {
            public List<Inmueble> Inmuebles { get; set; } = new();
            public int TotalItems { get; set; }
        }
    }
}