using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private static readonly string[] Modalidades = { "Presencial", "Virtual" };

    private readonly EventoRepositorio _eventos;

    public EventosController(EventoRepositorio eventos)
    {
        _eventos = eventos;
    }

    public async Task<IActionResult> Index(int? categoriaId)
    {
        ViewBag.Categorias = new SelectList(await _eventos.ListarCategoriasActivasAsync(), "Id", "Nombre", categoriaId);
        var lista = await _eventos.ListarActivosAsync(categoriaId);
        return View(lista);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var evento = await _eventos.ObtenerAsync(id.Value);
        if (evento == null) return NotFound();

        return View(evento);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Evento { Fecha = DateTime.Today.AddDays(7).AddHours(18) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        if (!await _eventos.CategoriaActivaExisteAsync(evento.CategoriaId))
        {
            ModelState.AddModelError(nameof(evento.CategoriaId), "Seleccione una categoría válida.");
        }

        if (ModelState.IsValid)
        {
            await _eventos.CrearAsync(evento);
            TempData["Mensaje"] = "El evento se registró correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync(evento.CategoriaId);
        return View(evento);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var evento = await _eventos.ObtenerAsync(id.Value);
        if (evento == null) return NotFound();

        await CargarListasAsync(evento.CategoriaId);
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        if (id != evento.Id) return NotFound();

        if (!await _eventos.CategoriaActivaExisteAsync(evento.CategoriaId))
        {
            ModelState.AddModelError(nameof(evento.CategoriaId), "Seleccione una categoría válida.");
        }

        if (ModelState.IsValid)
        {
            var actualizado = await _eventos.ActualizarAsync(evento);
            if (!actualizado) return NotFound();

            TempData["Mensaje"] = "El evento se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync(evento.CategoriaId);
        return View(evento);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var evento = await _eventos.ObtenerAsync(id.Value);
        if (evento == null) return NotFound();

        return View(evento);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eliminado = await _eventos.DesactivarAsync(id);
        if (!eliminado) return NotFound();

        TempData["Mensaje"] = "El evento se eliminó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(int? categoriaId = null, string? modalidad = null)
    {
        ViewBag.Categorias = new SelectList(await _eventos.ListarCategoriasActivasAsync(), "Id", "Nombre", categoriaId);
        ViewBag.Modalidades = new SelectList(Modalidades, modalidad);
    }
}
