# 🎟️ Eventos e Inscripciones

### EF Core Code First · ASP.NET Core MVC · SQL Server · SEMANA 11

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-EventosDB-CC2927?logo=microsoftsqlserver&logoColor=white)
![Editor](https://img.shields.io/badge/Editor-VS%20Code-007ACC?logo=visualstudiocode&logoColor=white)

Aplicación web para gestionar **eventos, asistentes e inscripciones**. La base de datos `EventosDB` se crea
a partir de las clases C# mediante **migraciones** (sin scripts SQL), y todo el acceso a datos se resuelve con **LINQ**.

---

## 📦 Contenido del zip

```
EventosSemana11/
├── Eventos.Web/            ← Proyecto en ETAPA 1 (listo para InitialCreate)
├── Etapa2_Modalidad/       ← Archivos a copiar ENCIMA para la migración 2
├── Etapa3_Telefono/        ← Archivos a copiar ENCIMA para la migración 3
└── README.md
```

> 💡 **¿Por qué por etapas?** Las 3 migraciones las genera `dotnet ef` en tu equipo (no vienen escritas a mano).
> Si el modelo ya trajera `Modalidad` y `Telefono`, `InitialCreate` los incluiría y no tendrías 3 migraciones reales.

---

## 🧰 Requisitos

| Requisito     | Detalle                                                                                       |
| ------------- | --------------------------------------------------------------------------------------------- |
| SDK           | **.NET 10** (`net10.0`)                                                                       |
| ORM           | Entity Framework Core **10.0** (`SqlServer` + `Design`)                                       |
| Base de datos | SQL Server (ajusta `Server=` en `appsettings.json`; para SQL Express: `Server=.\SQLEXPRESS;`) |
| Herramienta   | `dotnet-ef` (se instala una sola vez)                                                         |

```bash
dotnet tool install --global dotnet-ef
```

> ⚠️ **No ejecutes `dotnet run` antes de crear y aplicar `InitialCreate`.**
> El `Program.cs` carga los datos iniciales y necesita que las tablas ya existan.

---

## 🚀 Guía paso a paso

### 🟢 Etapa 1 · Migración `InitialCreate`

```bash
cd Eventos.Web
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
```

✅ Comprueba en SQL Server: base `EventosDB`, tablas `Categorias`, `Eventos`, `Asistentes`, `Inscripciones` y `__EFMigrationsHistory`.

### 🟡 Etapa 2 · Migración `AgregarModalidadEvento`

1. Copia el contenido de `Etapa2_Modalidad/Eventos.Web/` **sobre** tu carpeta `Eventos.Web/` (reemplazar archivos).
2. Genera la migración:

   ```bash
   dotnet ef migrations add AgregarModalidadEvento
   ```

3. **Antes de aplicarla**, abre `Migrations/<fecha>_AgregarModalidadEvento.cs` y cambia el `defaultValue` por `"Presencial"`:

   ```csharp
   migrationBuilder.AddColumn<string>(
       name: "Modalidad",
       table: "Eventos",
       type: "nvarchar(20)",
       maxLength: 20,
       nullable: false,
       defaultValue: "Presencial");   // ← antes era ""
   ```

4. Aplica:

   ```bash
   dotnet ef database update
   ```

### 🔵 Etapa 3 · Migración `AgregarTelefonoAsistente`

1. Copia el contenido de `Etapa3_Telefono/Eventos.Web/` **sobre** tu carpeta `Eventos.Web/`.
2. Ejecuta:

   ```bash
   dotnet ef migrations add AgregarTelefonoAsistente
   dotnet ef database update
   ```

---

## 🧾 Comandos de migración utilizados _(para adjuntar)_

```bash
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate
dotnet ef database update

dotnet ef migrations add AgregarModalidadEvento
dotnet ef database update

dotnet ef migrations add AgregarTelefonoAsistente
dotnet ef database update
```

> Si SQL Server rechaza el certificado, la cadena de conexión ya incluye `TrustServerCertificate=True`.

---

## 📸 Evidencia

En SQL Server (base `EventosDB`) ejecuta:

```sql
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```

Deben verse **3 filas** (la fecha/hora será la de tu equipo):

| MigrationId                               |
| ----------------------------------------- |
| `aaaaMMddHHmmss_InitialCreate`            |
| `aaaaMMddHHmmss_AgregarModalidadEvento`   |
| `aaaaMMddHHmmss_AgregarTelefonoAsistente` |

Captura también la carpeta **`Migrations` expandida**: los 3 archivos `.cs` (+ sus `.Designer.cs`) y `EventosContextModelSnapshot.cs`.

🚫 No borres, renombres ni edites migraciones (salvo el `defaultValue` de la etapa 2) y no modifiques la base con SQL.

---

## 🧱 Estructura del proyecto

| Carpeta         | Contenido                                                                                         |
| --------------- | ------------------------------------------------------------------------------------------------- |
| `Models/`       | `Categoria`, `Evento`, `Asistente`, `Inscripcion` (solo Data Annotations)                         |
| `Data/`         | `EventosContext` (un `DbSet` por entidad, sin Fluent API)                                         |
| `Repositorios/` | `EventoRepositorio`, `AsistenteRepositorio`, `InscripcionRepositorio` (LINQ + `SaveChangesAsync`) |
| `Controllers/`  | `EventosController`, `AsistentesController`, `InscripcionesController`                            |
| `Views/`        | Vistas fuertemente tipadas y `_Layout` con menú                                                   |
| `ViewModels/`   | `EventoListadoItem` (inscritos y cupos del listado)                                               |

## ✨ Funcionalidades

- 📅 **Eventos:** listado con filtro por categoría, inscritos confirmados y cupos disponibles; crear, editar, ver detalle y eliminar (lógico).
- 🙋 **Asistentes:** listado ordenado por nombres y registro con validación de DNI y correo duplicados.
- 📝 **Inscripciones:** valida evento no pasado, asistente no duplicado y aforo disponible; permite cancelar inscripciones confirmadas.
- 🔄 Patrón **Post/Redirect/Get** con mensajes mediante `TempData`.
- ⚡ Todo **async**, lecturas con `AsNoTracking()` y sin SQL escrito a mano.

---

## ▶️ Ejecutar la aplicación

```bash
dotnet run
```

Abre 👉 **http://localhost:5080**
