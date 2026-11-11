using System;
using System.Collections.Generic;
using Gimnasio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Models;

public partial class GimnasioContext : DbContext
{
    public GimnasioContext(DbContextOptions<GimnasioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Membresia> Membresias { get; set; }

    public virtual DbSet<Socio> Socios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Membresia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Membresi__3214EC072EB8D110");

            entity.ToTable("Membresia");

            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.Nombre).HasMaxLength(80);
            entity.Property(e => e.PrecioMensual).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Socio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Socio__3214EC07625EF527");

            entity.ToTable("Socio");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Dni).HasMaxLength(8);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FechaInscripcion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NombreCompleto).HasMaxLength(120);
            entity.Property(e => e.Telefono).HasMaxLength(15);

            entity.HasOne(d => d.Membresia).WithMany(p => p.Socios)
                .HasForeignKey(d => d.MembresiaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Socio_Membresia");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
