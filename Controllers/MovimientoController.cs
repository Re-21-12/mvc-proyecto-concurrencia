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
    public class MovimientoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovimientoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movimiento
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Movimientos.Include(m => m.CodigoCajeroNavigation).Include(m => m.CodigoTitularNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Movimiento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimiento = await _context.Movimientos
                .Include(m => m.CodigoCajeroNavigation)
                .Include(m => m.CodigoTitularNavigation)
                .FirstOrDefaultAsync(m => m.CodigoMovimiento == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // GET: Movimiento/Create
        public IActionResult Create()
        {
            ViewData["CodigoCajero"] = new SelectList(_context.Cajeros, "CodigoCajero", "CodigoCajero");
            ViewData["CodigoTitular"] = new SelectList(_context.Titulars, "CodigoTitular", "CodigoTitular");
            return View();
        }

        // POST: Movimiento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoMovimiento,TipoOperacion,Fecha,CodigoTitular,CodigoCajero,CuentaDebitar,CuentaAcreditar,Monto")] Movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodigoCajero"] = new SelectList(_context.Cajeros, "CodigoCajero", "CodigoCajero", movimiento.CodigoCajero);
            ViewData["CodigoTitular"] = new SelectList(_context.Titulars, "CodigoTitular", "CodigoTitular", movimiento.CodigoTitular);
            return View(movimiento);
        }

        // GET: Movimiento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }
            ViewData["CodigoCajero"] = new SelectList(_context.Cajeros, "CodigoCajero", "CodigoCajero", movimiento.CodigoCajero);
            ViewData["CodigoTitular"] = new SelectList(_context.Titulars, "CodigoTitular", "CodigoTitular", movimiento.CodigoTitular);
            return View(movimiento);
        }

        // POST: Movimiento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoMovimiento,TipoOperacion,Fecha,CodigoTitular,CodigoCajero,CuentaDebitar,CuentaAcreditar,Monto")] Movimiento movimiento)
        {
            if (id != movimiento.CodigoMovimiento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovimientoExists(movimiento.CodigoMovimiento))
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
            ViewData["CodigoCajero"] = new SelectList(_context.Cajeros, "CodigoCajero", "CodigoCajero", movimiento.CodigoCajero);
            ViewData["CodigoTitular"] = new SelectList(_context.Titulars, "CodigoTitular", "CodigoTitular", movimiento.CodigoTitular);
            return View(movimiento);
        }

        // GET: Movimiento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimiento = await _context.Movimientos
                .Include(m => m.CodigoCajeroNavigation)
                .Include(m => m.CodigoTitularNavigation)
                .FirstOrDefaultAsync(m => m.CodigoMovimiento == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // POST: Movimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento != null)
            {
                _context.Movimientos.Remove(movimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovimientoExists(int id)
        {
            return _context.Movimientos.Any(e => e.CodigoMovimiento == id);
        }
    }
}
