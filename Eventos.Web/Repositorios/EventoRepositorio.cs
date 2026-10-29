using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio
{
    private readonly EventosContext _context;

    public EventoRepositorio(EventosContext context)
    {
        _context = context;
    }

    // Datos de las listas desplegables
    public async Task<List<Categoria>> ListarCategoriasActivasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<bool> CategoriaActivaExisteAsync(int categoriaId)
    {
        return await _context.Categorias.AnyAsync(c => c.Id == categoriaId && c.Activo);
    }

    // Index: eventos activos con categoria, filtro opcional y orden por fecha
    public async Task<List<EventoListadoItem>> ListarActivosAsync(int? categoriaId)
    {
        var consulta = _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo);

        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(e => e.CategoriaId == categoriaId.Value);
        }

        return await consulta
            .OrderBy(e => e.Fecha)
            .Select(e => new EventoListadoItem
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Categoria = e.Categoria!.Nombre,
                Fecha = e.Fecha,
                Lugar = e.Lugar,
                Modalidad = e.Modalidad,
                Precio = e.Precio,
                Aforo = e.Aforo,
                Inscritos = e.Inscripciones.Count(i => i.Estado == "Confirmada")
            })
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    }

    public async Task CrearAsync(Evento evento)
    {
        evento.Activo = true;
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(Evento datos)
    {
        var existente = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == datos.Id && e.Activo);
        if (existente == null)
        {
            return false;
        }

        existente.Titulo = datos.Titulo;
        existente.Fecha = datos.Fecha;
        existente.Lugar = datos.Lugar;
        existente.Aforo = datos.Aforo;
        existente.Precio = datos.Precio;
        existente.CategoriaId = datos.CategoriaId;
        existente.Modalidad = datos.Modalidad;

        await _context.SaveChangesAsync();
        return true;
    }

    // Eliminacion logica
    public async Task<bool> DesactivarAsync(int id)
    {
        var existente = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (existente == null)
        {
            return false;
        }

        existente.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
