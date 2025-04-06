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
    public class BitacoraController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BitacoraController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bitacora
        public async Task<IActionResult> Index()
        {
            return View(await _context.Bitacoras.ToListAsync());
        }

        // GET: Bitacora/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacora = await _context.Bitacoras
                .FirstOrDefaultAsync(m => m.NumTransaccion == id);
            if (bitacora == null)
            {
                return NotFound();
            }

            return View(bitacora);
        }

        // GET: Bitacora/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bitacora/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NumTransaccion,NomTabla,NomCampo,NuevoValor,ValorAnterior,UsuarioTransaccion,FechaTransaccion,TipoTransaccion,LlavePrimaria")] Bitacora bitacora)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bitacora);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bitacora);
        }

        // GET: Bitacora/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacora = await _context.Bitacoras.FindAsync(id);
            if (bitacora == null)
            {
                return NotFound();
            }
            return View(bitacora);
        }

        // POST: Bitacora/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("NumTransaccion,NomTabla,NomCampo,NuevoValor,ValorAnterior,UsuarioTransaccion,FechaTransaccion,TipoTransaccion,LlavePrimaria")] Bitacora bitacora)
        {
            if (id != bitacora.NumTransaccion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bitacora);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BitacoraExists(bitacora.NumTransaccion))
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
            return View(bitacora);
        }

        // GET: Bitacora/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacora = await _context.Bitacoras
                .FirstOrDefaultAsync(m => m.NumTransaccion == id);
            if (bitacora == null)
            {
                return NotFound();
            }

            return View(bitacora);
        }

        // POST: Bitacora/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var bitacora = await _context.Bitacoras.FindAsync(id);
            if (bitacora != null)
            {
                _context.Bitacoras.Remove(bitacora);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BitacoraExists(long? id)
        {
            return _context.Bitacoras.Any(e => e.NumTransaccion == id);
        }
    }
}
