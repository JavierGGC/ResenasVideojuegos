using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ReseñasVideojuegos.Models
{
    public partial class resenajuegosContext : DbContext
    {
        public resenajuegosContext()
        {
        }

        public resenajuegosContext(DbContextOptions<resenajuegosContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrador> Administradors { get; set; }
        public virtual DbSet<Resena> Resenas { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4");

            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.HasKey(e => e.IdAdmin)
                    .HasName("PRIMARY");

                entity.ToTable("administrador");

                entity.Property(e => e.Contrasena)
                    .IsRequired()
                    .HasMaxLength(45);

                entity.Property(e => e.Nombreadmin)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Resena>(entity =>
            {
                entity.HasKey(e => e.IdResenas)
                    .HasName("PRIMARY");

                entity.ToTable("resenas");

                entity.Property(e => e.Compañia)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Estado).HasColumnType("bit(1)");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario)
                    .HasName("PRIMARY");

                entity.ToTable("usuario");

                entity.Property(e => e.Contrasena)
                    .IsRequired()
                    .HasColumnType("tinytext");

                entity.Property(e => e.Estado).HasColumnType("bit(1)");

                entity.Property(e => e.Nombreusuario)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
