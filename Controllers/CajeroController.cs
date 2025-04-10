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
using backenddb_c.Extensions;
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
                        new OracleParameter("p_pin", model.Pin),
                        new OracleParameter("p_codigo_cajero", model.CajeroId)
                    };
                    // :p_codigo_cajero
                    await _context.Database.ExecuteSqlRawAsync(
                        "BEGIN BANCARIO.registrar_inicio_sesion(:p_numero_tarjeta, :p_pin, :p_codigo_cajero ); END;",
                        parameters
                    );

                    // Redirección a Details
                    return RedirectToAction("Details", new
                    {
                        id = model.CajeroId,
                        tarjetaCodigoCaja = tarjetaCodigoCaja,
                        Numerotarjeta = tarjeta.NumeroTarjeta
                    });
                }
                catch (OracleException ex) when (ex.Number == 20001)
                {
                    this.ShowError("PIN Incorrecto", "Error en Inicio Sesion");
                    return View(model);
                }
                catch (OracleException ex)
                {
                    this.ShowError("Ocurrio un error", "Inicio Sesion");

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                this.ShowError("Ocurrio un error", "Inicio Sesion");
                return View(model);
            }
        }

        // GET: Cajero/Details/5
        public async Task<IActionResult> Details(int? id, int? tarjetaCodigoCaja, int? Numerotarjeta)
        {
            if (id == null || tarjetaCodigoCaja == null)
            {
                return NotFound();
            }

            var cajero = await _context.Cajeros
                .FirstOrDefaultAsync(m => m.CodigoCajero == id);
            if (cajero == null || tarjetaCodigoCaja == null)
            {
                return NotFound();
            }

            var model = new CajeroViewModel
            {
                CodigoCajero = cajero.CodigoCajero,
                Ubicacion = cajero.Ubicacion,
                Saldo = cajero.Saldo,
                SaldoCaja = 0, // Valor por defecto
                NumeroTarjeta = Numerotarjeta ?? 0,
                tarjetaCodigoCaja = tarjetaCodigoCaja ?? 0
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
        public async Task<IActionResult> Transferir(int? id, int? tarjetaCodigoCaja, int? Numerotarjeta)
        {
            try
            {
                // Validación básica del ID
                if (id == null || id <= 0)
                {
                    TempData["ErrorMessage"] = "ID de cajero no válido";
                    return RedirectToAction(nameof(Index));
                }
                if (Numerotarjeta == null || Numerotarjeta < 0)
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
                    .FirstOrDefaultAsync(c => c.CodigoCaja == tarjetaCodigoCaja);
                var tarjeta = await _context.Tarjeta
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.NumeroTarjeta == Numerotarjeta);

                Console.WriteLine(tarjeta);

                if (tarjeta == null)
                {
                    TempData["ErrorMessage"] = $"Tarjeta con Numero de Tarjeta {Numerotarjeta} no encontrado";
                    return RedirectToAction(nameof(Index));
                }

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
                    CodigoCajero = cajero.CodigoCajero,
                    tarjetaCodigoCaja = tarjeta.CodigoCaja,
                    CodigoCuentaOrigen = caja_ahorro.CodigoCaja,
                    CodigoTitular = tarjeta.CodigoTitular,
                    Numerotarjeta = Numerotarjeta ?? 0,
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
                    this.ShowError("CodigoCuentaDestino No puede transferir a la misma cuenta");
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
                    "BEGIN BANCARIO.realizar_transferencia(:p_codigo_titular, :p_codigo_origen, :p_codigo_destino, :p_monto, :p_codigo_cajero); END;",
                    parameters
                );

                TempData["SuccessMessage"] = $"Transferencia exitosa por {model.Monto.ToString("C2")}";

                return RedirectToAction("Details", new { id = model.CodigoCajero, tarjetaCodigoCaja = model.CodigoCuentaOrigen, Numerotarjeta = model.Numerotarjeta });
            }
            catch (OracleException ex) when (ex.Number == 20001)
            {
                this.ShowError("Fondos insuficientes en la cuenta de origen", "Error en Transferencia");
                await CargarDatosVistaTransferencia(model.CodigoCajero);
                return View(model);
            }
            catch (OracleException ex) when (ex.Number == 20002)
            {
                this.ShowError("Una de las cuentas no existe", "Error en Transferencia");
                await CargarDatosVistaTransferencia(model.CodigoCajero);
                return View(model);
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
        public async Task<IActionResult> Extraer(int? id, int? tarjetaCodigoCaja, int? Numerotarjeta)
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

                var caja_ahorro = await _context.CajaAhorros
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(c => c.CodigoCaja == tarjetaCodigoCaja);

                var tarjeta = await _context.Tarjeta
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.NumeroTarjeta == Numerotarjeta);

                var titular = await _context.Titulars
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.CodigoTitular == tarjeta.CodigoTitular);
                if (cajero == null)
                {
                    TempData["ErrorMessage"] = $"Cajero con ID {id} no encontrado";
                    return RedirectToAction(nameof(Index));
                }
                if (caja_ahorro == null)
                {
                    TempData["ErrorMessage"] = $"Caja Ahorro con  {tarjetaCodigoCaja} no encontrado";
                    return RedirectToAction(nameof(Index));
                }
                if (tarjeta == null)
                {
                    TempData["ErrorMessage"] = $"Tarjeta con ID {Numerotarjeta} no encontrado";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.SaldoDisponible = cajero.Saldo.ToString("C2");
                ViewBag.CajeroUbicacion = cajero.Ubicacion;
                var model = new ExtraccionViewModel
                {
                    CodigoCajero = cajero.CodigoCajero,
                    tarjetaCodigoCaja = tarjetaCodigoCaja ?? 0,
                    CodigoTitular = tarjeta.CodigoTitular,
                    Numerotarjeta = Numerotarjeta ?? 0,

                };
                return View(model);
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
                    new OracleParameter("p_codigo_caja", model.tarjetaCodigoCaja),
                    new OracleParameter("p_monto", model.Monto),
                    new OracleParameter("p_codigo_cajero", model.CodigoCajero)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "BEGIN BANCARIO.realizar_extraccion(:p_codigo_titular, :p_codigo_caja, :p_monto, :p_codigo_cajero); END;",
                    parameters
                );

                TempData["SuccessMessage"] = $"Extracción exitosa por {model.Monto.ToString("C2")}";
                return RedirectToAction("Details", new { id = model.CodigoCajero, tarjetaCodigoCaja = model.tarjetaCodigoCaja, Numerotarjeta = model.Numerotarjeta });
            }
            catch (OracleException ex) when (ex.Number == 20001)
            {
                this.ShowError("Saldo insuficiente en la caja de ahorro", "Error en Extracción");
                return View(model);
            }
            catch (OracleException ex) when (ex.Number == 20002)
            {
                this.ShowError("Saldo insuficiente en la caja del cajero", "Error en Extracción");
                return View(model);
            }
            catch (OracleException ex) when (ex.Number == 20003)
            {
                this.ShowError("La caja de ahorro o el cajero no existen", "Error en Extracción");
                return View(model);
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
        [HttpGet]
        public async Task<IActionResult> PagoPrestamo(int? id, int? tarjetaCodigoCaja, int? Numerotarjeta)
        {
            try
            {
                if (id == null || Numerotarjeta == null)
                {
                    TempData["ErrorMessage"] = "Parámetros inválidos";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar que el cajero existe
                var cajero = await _context.Cajeros
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CodigoCajero == id);

                if (cajero == null)
                {
                    TempData["ErrorMessage"] = $"Cajero con ID {id} no encontrado";
                    return RedirectToAction(nameof(Index));
                }



                var caja_ahorro = await _context.CajaAhorros
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CodigoCaja == tarjetaCodigoCaja);

                if (caja_ahorro == null)
                {
                    TempData["ErrorMessage"] = $"Caja de Ahorro con ID {tarjetaCodigoCaja} no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.SaldoDisponible = caja_ahorro.SaldoCaja?.ToString("C2");
                ViewBag.CajeroUbicacion = cajero.Ubicacion;

                var model = new PagoPrestamoViewModel
                {
                    CodigoCajero = cajero.CodigoCajero,
                    Numerotarjeta = Numerotarjeta ?? 0,
                    tarjetaCodigoCaja = tarjetaCodigoCaja ?? 0,
                };

                return View(model);
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

                // Ejecutar el procedimiento almacenado
                try
                {
                    var parameters = new[]
                    {
                new OracleParameter("p_numero_tarjeta", model.Numerotarjeta),
                new OracleParameter("p_codigo_prestamo",model.CodigoPrestamo),
            };

                    await _context.Database.ExecuteSqlRawAsync(
                        "BEGIN BANCARIO.realizar_pago_prestamo(:p_numero_tarjeta,:p_codigo_prestamo); END;",
                        parameters
                    );

                    TempData["SuccessMessage"] = $"Pago  realizado con éxito";
                    return RedirectToAction("Details", new
                    {
                        id = model.CodigoCajero,
                        tarjetaCodigoCaja = model.tarjetaCodigoCaja,
                        Numerotarjeta = model.Numerotarjeta
                    });
                }
                catch (OracleException ex) when (ex.Number == 20001)
                {
                    this.ShowError("No se encontró ningún pago pendiente para el préstamo", "Error en Pago");
                    return View(model);
                }
                catch (OracleException ex) when (ex.Number == 20002)
                {
                    this.ShowError($"Error al realizar el pago: {ex.Message}", "Error en Pago");
                    return View(model);
                }
                catch (OracleException ex) when (ex.Number == 20003)
                {
                    this.ShowError("Fondos insuficientes en la caja de ahorro", "Error en Pago");
                    return View(model);
                }
                catch (OracleException ex)
                {
                    ModelState.AddModelError("", $"Error en la base de datos: {ex.Message}");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error inesperado: {ex.Message}");
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
