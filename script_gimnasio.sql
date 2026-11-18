-- =========================================================================
-- Script SQL - Base de Datos GimnasioDb (Semana 13 y Semana 14 - JWT)
-- Curso: Desarrollo de Aplicaciones Empresariales Avanzadas
-- Tema: Gestión de Gimnasio (Membresías, Socios y Usuarios con Roles JWT)
-- =========================================================================

IF DB_ID('GimnasioDb') IS NOT NULL
BEGIN
    ALTER DATABASE GimnasioDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE GimnasioDb;
END
GO

CREATE DATABASE GimnasioDb;
GO

USE GimnasioDb;
GO

-- 1. Tabla Membresia (Lado "1")
CREATE TABLE Membresia (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(80) NOT NULL,
    Descripcion NVARCHAR(250) NOT NULL,
    PrecioMensual DECIMAL(10,2) NOT NULL,
    DuracionMeses INT NOT NULL
);
GO

-- 2. Tabla Socio (Lado "N")
CREATE TABLE Socio (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto NVARCHAR(120) NOT NULL,
    Dni NVARCHAR(8) NOT NULL,
    Telefono NVARCHAR(15) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Edad INT NOT NULL,
    FechaInscripcion DATETIME2 NOT NULL DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1,
    MembresiaId INT NOT NULL,
    CONSTRAINT FK_Socio_Membresia FOREIGN KEY (MembresiaId)
        REFERENCES Membresia(Id)
);
GO

-- 3. Tabla Usuario (Semana 14: Autenticación JWT y Roles del Negocio)
CREATE TABLE Usuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Correo NVARCHAR(100) NOT NULL,
    ClaveHash NVARCHAR(255) NOT NULL,
    Rol NVARCHAR(40) NOT NULL,
    CONSTRAINT UQ_Usuario_Correo UNIQUE (Correo)
);
GO

-- Datos de ejemplo para Membresia
INSERT INTO Membresia (Nombre, Descripcion, PrecioMensual, DuracionMeses) VALUES
(N'Plan Básico', N'Acceso a zona de máquinas y pesas libres de lunes a viernes', 89.90, 1),
(N'Plan Trimestral Fitness', N'Acceso total a máquinas, cardio y clases grupales por 3 meses', 79.90, 3),
(N'Plan VIP Anual', N'Acceso ilimitado todas las sedes, sauna, nutricionista y entrenador personal', 129.90, 12),
(N'Plan Estudiantil', N'Tarifa especial para estudiantes con acceso a zona de musculación y cardio', 65.00, 1);
GO

-- Datos de ejemplo para Socio
INSERT INTO Socio (NombreCompleto, Dni, Telefono, Email, Edad, FechaInscripcion, Activo, MembresiaId) VALUES
(N'Carlos Alberto Mendoza Ríos', N'71234561', N'987654321', N'carlos.mendoza@correo.com', 26, '2026-01-15T09:30:00', 1, 3),
(N'Lucía Fernanda Paredes Soto', N'74561238', N'912345678', N'lucia.paredes@correo.com', 22, '2026-02-10T14:15:00', 1, 4),
(N'Jorge Luis Vargas Quispe', N'45896321', N'956781234', N'jorge.vargas@correo.com', 34, '2026-03-01T18:00:00', 1, 1),
(N'Valeria Sofía Castro Salazar', N'73698521', N'945612378', N'valeria.castro@correo.com', 29, '2026-03-20T10:00:00', 1, 3),
(N'Diego Alejandro Torres Rojas', N'76543219', N'934567890', N'diego.torres@correo.com', 19, '2026-04-05T16:45:00', 0, 2);
GO

-- Usuarios de prueba (Semana 14) registrados con la clave en común: Gym123456!
-- Hash BCrypt generado a través del endpoint POST /api/auth/registro
INSERT INTO Usuario (Correo, ClaveHash, Rol) VALUES
(N'recepcion@gimnasio.com', N'$2a$11$VbIVDJ/RB/.aVTzxgdyZ9ekuC065N3Up93Q7OaOZMT71lf3Kao/4G', N'Recepcionista'),
(N'gerente@gimnasio.com',   N'$2a$11$VGGLnK8pVEb5oYiH60Bm2e9rz1RGPjuYMIKlpEq3OoTHQQb8JkaN6', N'Recepcionista');
GO

-- Cambio de rol con UPDATE para el usuario con rol Gerente (como indica la guía)
UPDATE Usuario
SET Rol = N'Gerente'
WHERE Correo = N'gerente@gimnasio.com';
GO
