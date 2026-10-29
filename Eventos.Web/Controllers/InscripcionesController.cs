using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _inscripciones;

    public InscripcionesController(InscripcionRepositorio inscripciones)
    {
        _inscripciones = inscripciones;
    }

    public async Task<IActionResult> Index()
    {
        var lista = await _inscripciones.ListarAsync();
        return View(lista);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Inscripcion());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventoId,AsistenteId")] Inscripcion inscripcion)
    {
        if (ModelState.IsValid)
        {
            var evento = await _inscripciones.ObtenerEventoActivoAsync(inscripcion.EventoId);
            if (evento == null)
            {
                ModelState.AddModelError(nameof(inscripcion.EventoId), "El evento seleccionado no existe o no está activo.");
            }
            else
            {
                if (evento.Fecha < DateTime.Now)
                {
                    ModelState.AddModelError(nameof(inscripcion.EventoId), "No se puede inscribir: el evento ya pasó.");
                }

                if (await _inscripciones.YaInscritoAsync(evento.Id, inscripcion.AsistenteId))
                {
                    ModelState.AddModelError(nameof(inscripcion.AsistenteId), "El asistente ya está inscrito (Confirmada) en este evento.");
                }

                var confirmadas = await _inscripciones.ContarConfirmadasAsync(evento.Id);
                if (confirmadas >= evento.Aforo)
                {
                    ModelState.AddModelError(nameof(inscripcion.EventoId), "El aforo del evento está completo.");
                }
            }

            if (!await _inscripciones.AsistenteActivoExisteAsync(inscripcion.AsistenteId))
            {
                ModelState.AddModelError(nameof(inscripcion.AsistenteId), "El asistente seleccionado no existe o no está activo.");
            }
        }

        if (ModelState.IsValid)
        {
            await _inscripciones.CrearAsync(inscripcion);
            TempData["Mensaje"] = "La inscripción se registró correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync(inscripcion.EventoId, inscripcion.AsistenteId);
        return View(inscripcion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var cancelada = await _inscripciones.CancelarAsync(id);
        if (cancelada)
        {
            TempData["Mensaje"] = "La inscripción fue cancelada.";
        }
        else
        {
            TempData["Error"] = "No se pudo cancelar: la inscripción no existe o ya estaba cancelada.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(int? eventoId = null, int? asistenteId = null)
    {
        var eventos = await _inscripciones.ListarEventosActivosAsync();
        var asistentes = await _inscripciones.ListarAsistentesActivosAsync();

        ViewBag.Eventos = new SelectList(
            eventos.Select(e => new { e.Id, Texto = $"{e.Titulo} - {e.Fecha:dd/MM/yyyy HH:mm}" }),
            "Id", "Texto", eventoId);

        ViewBag.Asistentes = new SelectList(
            asistentes.Select(a => new { a.Id, Texto = $"{a.Nombres} ({a.Dni})" }),
            "Id", "Texto", asistenteId);
    }
}
