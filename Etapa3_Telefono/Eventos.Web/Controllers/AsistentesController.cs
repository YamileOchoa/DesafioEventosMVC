using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _asistentes;

    public AsistentesController(AsistenteRepositorio asistentes)
    {
        _asistentes = asistentes;
    }

    public async Task<IActionResult> Index()
    {
        var lista = await _asistentes.ListarActivosAsync();
        return View(lista);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Asistente());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombres,Dni,Correo,Telefono")] Asistente asistente)
    {
        if (ModelState.IsValid)
        {
            if (await _asistentes.ExisteDniAsync(asistente.Dni))
            {
                ModelState.AddModelError(nameof(asistente.Dni), "Ya existe un asistente registrado con ese DNI.");
            }

            if (await _asistentes.ExisteCorreoAsync(asistente.Correo))
            {
                ModelState.AddModelError(nameof(asistente.Correo), "Ya existe un asistente registrado con ese correo.");
            }
        }

        if (ModelState.IsValid)
        {
            await _asistentes.CrearAsync(asistente);
            TempData["Mensaje"] = "El asistente se registró correctamente.";
            return RedirectToAction(nameof(Index));
        }

        return View(asistente);
    }
}
