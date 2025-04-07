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
    public class InicioSesionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InicioSesionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InicioSesion
        public async Task<IActionResult> Index()
        {
            return View(await _context.InicioSesions.ToListAsync());
        }

        // GET: InicioSesion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inicioSesion = await _context.InicioSesions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inicioSesion == null)
            {
                return NotFound();
            }

            return View(inicioSesion);
        }

        // GET: InicioSesion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InicioSesion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Secuencia,NumeroTarjeta,CodigoCaja,CodigoCliente,CodigoTitular,FechaHora")] InicioSesion inicioSesion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inicioSesion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inicioSesion);
        }

        // GET: InicioSesion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inicioSesion = await _context.InicioSesions.FindAsync(id);
            if (inicioSesion == null)
            {
                return NotFound();
            }
            return View(inicioSesion);
        }

        // POST: InicioSesion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Secuencia,NumeroTarjeta,CodigoCaja,CodigoCliente,CodigoTitular,FechaHora")] InicioSesion inicioSesion)
        {
            if (id != inicioSesion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inicioSesion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InicioSesionExists(inicioSesion.Id))
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
            return View(inicioSesion);
        }

        // GET: InicioSesion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inicioSesion = await _context.InicioSesions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inicioSesion == null)
            {
                return NotFound();
            }

            return View(inicioSesion);
        }

        // POST: InicioSesion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inicioSesion = await _context.InicioSesions.FindAsync(id);
            if (inicioSesion != null)
            {
                _context.InicioSesions.Remove(inicioSesion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InicioSesionExists(int id)
        {
            return _context.InicioSesions.Any(e => e.Id == id);
        }
    }
}
