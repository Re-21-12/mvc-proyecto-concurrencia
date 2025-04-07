using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backenddb_c.Data;
using backenddb_c.Models;

namespace backenddb_c.Controllers
{
    public class TarjetaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TarjetaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tarjeta
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tarjeta.Include(t => t.CodigoCajaNavigation).Include(t => t.CodigoTitularNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tarjeta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarjetum = await _context.Tarjeta
                .Include(t => t.CodigoCajaNavigation)
                .Include(t => t.CodigoTitularNavigation)
                .FirstOrDefaultAsync(m => m.NumeroTarjeta == id);
            if (tarjetum == null)
            {
                return NotFound();
            }

            return View(tarjetum);
        }

        // GET: Tarjeta/Create
        public IActionResult Create()
        {
            ViewData["CodigoCaja"] = new SelectList(_context.CajaAhorros, "CodigoCaja", "CodigoCaja");
            ViewData["CodigoTitular"] = new SelectList(_context.Titulars, "CodigoTitular", "CodigoTitular");
            return View();
        }

        // POST: Tarjeta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumeroTarjeta,Marca,FechaVencimiento,Pin,CodigoCaja,CodigoTitular")] Tarjetum tarjetum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarjetum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodigoCaja"] = new SelectList(_context.CajaAhorros, "CodigoCaja", "CodigoCaja", tarjetum.CodigoCaja);
            ViewData["CodigoTitular"] = new SelectList(_context.Titulars, "CodigoTitular", "CodigoTitular", tarjetum.CodigoTitular);
            return View(tarjetum);
        }

        // GET: Tarjeta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarjetum = await _context.Tarjeta.FindAsync(id);
            if (tarjetum == null)
            {
                return NotFound();
            }
            ViewData["CodigoCaja"] = new SelectList(_context.CajaAhorros, "CodigoCaja", "CodigoCaja", tarjetum.CodigoCaja);
            ViewData["CodigoTitular"] = new SelectList(_context.Titulars, "CodigoTitular", "CodigoTitular", tarjetum.CodigoTitular);
            return View(tarjetum);
        }

        // POST: Tarjeta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NumeroTarjeta,Marca,FechaVencimiento,Pin,CodigoCaja,CodigoTitular")] Tarjetum tarjetum)
        {
            if (id != tarjetum.NumeroTarjeta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarjetum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarjetumExists(tarjetum.NumeroTarjeta))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodigoCaja"] = new SelectList(_context.CajaAhorros, "CodigoCaja", "CodigoCaja", tarjetum.CodigoCaja);
            ViewData["CodigoTitular"] = new SelectList(_context.Titulars, "CodigoTitular", "CodigoTitular", tarjetum.CodigoTitular);
            return View(tarjetum);
        }

        // GET: Tarjeta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarjetum = await _context.Tarjeta
                .Include(t => t.CodigoCajaNavigation)
                .Include(t => t.CodigoTitularNavigation)
                .FirstOrDefaultAsync(m => m.NumeroTarjeta == id);
            if (tarjetum == null)
            {
                return NotFound();
            }

            return View(tarjetum);
        }

        // POST: Tarjeta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarjetum = await _context.Tarjeta.FindAsync(id);
            if (tarjetum != null)
            {
                _context.Tarjeta.Remove(tarjetum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarjetumExists(int id)
        {
            return _context.Tarjeta.Any(e => e.NumeroTarjeta == id);
        }
    }
}
