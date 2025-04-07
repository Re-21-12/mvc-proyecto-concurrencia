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
    public class OperacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OperacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Operacion
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Operacions.Include(o => o.CodigoCajeroNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Operacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operacion = await _context.Operacions
                .Include(o => o.CodigoCajeroNavigation)
                .FirstOrDefaultAsync(m => m.CodigoOperacion == id);
            if (operacion == null)
            {
                return NotFound();
            }

            return View(operacion);
        }

        // GET: Operacion/Create
        public IActionResult Create()
        {
            ViewData["CodigoCajero"] = new SelectList(_context.Cajeros, "CodigoCajero", "CodigoCajero");
            return View();
        }

        // POST: Operacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoOperacion,NombreOperacion,CodigoCajero")] Operacion operacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(operacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodigoCajero"] = new SelectList(_context.Cajeros, "CodigoCajero", "CodigoCajero", operacion.CodigoCajero);
            return View(operacion);
        }

        // GET: Operacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operacion = await _context.Operacions.FindAsync(id);
            if (operacion == null)
            {
                return NotFound();
            }
            ViewData["CodigoCajero"] = new SelectList(_context.Cajeros, "CodigoCajero", "CodigoCajero", operacion.CodigoCajero);
            return View(operacion);
        }

        // POST: Operacion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoOperacion,NombreOperacion,CodigoCajero")] Operacion operacion)
        {
            if (id != operacion.CodigoOperacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(operacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OperacionExists(operacion.CodigoOperacion))
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
            ViewData["CodigoCajero"] = new SelectList(_context.Cajeros, "CodigoCajero", "CodigoCajero", operacion.CodigoCajero);
            return View(operacion);
        }

        // GET: Operacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operacion = await _context.Operacions
                .Include(o => o.CodigoCajeroNavigation)
                .FirstOrDefaultAsync(m => m.CodigoOperacion == id);
            if (operacion == null)
            {
                return NotFound();
            }

            return View(operacion);
        }

        // POST: Operacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var operacion = await _context.Operacions.FindAsync(id);
            if (operacion != null)
            {
                _context.Operacions.Remove(operacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OperacionExists(int id)
        {
            return _context.Operacions.Any(e => e.CodigoOperacion == id);
        }
    }
}
