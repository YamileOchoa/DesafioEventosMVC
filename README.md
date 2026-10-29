# Eventos e inscripciones – EF Core Code First (SEMANA 11)

Contenido del zip:

```
Eventos.Web/            -> Proyecto en ETAPA 1 (listo para InitialCreate)
Etapa2_Modalidad/       -> Archivos a copiar ENCIMA de Eventos.Web para la migración 2
Etapa3_Telefono/        -> Archivos a copiar ENCIMA de Eventos.Web para la migración 3
```

> Las 3 migraciones **las genera `dotnet ef`** en tu equipo (no vienen escritas a mano).
> Por eso el proyecto se entrega por etapas: si el modelo ya tuviera `Modalidad` y `Telefono`,
> la migración `InitialCreate` los incluiría y no tendrías 3 migraciones reales.

## Requisitos

- SDK de .NET 8 (si tienes otro, cambia `TargetFramework` y las versiones `8.0.*` de los paquetes en `Eventos.Web.csproj`, p. ej. `net9.0` y `9.0.*`).
- SQL Server (ajusta `Server=` en `appsettings.json` si usas SQL Express: `Server=.\SQLEXPRESS;`).
- Herramienta dotnet-ef (una sola vez):

```
dotnet tool install --global dotnet-ef
```

> IMPORTANTE: no ejecutes `dotnet run` antes de crear y aplicar `InitialCreate`
> (el `Program.cs` hace la carga de datos y necesita que las tablas ya existan).

---

## ETAPA 1 – Migración InitialCreate

```
cd Eventos.Web
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Comprueba en SQL Server: base `EventosDB`, tablas `Categorias`, `Eventos`, `Asistentes`, `Inscripciones` y `__EFMigrationsHistory`.

## ETAPA 2 – Migración AgregarModalidadEvento

1. Copia el contenido de `Etapa2_Modalidad/Eventos.Web/` **sobre** tu carpeta `Eventos.Web/` (reemplazar archivos).
2. Genera la migración:

```
dotnet ef migrations add AgregarModalidadEvento
```

3. **Antes de aplicarla**, abre el archivo `Migrations/<fecha>_AgregarModalidadEvento.cs` y en el `AddColumn` de `Modalidad` cambia el `defaultValue` por `"Presencial"`:

```csharp
migrationBuilder.AddColumn<string>(
    name: "Modalidad",
    table: "Eventos",
    type: "nvarchar(20)",
    maxLength: 20,
    nullable: false,
    defaultValue: "Presencial");   // <- antes era ""
```

4. Aplica:

```
dotnet ef database update
```

## ETAPA 3 – Migración AgregarTelefonoAsistente

1. Copia el contenido de `Etapa3_Telefono/Eventos.Web/` **sobre** tu carpeta `Eventos.Web/`.
2. Ejecuta:

```
dotnet ef migrations add AgregarTelefonoAsistente
dotnet ef database update
```

---

## Evidencia

En SQL Server (base `EventosDB`):

```sql
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```

Deben verse 3 filas, con este formato (la fecha/hora será la de tu equipo):

| MigrationId |
|---|
| 2026MMDDHHMMSS_InitialCreate |
| 2026MMDDHHMMSS_AgregarModalidadEvento |
| 2026MMDDHHMMSS_AgregarTelefonoAsistente |

Captura también la carpeta `Migrations` expandida: deben verse los 3 archivos `.cs` (+ sus `.Designer.cs`) y `EventosContextModelSnapshot.cs`.
No borres, renombres ni edites migraciones (salvo el `defaultValue` de la etapa 2), y no toques la base con SQL.

## Comandos de migración utilizados (para adjuntar)

```
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate
dotnet ef database update

dotnet ef migrations add AgregarModalidadEvento
dotnet ef database update

dotnet ef migrations add AgregarTelefonoAsistente
dotnet ef database update
```

(Si SQL Server rechaza el certificado, la cadena de conexión ya incluye `TrustServerCertificate=True`.)

## Ejecutar la aplicación

```
dotnet run
```

Abre http://localhost:5080
