namespace Eventos.Web.ViewModels;

public class EventoListadoItem
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public string Modalidad { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Aforo { get; set; }
    public int Inscritos { get; set; }

    public int CuposDisponibles => Math.Max(0, Aforo - Inscritos);
}
