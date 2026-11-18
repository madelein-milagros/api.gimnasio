# 🏋️‍♀️ Gimnasio API — Arquitectura en Capas con DTOs, AutoMapper y Autenticación JWT

**Curso:** Desarrollo de Aplicaciones Empresariales Avanzadas — Semanas 13 y 14  
**Docente:** Edwin William Arévalo Sermeño  
**Tecnologías:** ASP.NET Core Web API (.NET 9), Entity Framework Core (Database First / Scaffold), AutoMapper, Autenticación JWT Bearer, BCrypt.Net-Next, SQL Server, OpenAPI / Swagger UI.

---

## 📌 Descripción del Proyecto

Esta Web API REST gestiona los **Planes de Membresía (`Membresia`)**, el registro de **Socios (`Socio`)** y la autenticación/autorización de **Usuarios (`Usuario`)** de un gimnasio bajo una arquitectura empresarial dividida en **cuatro capas** (`Entidades`, `Datos`, `Negocio` y `Api`).

- **Enfoque Database First (Scaffold):** La base de datos `GimnasioDb` se construye mediante el script [`script_gimnasio.sql`](./script_gimnasio.sql).
- **Exposición exclusiva de DTOs:** Los controladores nunca reciben ni devuelven entidades de base de datos (`Membresia`, `Socio`, `Usuario`), sino únicamente **DTOs**, eliminando los ciclos de serialización JSON sin necesidad de usar `[JsonIgnore]`.
- **Autenticación JWT y Hashing BCrypt (Semana 14):** Registro de usuarios con contraseña hasheada mediante **BCrypt**, inicio de sesión con emisión de token **JWT** firmado con HMAC-SHA256 (con claims `sub`, `email` y `rol`), y autorización basada en roles propios del negocio (**`Recepcionista`** y **`Gerente`**).

---

## 🔐 Roles del Negocio y Matriz de Permisos (Semana 14)

- **`Recepcionista`** *(Rol asignado por defecto en el servidor al registrarse)*: Atiende e inscribe socios, actualiza datos de socios y consulta membresías. No puede crear, modificar ni eliminar planes de membresía, ni eliminar socios (`403 Forbidden`).
- **`Gerente`**: Administra los planes de membresía (`POST`, `PUT`, `DELETE /api/membresias`) y tiene autorización para eliminar registros de socios (`DELETE /api/socios/{id}`).

| Endpoint | Sin token | Rol Recepcionista | Rol Gerente |
| :--- | :---: | :---: | :---: |
| `POST /api/auth/registro` | Sí | Sí | Sí |
| `POST /api/auth/login` | Sí | Sí | Sí |
| `GET /api/auth/perfil` | No (401) | Sí | Sí |
| `GET /api/membresias` | Sí | Sí | Sí |
| `GET /api/membresias/{id}` | Sí | Sí | Sí |
| `GET /api/membresias/{id}/socios` | No (401) | Sí | Sí |
| `POST /api/membresias` | No (401) | No (403) | Sí |
| `PUT /api/membresias/{id}` | No (401) | No (403) | Sí |
| `DELETE /api/membresias/{id}` | No (401) | No (403) | Sí |
| `GET /api/socios` | No (401) | Sí | Sí |
| `GET /api/socios/filtrar` | No (401) | Sí | Sí |
| `GET /api/socios/{id}` | No (401) | Sí | Sí |
| `POST /api/socios` | No (401) | Sí | Sí |
| `PUT /api/socios/{id}` | No (401) | Sí | Sí |
| `DELETE /api/socios/{id}` | No (401) | No (403) | Sí |

---

## 🏛️ Arquitectura en 4 Capas

Referencias en cadena: **`Gimnasio.Api` → `Gimnasio.Negocio` → `Gimnasio.Datos` → `Gimnasio.Entidades`**

```text
Gimnasio.sln
├── script_gimnasio.sql                  # Script SQL (Membresia, Socio y Usuario con roles Recepcionista y Gerente)
├── Gimnasio.Entidades/                  # Capa 1: Entidades del dominio y DTOs
│   ├── Membresia.cs
│   ├── Socio.cs
│   ├── Usuario.cs
│   └── DTOs/
│       ├── MembresiaDto.cs, MembresiaCrearDto.cs, MembresiaActualizarDto.cs
│       ├── SocioDto.cs, SocioCrearDto.cs, SocioActualizarDto.cs
│       └── RegistroDto.cs, LoginDto.cs, TokenDto.cs, PerfilDto.cs
├── Gimnasio.Datos/                      # Capa 2: DbContext y Repositorios asíncronos con LINQ
│   ├── Models/GimnasioContext.cs
│   └── Repositorios/
│       ├── IMembresiaRepositorio.cs / MembresiaRepositorio.cs
│       ├── ISocioRepositorio.cs / SocioRepositorio.cs
│       └── IUsuarioRepositorio.cs / UsuarioRepositorio.cs
├── Gimnasio.Negocio/                    # Capa 3: Servicios, Reglas de Negocio, BCrypt, JWT y AutoMapper
│   ├── Excepciones/ReglaNegocioException.cs
│   ├── Perfiles/MappingProfile.cs
│   └── Servicios/
│       ├── IMembresiaServicio.cs / MembresiaServicio.cs
│       ├── ISocioServicio.cs / SocioServicio.cs
│       └── IAuthServicio.cs / AuthServicio.cs
└── Gimnasio.Api/                        # Capa 4: Controladores REST, JWT Bearer y Swagger con Authorize
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── MembresiasController.cs
    │   └── SociosController.cs
    ├── Gimnasio.Api.http
    └── Program.cs
```

---

## 👤 Usuarios de Prueba en el Script SQL

| Correo | Clave | Rol |
| :--- | :--- | :--- |
| `recepcion@gimnasio.com` | `Gym123456!` | `Recepcionista` |
| `gerente@gimnasio.com` | `Gym123456!` | `Gerente` |

---

## 🚀 Cómo Ejecutar el Proyecto Localmente

1. **Base de datos:** Ejecuta [`script_gimnasio.sql`](./script_gimnasio.sql) en SQL Server.
2. **Configuración local (`Gimnasio.Api/appsettings.Development.json`, excluido del repositorio):**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=TU_SERVIDOR\\SQLEXPRESS;Database=GimnasioDb;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "Jwt": {
       "Key": "ClaveSecretaSuperSeguraGimnasioApi2026_Minimo32Caracteres!",
       "Issuer": "Gimnasio.Api",
       "Audience": "Gimnasio.Clientes",
       "ExpireMinutes": 60
     }
   }
   ```
3. **Ejecutar la API:**
   ```bash
   dotnet run --project Gimnasio.Api
   ```
4. **Probar en Swagger UI:** Abre `http://localhost:5046/swagger`, haz login en `POST /api/Auth/login`, copia el `token` y pégalo en el botón **Authorize** (arriba a la derecha).
