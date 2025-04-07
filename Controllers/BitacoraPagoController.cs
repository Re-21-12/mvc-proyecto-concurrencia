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
    public class BitacoraPagoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BitacoraPagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BitacoraPago
        public async Task<IActionResult> Index()
        {
            return View(await _context.BitacoraPagos.ToListAsync());
        }

        // GET: BitacoraPago/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacoraPago = await _context.BitacoraPagos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bitacoraPago == null)
            {
                return NotFound();
            }

            return View(bitacoraPago);
        }

        // GET: BitacoraPago/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BitacoraPago/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumTransaccion,CodigoPrestamo,MontoPago,FechaPago,SaldoAnterior,SaldoNuevo,MesesPendiente,TipoTransaccion,UsuarioTransaccion,FechaTransaccion")] BitacoraPago bitacoraPago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bitacoraPago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bitacoraPago);
        }

        // GET: BitacoraPago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacoraPago = await _context.BitacoraPagos.FindAsync(id);
            if (bitacoraPago == null)
            {
                return NotFound();
            }
            return View(bitacoraPago);
        }

        // POST: BitacoraPago/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumTransaccion,CodigoPrestamo,MontoPago,FechaPago,SaldoAnterior,SaldoNuevo,MesesPendiente,TipoTransaccion,UsuarioTransaccion,FechaTransaccion")] BitacoraPago bitacoraPago)
        {
            if (id != bitacoraPago.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bitacoraPago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BitacoraPagoExists(bitacoraPago.Id))
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
            return View(bitacoraPago);
        }

        // GET: BitacoraPago/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacoraPago = await _context.BitacoraPagos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bitacoraPago == null)
            {
                return NotFound();
            }

            return View(bitacoraPago);
        }

        // POST: BitacoraPago/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bitacoraPago = await _context.BitacoraPagos.FindAsync(id);
            if (bitacoraPago != null)
            {
                _context.BitacoraPagos.Remove(bitacoraPago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BitacoraPagoExists(int id)
        {
            return _context.BitacoraPagos.Any(e => e.Id == id);
        }
    }
}
