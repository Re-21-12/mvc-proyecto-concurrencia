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
    public class TitularController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TitularController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Titular
        public async Task<IActionResult> Index()
        {
            return View(await _context.Titulars.ToListAsync());
        }

        // GET: Titular/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var titular = await _context.Titulars
                .FirstOrDefaultAsync(m => m.CodigoTitular == id);
            if (titular == null)
            {
                return NotFound();
            }

            return View(titular);
        }

        // GET: Titular/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Titular/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoTitular,PrimerNombre,SegundoNombre,TercerNombre,PrimerApellido,SegundoApellido,Direccion,Edad")] Titular titular)
        {
            if (ModelState.IsValid)
            {
                _context.Add(titular);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(titular);
        }

        // GET: Titular/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var titular = await _context.Titulars.FindAsync(id);
            if (titular == null)
            {
                return NotFound();
            }
            return View(titular);
        }

        // POST: Titular/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoTitular,PrimerNombre,SegundoNombre,TercerNombre,PrimerApellido,SegundoApellido,Direccion,Edad")] Titular titular)
        {
            if (id != titular.CodigoTitular)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(titular);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TitularExists(titular.CodigoTitular))
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
            return View(titular);
        }

        // GET: Titular/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var titular = await _context.Titulars
                .FirstOrDefaultAsync(m => m.CodigoTitular == id);
            if (titular == null)
            {
                return NotFound();
            }

            return View(titular);
        }

        // POST: Titular/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var titular = await _context.Titulars.FindAsync(id);
            if (titular != null)
            {
                _context.Titulars.Remove(titular);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TitularExists(int id)
        {
            return _context.Titulars.Any(e => e.CodigoTitular == id);
        }
    }
}
