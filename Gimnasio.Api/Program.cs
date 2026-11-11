using Gimnasio.Datos.Models;
using Gimnasio.Datos.Repositorios;
using Gimnasio.Negocio.Perfiles;
using Gimnasio.Negocio.Servicios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Registro de DbContext (Capa de Datos)
builder.Services.AddDbContext<GimnasioContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registro de Repositorios (Capa de Datos)
builder.Services.AddScoped<IMembresiaRepositorio, MembresiaRepositorio>();
builder.Services.AddScoped<ISocioRepositorio, SocioRepositorio>();

// 3. Registro de Servicios (Capa de Negocio)
builder.Services.AddScoped<IMembresiaServicio, MembresiaServicio>();
builder.Services.AddScoped<ISocioServicio, SocioServicio>();

// 4. Registro de AutoMapper con el MappingProfile de la Capa de Negocio
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// 5. Controladores y OpenAPI + SwaggerUI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Gimnasio API v1");
    });
}

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
