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
    public class CajaAhorroController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CajaAhorroController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CajaAhorro
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CajaAhorros.Include(c => c.CodigoClienteNavigation);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CajaAhorro/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cajaAhorro = await _context.CajaAhorros
                .Include(c => c.CodigoClienteNavigation)
                .FirstOrDefaultAsync(m => m.CodigoCaja == id);
            if (cajaAhorro == null)
            {
                return NotFound();
            }

            return View(cajaAhorro);
        }

        // GET: CajaAhorro/Create
        public IActionResult Create()
        {
            ViewData["CodigoCliente"] = new SelectList(_context.Clientes, "CodigoCliente", "CodigoCliente");
            return View();
        }

        // POST: CajaAhorro/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoCaja,Descripcion,CodigoCliente,SaldoCaja")] CajaAhorro cajaAhorro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cajaAhorro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodigoCliente"] = new SelectList(_context.Clientes, "CodigoCliente", "CodigoCliente", cajaAhorro.CodigoCliente);
            return View(cajaAhorro);
        }

        // GET: CajaAhorro/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cajaAhorro = await _context.CajaAhorros.FindAsync(id);
            if (cajaAhorro == null)
            {
                return NotFound();
            }
            ViewData["CodigoCliente"] = new SelectList(_context.Clientes, "CodigoCliente", "CodigoCliente", cajaAhorro.CodigoCliente);
            return View(cajaAhorro);
        }

        // POST: CajaAhorro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoCaja,Descripcion,CodigoCliente,SaldoCaja")] CajaAhorro cajaAhorro)
        {
            if (id != cajaAhorro.CodigoCaja)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cajaAhorro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CajaAhorroExists(cajaAhorro.CodigoCaja))
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
            ViewData["CodigoCliente"] = new SelectList(_context.Clientes, "CodigoCliente", "CodigoCliente", cajaAhorro.CodigoCliente);
            return View(cajaAhorro);
        }

        // GET: CajaAhorro/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cajaAhorro = await _context.CajaAhorros
                .Include(c => c.CodigoClienteNavigation)
                .FirstOrDefaultAsync(m => m.CodigoCaja == id);
            if (cajaAhorro == null)
            {
                return NotFound();
            }

            return View(cajaAhorro);
        }

        // POST: CajaAhorro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cajaAhorro = await _context.CajaAhorros.FindAsync(id);
            if (cajaAhorro != null)
            {
                _context.CajaAhorros.Remove(cajaAhorro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CajaAhorroExists(int id)
        {
            return _context.CajaAhorros.Any(e => e.CodigoCaja == id);
        }
    }
}
