# 🏋️‍♀️ Gimnasio API — Arquitectura en Capas con DTOs y AutoMapper
---

## 📌 Descripción del Proyecto

Esta Web API REST gestiona los **Planes de Membresía (`Membresia`)** y el registro de **Socios (`Socio`)** de un gimnasio bajo una arquitectura empresarial dividida en **cuatro capas** (`Entidades`, `Datos`, `Negocio` y `Api`).

- **Enfoque Database First (Scaffold):** La base de datos `GimnasioDb` se construye mediante el script [`script_gimnasio.sql`](./script_gimnasio.sql) y el modelo se genera con `dotnet ef dbcontext scaffold`.
- **Exposición exclusiva de DTOs:** Los controladores nunca reciben ni devuelven entidades de base de datos (`Membresia`, `Socio`), sino únicamente **DTOs**, eliminando los ciclos de serialización JSON sin necesidad de usar `[JsonIgnore]`.
- **Conversión con AutoMapper:** Un único [`MappingProfile`](./Gimnasio.Negocio/Perfiles/MappingProfile.cs) en la capa de Negocio gestiona todas las conversiones entre entidades y DTOs (incluyendo proyecciones con `ForMember` y `MapFrom`).

---

## 🏛️ Arquitectura en 4 Capas

Referencias en cadena: **`Gimnasio.Api` → `Gimnasio.Negocio` → `Gimnasio.Datos` → `Gimnasio.Entidades`**

```text
Gimnasio.sln
├── script_gimnasio.sql                  # Script SQL de creación y datos de prueba (Database First)
├── Gimnasio.Entidades/                  # Capa 1: Entidades del dominio y DTOs (lectura, crear, actualizar)
│   ├── Membresia.cs
│   ├── Socio.cs
│   └── DTOs/
│       ├── MembresiaDto.cs
│       ├── MembresiaCrearDto.cs
│       ├── MembresiaActualizarDto.cs
│       ├── SocioDto.cs
│       ├── SocioCrearDto.cs
│       └── SocioActualizarDto.cs
├── Gimnasio.Datos/                      # Capa 2: DbContext y Repositorios asíncronos con LINQ
│   ├── Models/GimnasioContext.cs
│   └── Repositorios/
│       ├── IMembresiaRepositorio.cs
│       ├── MembresiaRepositorio.cs
│       ├── ISocioRepositorio.cs
│       └── SocioRepositorio.cs
├── Gimnasio.Negocio/                    # Capa 3: Servicios, Reglas de Negocio y AutoMapper Profile
│   ├── Excepciones/ReglaNegocioException.cs
│   ├── Perfiles/MappingProfile.cs
│   └── Servicios/
│       ├── IMembresiaServicio.cs
│       ├── MembresiaServicio.cs
│       ├── ISocioServicio.cs
│       └── SocioServicio.cs
└── Gimnasio.Api/                        # Capa 4: Controladores REST, Inyección de Dependencias y Swagger
    ├── Controllers/
    │   ├── MembresiasController.cs
    │   └── SociosController.cs
    ├── Gimnasio.Api.http
    └── Program.cs
```

---

## 🔗 Modelo de Datos (Relación 1:N)

- **`Membresia` (1)** ↔ **`Socio` (N)**: Una membresía puede tener múltiples socios inscritos, mientras que cada socio tiene asignada una única membresía mediante la clave foránea `MembresiaId`.

---

## ⚖️ Reglas de Negocio Implementadas (`409 Conflict`)

Las reglas de negocio se validan en la capa `Gimnasio.Negocio` y lanzan `ReglaNegocioException`, que los controladores traducen en **`409 Conflict`**:

1. **Nombre de membresía único:** No se permite registrar ni actualizar una membresía con un nombre que ya existe.
2. **Protección al eliminar membresía:** No se permite eliminar una membresía que tiene socios registrados asociados.
3. **Edad mínima del socio:** Todo socio debe tener al menos **14 años** cumplidos para inscribirse en el gimnasio.
4. **DNI único del socio:** No se permite registrar ni actualizar un socio con un DNI que ya pertenezca a otro socio.
5. **Membresía válida:** Al crear o actualizar un socio, el `MembresiaId` indicado debe existir en la base de datos.

---

## 🔄 Sustentación del `MappingProfile` (AutoMapper)

| Mapeo | Origen → Destino | ¿Por qué en ese sentido? | Configuración especial |
| :--- | :--- | :--- | :--- |
| `Membresia → MembresiaDto` | Entidad → DTO de lectura | Es lo que la API devuelve al cliente; la entidad no debe salir de la API. | `ForMember`: `TotalSocios` toma `src.Socios.Count`. |
| `MembresiaCrearDto → Membresia` | DTO de entrada → Entidad | Convierte el cuerpo del `POST` en entidad para guardarla en la base de datos; no se devuelve. | Ninguna: los nombres de las propiedades coinciden. |
| `MembresiaActualizarDto → Membresia` | DTO de entrada → Entidad | Vuelca los datos del `PUT` sobre la entidad existente con `Map(dto, existente)`. | Ninguna: los nombres de las propiedades coinciden. |
| `Socio → SocioDto` | Entidad → DTO de lectura | Devuelve los datos del socio aplanando la relación con `Membresia` para evitar ciclos JSON. | `ForMember`: `Membresia` toma `src.Membresia.Nombre` y `PrecioMensual` toma `src.Membresia.PrecioMensual`. |
| `SocioCrearDto → Socio` | DTO de entrada → Entidad | Convierte los datos de entrada del `POST` en una entidad `Socio` para persistirla. | Ninguna: los nombres coinciden. |
| `SocioActualizarDto → Socio` | DTO de entrada → Entidad | Actualiza la entidad existente en el `PUT` mediante `Map(dto, existente)`. | Ninguna: los nombres coinciden. |

---

## 🌐 Endpoints Disponibles (12 Endpoints)

### Membresías (`/api/membresias`)
| Método | Ruta | Descripción | Respuestas |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/membresias` | Lista todas las membresías | `200 OK` |
| `GET` | `/api/membresias/{id}` | Obtiene una membresía por ID | `200 OK`, `404 NotFound` |
| `GET` | `/api/membresias/{id}/socios` | **Ruta anidada:** Lista los socios de una membresía | `200 OK`, `404 NotFound` |
| `POST` | `/api/membresias` | Crea una nueva membresía | `201 Created`, `400 BadRequest`, `409 Conflict` |
| `PUT` | `/api/membresias/{id}` | Actualiza una membresía existente | `204 NoContent`, `400 BadRequest`, `404 NotFound`, `409 Conflict` |
| `DELETE` | `/api/membresias/{id}` | Elimina una membresía sin socios | `204 NoContent`, `404 NotFound`, `409 Conflict` |

### Socios (`/api/socios`)
| Método | Ruta | Descripción | Respuestas |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/socios` | Lista todos los socios | `200 OK` |
| `GET` | `/api/socios/filtrar?nombre=&membresiaId=&activo=` | **Filtro por Query String** | `200 OK` |
| `GET` | `/api/socios/{id}` | Obtiene un socio por ID | `200 OK`, `404 NotFound` |
| `POST` | `/api/socios` | Registra un nuevo socio | `201 Created`, `400 BadRequest`, `409 Conflict` |
| `PUT` | `/api/socios/{id}` | Actualiza un socio existente | `204 NoContent`, `400 BadRequest`, `404 NotFound`, `409 Conflict` |
| `DELETE` | `/api/socios/{id}` | Elimina un socio por ID | `204 NoContent`, `404 NotFound` |

---

## 🚀 Cómo Ejecutar el Proyecto Localmente

1. **Base de datos:** Ejecuta el script [`script_gimnasio.sql`](./script_gimnasio.sql) en tu instancia de SQL Server.
2. **Cadena de conexión:** Configura tu cadena de conexión local en `Gimnasio.Api/appsettings.Development.json` (excluido del repositorio por seguridad):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=TU_SERVIDOR\\SQLEXPRESS;Database=GimnasioDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```
3. **Ejecutar la API:**
   ```bash
   dotnet run --project Gimnasio.Api
   ```
4. **Probar en Swagger UI:** Abre `http://localhost:5046/swagger` en tu navegador.
