# SakilaApp

Aplicación web desarrollada con **ASP.NET Core 8.0 MVC** para la gestión de una base de datos de videoclub basada en el esquema **Sakila**. Permite administrar películas, actores, clientes, alquileres, inventario, tiendas y categorías a través de una interfaz web completa con autenticación de usuarios.

---

## Tabla de contenidos

- [Descripción general](#descripción-general)
- [Tecnologías utilizadas](#tecnologías-utilizadas)
- [Requisitos previos](#requisitos-previos)
- [Instalación y configuración](#instalación-y-configuración)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Módulos y funcionalidades](#módulos-y-funcionalidades)
- [Autenticación](#autenticación)
- [Base de datos](#base-de-datos)
- [Ejecución](#ejecución)

---

## Descripción general

SakilaApp es un sistema de gestión para un videoclub que utiliza la base de datos de ejemplo **Sakila** de MySQL (adaptada a SQL Server). La aplicación permite realizar operaciones CRUD sobre las principales entidades del negocio, con búsqueda, paginación y eliminación lógica donde corresponde.

El proyecto fue desarrollado como parte de la materia **Diseño Web para Integración de Tecnologías**.

---

## Tecnologías utilizadas

| Tecnología | Versión |
|---|---|
| ASP.NET Core MVC | 8.0 |
| Entity Framework Core | 8.0 |
| ASP.NET Core Identity | 8.0 |
| SQL Server Express | cualquier versión reciente |
| Bootstrap | 5.3 (CDN) |
| Bootstrap Icons | CDN |
| jQuery | incluido en wwwroot |

---

## Requisitos previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express instalado localmente con instancia `.\SQLEXPRESS`
- Base de datos `sakila` creada y con las tablas del esquema Sakila ya existentes

---

## Instalación y configuración

### 1. Clonar o descargar el proyecto

Colocar el proyecto en la ruta deseada. La ruta original de desarrollo es:

```
C:\Users\aefaj\OneDrive\Desktop\DES WEB PARA INTEG DE TECNOL\SakilaApp\SakilaApp
```

### 2. Cadena de conexión

En `appsettings.json` está configurada la conexión a SQL Server Express:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=sakila;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Ajustar si el nombre de la instancia o la base de datos es diferente.

### 3. Preparar la base de datos

Las tablas del esquema Sakila deben existir previamente en la base de datos. Luego aplicar las migraciones de Identity para crear las tablas de autenticación:

```powershell
dotnet ef database update
```

Esto aplica la migración `AddIdentity` que crea las tablas de ASP.NET Core Identity (`AspNetUsers`, `AspNetRoles`, etc.).

### 4. Columnas adicionales agregadas manualmente

Algunas columnas no se gestionan por migraciones EF Core para evitar conflictos con el esquema Sakila existente. Deben agregarse directamente en SQL Server:

```sql
-- Columna de eliminación lógica en películas
ALTER TABLE film ADD is_active BIT NOT NULL DEFAULT 1;

-- Columna de eliminación lógica en actores
ALTER TABLE actor ADD is_active BIT NOT NULL DEFAULT 1;
```

---

## Estructura del proyecto

```
SakilaApp/
├── Controllers/
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── ActorsController.cs
│   ├── CategoriesController.cs
│   ├── CustomersController.cs
│   ├── FilmsController.cs
│   ├── InventoriesController.cs
│   ├── RentalsController.cs
│   └── StoresController.cs
├── Models/
│   ├── SakilaContext.cs         (DbContext principal)
│   ├── Actor.cs
│   ├── Category.cs
│   ├── Customer.cs
│   ├── Film.cs
│   ├── FilmActor.cs
│   ├── Inventory.cs
│   ├── Rental.cs
│   ├── Store.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── ForgotPasswordViewModel.cs
│   ├── ResetPasswordViewModel.cs
│   └── ErrorViewModel.cs
├── Views/
│   ├── Shared/                  (Layout y parciales)
│   ├── Home/
│   ├── Account/                 (6 vistas de autenticación)
│   ├── Actors/                  (5 vistas CRUD)
│   ├── Categories/              (5 vistas CRUD)
│   ├── Customers/               (5 vistas CRUD)
│   ├── Films/                   (5 vistas CRUD)
│   ├── Inventories/             (5 vistas CRUD)
│   ├── Rentals/                 (5 vistas CRUD)
│   └── Stores/                  (5 vistas CRUD)
├── Services/
│   └── ConsoleEmailSender.cs
├── Migrations/
│   └── 20260513123426_AddIdentity.cs
├── wwwroot/
│   ├── css/site.css
│   ├── js/site.js
│   └── lib/                     (Bootstrap, jQuery)
├── Program.cs
├── appsettings.json
└── SakilaApp.csproj
```

---

## Módulos y funcionalidades

Todos los módulos incluyen: **listado con búsqueda, paginación (20 registros por página), creación, edición, detalles y eliminación.**

### Películas (`/Films`)

| Funcionalidad | Detalle |
|---|---|
| Listado | Búsqueda por título, calificación o año de lanzamiento |
| Paginación | 20 películas por página con controles de navegación |
| Eliminación | Lógica — marca `is_active = false` en lugar de borrar el registro |
| Campos | Título, descripción, año, duración, calificación, idioma |

### Actores (`/Actors`)

| Funcionalidad | Detalle |
|---|---|
| Listado | Búsqueda por nombre o apellido |
| Paginación | 20 actores por página |
| Eliminación | Lógica — marca `is_active = false` |
| Detalles | Muestra la lista de películas en las que actuó el actor |

### Categorías (`/Categories`)

| Funcionalidad | Detalle |
|---|---|
| Listado | Búsqueda por nombre |
| Paginación | 20 categorías por página |
| Eliminación | Física (DELETE directo) |

### Clientes (`/Customers`)

| Funcionalidad | Detalle |
|---|---|
| Listado | Búsqueda por nombre, apellido o correo electrónico |
| Paginación | 20 clientes por página |
| Eliminación | Lógica — marca el campo `Active` como inactivo |

### Inventario (`/Inventories`)

| Funcionalidad | Detalle |
|---|---|
| Listado | Búsqueda por ID de película o ID de tienda |
| Paginación | 20 registros por página |
| Eliminación | Física (DELETE directo) |

### Tiendas (`/Stores`)

| Funcionalidad | Detalle |
|---|---|
| Listado | Sin búsqueda (pocos registros) |
| Eliminación | Física (DELETE directo) |
| Campos | ID de tienda, gerente de staff, dirección |

### Alquileres (`/Rentals`)

| Funcionalidad | Detalle |
|---|---|
| Listado | Búsqueda por ID de cliente o ID de inventario |
| Paginación | 20 registros por página |
| Eliminación | Física (DELETE directo) |
| Campos | Fecha de alquiler, fecha de devolución, cliente, inventario, staff |

---

## Autenticación

La aplicación usa **ASP.NET Core Identity** con controlador propio (`AccountController`) en lugar de las Razor Pages predeterminadas de Identity.

| Ruta | Descripción |
|---|---|
| `/Account/Login` | Inicio de sesión |
| `/Account/Register` | Registro de nuevo usuario |
| `/Account/Logout` | Cierre de sesión |
| `/Account/ForgotPassword` | Solicitar restablecimiento de contraseña |
| `/Account/ResetPassword` | Restablecer contraseña con token |

> **Nota:** El envío de correos usa `ConsoleEmailSender`, que imprime el contenido del correo en la consola en lugar de enviarlo realmente. En producción se debe reemplazar por un servicio de email real (SendGrid, SMTP, etc.).

---

## Base de datos

### Esquema Sakila

La base de datos `sakila` es un esquema de ejemplo que representa un videoclub. Las tablas principales utilizadas en la aplicación son:

| Tabla | Descripción |
|---|---|
| `film` | Películas del catálogo |
| `actor` | Actores |
| `film_actor` | Relación muchos a muchos entre películas y actores |
| `category` | Categorías de películas |
| `customer` | Clientes del videoclub |
| `inventory` | Copias físicas de películas por tienda |
| `store` | Tiendas del videoclub |
| `rental` | Registro de alquileres |

### Columnas agregadas al esquema original

| Tabla | Columna | Tipo | Propósito |
|---|---|---|---|
| `film` | `is_active` | `BIT` | Eliminación lógica de películas |
| `actor` | `is_active` | `BIT` | Eliminación lógica de actores |

### Migraciones EF Core

Solo se gestionan via EF Core las tablas de Identity. Las tablas Sakila ya existentes no se tocan con migraciones para evitar conflictos de constraints.

| Migración | Descripción |
|---|---|
| `20260513123426_AddIdentity` | Crea las tablas de ASP.NET Core Identity |

---

## Ejecución

```powershell
dotnet run
```

La aplicación quedará disponible en: **http://localhost:5200**

> Siempre ejecutar con `dotnet run`, no con el ejecutable `.exe` directamente, para que las variables de entorno y configuración de desarrollo se apliquen correctamente.

---

## Notas de desarrollo

- El namespace del proyecto es `SakilaApp.*` en todos los archivos (Controllers, Models, Services, Migrations).
- Para agregar nuevas columnas a tablas existentes de Sakila, usar `sqlcmd` directamente en lugar de migraciones EF Core.
- Los campos nullable en la base de datos Sakila deben mapearse como tipos nullable en C# (`string?`, `DateTime?`) para evitar `SqlNullValueException` en tiempo de ejecución.
- La paginación usa `ViewData["Page"]`, `ViewData["TotalPages"]` y `ViewData["Total"]` con el patrón `Skip/Take` + `CountAsync`.
