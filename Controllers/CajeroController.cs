using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backenddb_c.Data;
using backenddb_c.Models;
using Microsoft.Data.SqlClient;

namespace backenddb_c.Controllers
{
    public class CajeroController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CajeroController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cajero
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cajeros.ToListAsync());
        }

        // GET: Cajero/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cajero = await _context.Cajeros
                .FirstOrDefaultAsync(m => m.CodigoCajero == id);
            if (cajero == null)
            {
                return NotFound();
            }

            return View(cajero);
        }

        [HttpGet]
        public async Task<IActionResult> Transferir(int? id)
        {
            try
            {
                // Validación básica del ID
                if (id == null || id <= 0)
                {
                    TempData["ErrorMessage"] = "ID de cajero no válido";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener información del cajero
                var cajero = await _context.Cajeros
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CodigoCajero == id);

                if (cajero == null)
                {
                    TempData["ErrorMessage"] = $"Cajero con ID {id} no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                // Preparar ViewBag con datos para la vista
                ViewBag.CajeroUbicacion = cajero.Ubicacion;
                ViewBag.SaldoFormateado = cajero.Saldo.ToString("C2");

                // Obtener últimas transferencias de este cajero
                ViewBag.UltimasTransferencias = await _context.Movimientos
                    .Where(t => t.CodigoCajero == id && t.TipoOperacion == "Transferencia")
                    .OrderByDescending(t => t.Fecha)
                    .Take(5)
                    .ToListAsync();

                // Crear y devolver el ViewModel con el código del cajero preestablecido
                var model = new TransferenciaViewModel
                {
                    CodigoCajero = cajero.CodigoCajero
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al cargar la información del cajero";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transferir(TransferenciaViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Recargar datos necesarios para la vista si la validación falla
                    await CargarDatosVistaTransferencia(model.CodigoCajero);
                    return View(model);
                }

                // Verificar que el cajero existe
                var cajero = await _context.Cajeros.FindAsync(model.CodigoCajero);
                if (cajero == null)
                {
                    ModelState.AddModelError("", "Cajero no encontrado");
                    await CargarDatosVistaTransferencia(model.CodigoCajero);
                    return View(model);
                }

                // Verificar que no sea la misma cuenta de origen y destino
                if (model.CodigoCuentaOrigen == model.CodigoCuentaDestino)
                {
                    ModelState.AddModelError("CodigoCuentaDestino", "No puede transferir a la misma cuenta");
                    await CargarDatosVistaTransferencia(model.CodigoCajero);
                    return View(model);
                }

                // Ejecutar el stored procedure de transferencia
                var parameters = new[]
                {
            new SqlParameter("@p_codigo_titular", model.CodigoTitular),
            new SqlParameter("@p_codigo_origen", model.CodigoCuentaOrigen),
            new SqlParameter("@p_codigo_destino", model.CodigoCuentaDestino),
            new SqlParameter("@p_monto", model.Monto),
            new SqlParameter("@p_codigo_cajero", model.CodigoCajero)
        };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC realizar_transferencia @p_codigo_titular, @p_codigo_origen, @p_codigo_destino, @p_monto, @p_codigo_cajero",
                    parameters);

                TempData["SuccessMessage"] = $"Transferencia exitosa por {model.Monto.ToString("C2")}";
                return RedirectToAction("Details", new { id = model.CodigoCajero });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al realizar la transferencia: {ex.Message}");
                await CargarDatosVistaTransferencia(model.CodigoCajero);
                return View(model);
            }
        }

        private async Task CargarDatosVistaTransferencia(int? cajeroId)
        {
            if (cajeroId.HasValue)
            {
                var cajero = await _context.Cajeros.FindAsync(cajeroId);
                if (cajero != null)
                {
                    ViewBag.SaldoFormateado = cajero.Saldo.ToString("C2");
                    ViewBag.UltimasTransferencias = await _context.Movimientos
                        .Where(t => t.CodigoCajero == cajeroId)
                        .OrderByDescending(t => t.Fecha)
                        .Take(5)
                        .ToListAsync();
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> Extraer(int? id)
        {
            try
            {
                if (id == null || id <= 0)
                {
                    TempData["ErrorMessage"] = "ID de cajero no válido";
                    return RedirectToAction(nameof(Index));
                }

                var cajero = await _context.Cajeros
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CodigoCajero == id);

                if (cajero == null)
                {
                    TempData["ErrorMessage"] = $"Cajero con ID {id} no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.SaldoDisponible = cajero.Saldo.ToString("C2");
                return View(new ExtraccionViewModel { CodigoCajero = cajero.CodigoCajero });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el formulario de extracción";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Extraer(ExtraccionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Verificar saldo del cajero
                var cajero = await _context.Cajeros.FindAsync(model.CodigoCajero);
                if (cajero == null)
                {
                    ModelState.AddModelError("", "Cajero no encontrado");
                    return View(model);
                }

                if (cajero.Saldo < model.Monto)
                {
                    ModelState.AddModelError("Monto", "El cajero no tiene suficiente saldo");
                    ViewBag.SaldoDisponible = cajero.Saldo.ToString("C2");
                    return View(model);
                }

                // Ejecutar el stored procedure
                var parameters = new[]
                {
            new SqlParameter("@p_codigo_titular", model.CodigoTitular),
            new SqlParameter("@p_codigo_caja", model.CodigoCuenta),
            new SqlParameter("@p_monto", model.Monto),
            new SqlParameter("@p_codigo_cajero", model.CodigoCajero)
        };

                await _context.Database.ExecuteSqlRawAsync("EXEC realizar_extraccion @p_codigo_titular, @p_codigo_caja, @p_monto, @p_codigo_cajero", parameters);

                TempData["SuccessMessage"] = $"Extracción exitosa por {model.Monto.ToString("C2")}";
                return RedirectToAction("Details", new { id = model.CodigoCajero });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al realizar la extracción: {ex.Message}");
                return View(model);
            }
        }
        // GET: Cajero/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cajero/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoCajero,Ubicacion,Saldo")] Cajero cajero)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cajero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cajero);
        }

        // GET: Cajero/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cajero = await _context.Cajeros.FindAsync(id);
            if (cajero == null)
            {
                return NotFound();
            }
            return View(cajero);
        }

        // POST: Cajero/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoCajero,Ubicacion,Saldo")] Cajero cajero)
        {
            if (id != cajero.CodigoCajero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cajero);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CajeroExists(cajero.CodigoCajero))
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
            return View(cajero);
        }

        // GET: Cajero/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cajero = await _context.Cajeros
                .FirstOrDefaultAsync(m => m.CodigoCajero == id);
            if (cajero == null)
            {
                return NotFound();
            }

            return View(cajero);
        }

        // POST: Cajero/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cajero = await _context.Cajeros.FindAsync(id);
            if (cajero != null)
            {
                _context.Cajeros.Remove(cajero);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CajeroExists(int id)
        {
            return _context.Cajeros.Any(e => e.CodigoCajero == id);
        }
    }
}
