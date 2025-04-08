using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backenddb_c.Data;
using backenddb_c.Models;
using Oracle.ManagedDataAccess.Client;

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

        // GET: Cajero/InicioSesion/5
        public async Task<IActionResult> InicioSesion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Verificar que el cajero existe
            var cajeroExists = await _context.Cajeros.AnyAsync(c => c.CodigoCajero == id);
            if (!cajeroExists)
            {
                return NotFound();
            }

            var model = new LoginTarjetaViewModel
            {
                CajeroId = id.Value
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> InicioSesion(LoginTarjetaViewModel model)
        {
            try
            {
                // Verificar que el modelo tiene el CajeroId
                if (model.CajeroId <= 0)
                {
                    TempData["ErrorMessage"] = "Cajero no especificado";
                    return View(model);
                }

                // Verificar que el cajero existe
                var cajeroExiste = await _context.Cajeros.AnyAsync(c => c.CodigoCajero == model.CajeroId);
                if (!cajeroExiste)
                {
                    TempData["ErrorMessage"] = "Cajero no encontrado";
                    return View(model);
                }

                // Verificar tarjeta
                var tarjetaExiste = await _context.Tarjeta.AnyAsync(t => t.NumeroTarjeta == model.NumeroTarjeta);
                if (!tarjetaExiste)
                {
                    TempData["ErrorMessage"] = "Tarjeta no encontrada";
                    return View(model);
                }
                var tarjeta = await _context.Tarjeta.FirstOrDefaultAsync(t => t.NumeroTarjeta == model.NumeroTarjeta);
                if (tarjeta.CodigoCaja == null)
                {
                    TempData["ErrorMessage"] = "La tarjeta no pertenece a este cajero";
                    return View(model);
                }
                var tarjetaCodigoCaja = tarjeta.CodigoCaja;


                // Ejecutar procedimiento almacenado con manejo explícito de errores
                try
                {
                    var parameters = new[]
                    {
                        new OracleParameter("p_numero_tarjeta", model.NumeroTarjeta),
                        new OracleParameter("p_pin", model.Pin)
                    };

                    await _context.Database.ExecuteSqlRawAsync(
                        "BEGIN registrar_inicio_sesion(:p_numero_tarjeta, :p_pin); END;",
                        parameters
                    );

                    // Redirección a Details
                    return RedirectToAction("Details", new
                    {
                        id = model.CajeroId,
                        tarjetaCodigoCaja = tarjetaCodigoCaja
                    });
                }
                catch (OracleException ex)
                {
                    Console.WriteLine($"Error en SP: {ex.Message}");
                    TempData["ErrorMessage"] = "Error en validación: " + ex.Message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                TempData["ErrorMessage"] = "Error inesperado: " + ex.Message;
                return View(model);
            }
        }

        // GET: Cajero/Details/5
        public async Task<IActionResult> Details(int? id, int? tarjetaCodigoCaja)
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

            var model = new CajeroViewModel
            {
                CodigoCajero = cajero.CodigoCajero,
                Ubicacion = cajero.Ubicacion,
                Saldo = cajero.Saldo,
                SaldoCaja = 0, // Valor por defecto
                NumeroTarjeta = tarjetaCodigoCaja ?? 0
            };

            if (tarjetaCodigoCaja.HasValue)
            {
                var cajaAhorro = await _context.CajaAhorros
                    .FirstOrDefaultAsync(m => m.CodigoCaja == tarjetaCodigoCaja.Value);

                if (cajaAhorro != null)
                {
                    model.SaldoCaja = cajaAhorro.SaldoCaja;
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Transferir(int? id, int? NumeroTarjeta)
        {
            try
            {
                // Validación básica del ID
                if (id == null || id <= 0)
                {
                    TempData["ErrorMessage"] = "ID de cajero no válido";
                    return RedirectToAction(nameof(Index));
                }
                if (NumeroTarjeta == null || NumeroTarjeta < 0)
                {
                    TempData["ErrorMessage"] = "Numero de Tarjeta no válido";
                    return RedirectToAction(nameof(Index));
                }
                // Obtener información del cajero
                var cajero = await _context.Cajeros
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CodigoCajero == id);

                var caja_ahorro = await _context.CajaAhorros
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CodigoCaja == NumeroTarjeta);


                if (cajero == null)
                {
                    TempData["ErrorMessage"] = $"Cajero con ID {id} no encontrado";
                    return RedirectToAction(nameof(Index));
                }
                if (caja_ahorro == null)
                {
                    TempData["ErrorMessage"] = $"Caja de Ahorro con Numero de Tarjeta {id} no encontrado";
                    return RedirectToAction(nameof(Index));
                }
                // Preparar ViewBag con datos para la vista
                ViewBag.SaldoFormateado = caja_ahorro.SaldoCaja?.ToString("C2");
                ViewBag.CajeroUbicacion = cajero.Ubicacion;

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
                    new OracleParameter("p_codigo_titular", model.CodigoTitular),
                    new OracleParameter("p_codigo_origen", model.CodigoCuentaOrigen),
                    new OracleParameter("p_codigo_destino", model.CodigoCuentaDestino),
                    new OracleParameter("p_monto", model.Monto),
                    new OracleParameter("p_codigo_cajero", model.CodigoCajero)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "BEGIN realizar_transferencia(:p_codigo_titular, :p_codigo_origen, :p_codigo_destino, :p_monto, :p_codigo_cajero); END;",
                    parameters
                );

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
                    new OracleParameter("p_codigo_titular", model.CodigoTitular),
                    new OracleParameter("p_codigo_caja", model.CodigoCuenta),
                    new OracleParameter("p_monto", model.Monto),
                    new OracleParameter("p_codigo_cajero", model.CodigoCajero)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "BEGIN realizar_extraccion(:p_codigo_titular, :p_codigo_caja, :p_monto, :p_codigo_cajero); END;",
                    parameters
                );

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
        // TODO: Cambiar esta implementacion por el uso de procedimientos almacenados
        [HttpGet]
        public async Task<IActionResult> PagoPrestamo(int? id, int? Numerotarjeta)
        {
            try
            {
                if (Numerotarjeta == null || id < 0)
                {
                    TempData["ErrorMessage"] = "Codigo de tarjeta  no válido";
                    return RedirectToAction(nameof(Index));
                }

                var CajaAhorro = await _context.CajaAhorros
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CodigoCaja == Numerotarjeta);

                var cajero = await _context.Cajeros
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CodigoCajero == id);

                if (CajaAhorro == null)
                {
                    TempData["ErrorMessage"] = $"Caja de Ahorro {Numerotarjeta} no encontrado";
                    return RedirectToAction(nameof(Index));
                }
                if (cajero == null)
                {
                    TempData["ErrorMessage"] = $"Cajero con ID {id} no encontrado";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.SaldoDisponible = CajaAhorro.SaldoCaja?.ToString("C2");



                return View(new PagoPrestamoViewModel { CodigoCajero = cajero.CodigoCajero });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el formulario de pago";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PagoPrestamo(PagoPrestamoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Verificar saldo del cajero
                var CajaAhrro = await _context.CajaAhorros.FindAsync(model.CodigoCajero);
                if (CajaAhrro == null)
                {
                    ModelState.AddModelError("", "Caja de Ahorro no encontrado");
                    return View(model);
                }

                if (CajaAhrro.SaldoCaja < model.Monto)
                {
                    ModelState.AddModelError("Monto", "El cajero no tiene suficiente saldo");
                    ViewBag.SaldoDisponible = CajaAhrro.SaldoCaja?.ToString("C2");
                    return View(model);
                }

                // Verificar que el préstamo existe
                var prestamo = await _context.Prestamos
                    .FirstOrDefaultAsync(p => p.CodigoPrestamo == model.CodigoPrestamo);

                if (prestamo == null)
                {
                    ModelState.AddModelError("CodigoPrestamo", "Préstamo no encontrado");
                    return View(model);
                }

                // Ejecutar el pago (el trigger se activará automáticamente)
                var pago = new Pago
                {
                    CodigoPrestamo = model.CodigoPrestamo,
                    MontoPago = model.Monto,
                    FechaPago = DateTime.Now,
                };

                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                CajaAhrro.SaldoCaja -= model.Monto;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Pago de {model.Monto.ToString("C2")} aplicado al préstamo {model.CodigoPrestamo}";
                return RedirectToAction("Details", new { id = model.CodigoCajero });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al procesar el pago: {ex.Message}");
                return View(model);
            }
        }

        // POST: Cajero/Create
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