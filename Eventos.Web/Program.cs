using System.Globalization;
using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

// Cultura es-PE: fechas dd/MM/yyyy y punto decimal
var cultura = new CultureInfo("es-PE");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosDB")));

builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Datos iniciales: se ejecutan DESPUES de aplicar las migraciones y solo si la tabla esta vacia
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventosContext>();

    await db.Database.MigrateAsync();

    if (!await db.Categorias.AnyAsync())
    {
        db.Categorias.AddRange(
            new Categoria { Nombre = "Conferencia" },
            new Categoria { Nombre = "Taller" },
            new Categoria { Nombre = "Networking" });
        await db.SaveChangesAsync();
    }

    if (!await db.Eventos.AnyAsync())
    {
        var cats = await db.Categorias.ToDictionaryAsync(c => c.Nombre, c => c.Id);
        var baseFecha = DateTime.Today;

        db.Eventos.AddRange(
            new Evento
            {
                Titulo = "Congreso de Tecnología 2026",
                Fecha = baseFecha.AddDays(30).AddHours(9),
                Lugar = "Centro de Convenciones de Lima",
                Aforo = 300,
                Precio = 120.00m,
                CategoriaId = cats["Conferencia"]
            },
            new Evento
            {
                Titulo = "Taller de ASP.NET Core y EF Core",
                Fecha = baseFecha.AddDays(14).AddHours(15),
                Lugar = "Laboratorio 3 - Campus Central",
                Aforo = 30,
                Precio = 50.00m,
                CategoriaId = cats["Taller"]
            },
            new Evento
            {
                Titulo = "Rueda de Networking Emprendedor",
                Fecha = baseFecha.AddDays(21).AddHours(19),
                Lugar = "Hotel Miraflores Park",
                Aforo = 80,
                Precio = 35.00m,
                CategoriaId = cats["Networking"]
            },
            new Evento
            {
                Titulo = "Webinar de Inteligencia Artificial",
                Fecha = baseFecha.AddDays(7).AddHours(18),
                Lugar = "Zoom",
                Aforo = 500,
                Precio = 0m,
                CategoriaId = cats["Conferencia"]
            });
        await db.SaveChangesAsync();
    }

    if (!await db.Asistentes.AnyAsync())
    {
        db.Asistentes.AddRange(
            new Asistente { Nombres = "Ana Torres Rojas", Dni = "70123456", Correo = "ana.torres@correo.com" },
            new Asistente { Nombres = "Luis Mendoza Paredes", Dni = "71234567", Correo = "luis.mendoza@correo.com" },
            new Asistente { Nombres = "María Quispe Huamán", Dni = "72345678", Correo = "maria.quispe@correo.com" },
            new Asistente { Nombres = "Carlos Salazar Vega", Dni = "73456789", Correo = "carlos.salazar@correo.com" },
            new Asistente { Nombres = "Lucía Fernández Díaz", Dni = "74567890", Correo = "lucia.fernandez@correo.com" });
        await db.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}");

app.Run();
