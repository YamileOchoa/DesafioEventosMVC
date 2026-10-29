using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio
{
    private const string Confirmada = "Confirmada";
    private const string Cancelada = "Cancelada";

    private readonly EventosContext _context;

    public InscripcionRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Inscripcion>> ListarAsync()
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .OrderByDescending(i => i.FechaInscripcion)
            .ToListAsync();
    }

    // Datos de las listas desplegables
    public async Task<List<Evento>> ListarEventosActivosAsync()
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<List<Asistente>> ListarAsistentesActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    // Reglas de negocio
    public async Task<Evento?> ObtenerEventoActivoAsync(int eventoId)
    {
        return await _context.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventoId && e.Activo);
    }

    public async Task<bool> AsistenteActivoExisteAsync(int asistenteId)
    {
        return await _context.Asistentes.AnyAsync(a => a.Id == asistenteId && a.Activo);
    }

    public async Task<bool> YaInscritoAsync(int eventoId, int asistenteId)
    {
        return await _context.Inscripciones.AnyAsync(i =>
            i.EventoId == eventoId && i.AsistenteId == asistenteId && i.Estado == Confirmada);
    }

    public async Task<int> ContarConfirmadasAsync(int eventoId)
    {
        return await _context.Inscripciones.CountAsync(i =>
            i.EventoId == eventoId && i.Estado == Confirmada);
    }

    public async Task CrearAsync(Inscripcion inscripcion)
    {
        inscripcion.FechaInscripcion = DateTime.Now;
        inscripcion.Estado = Confirmada;
        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();
    }

    // Cancelar = cambiar el Estado (no se borra)
    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await _context.Inscripciones
            .FirstOrDefaultAsync(i => i.Id == id && i.Estado == Confirmada);
        if (inscripcion == null)
        {
            return false;
        }

        inscripcion.Estado = Cancelada;
        await _context.SaveChangesAsync();
        return true;
    }
}
