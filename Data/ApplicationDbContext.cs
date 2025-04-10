using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backenddb_c.Models;

namespace backenddb_c.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bitacora> Bitacoras { get; set; }

    public virtual DbSet<BitacoraPago> BitacoraPagos { get; set; }

    public virtual DbSet<CajaAhorro> CajaAhorros { get; set; }

    public virtual DbSet<Cajero> Cajeros { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<InicioSesion> InicioSesions { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<Operacion> Operacions { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Tarjetum> Tarjeta { get; set; }

    public virtual DbSet<Titular> Titulars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //User Id=BANCARIO;Password=Alfredo+123;Data Source=localhost:1521/FREE
        => optionsBuilder.UseOracle("User Id=atm;Password=atm123;Data Source=localhost:1521/FREE");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("BANCARIO")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Bitacora>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BITACORA_PK"); // Nombre opcional para la PK
            entity
                .ToTable("BITACORA");

            entity.Property(e => e.FechaTransaccion)
                .HasColumnType("DATE")
                .HasColumnName("FECHA_TRANSACCION");
            entity.Property(e => e.LlavePrimaria)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("LLAVE_PRIMARIA");
            entity.Property(e => e.NomCampo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOM_CAMPO");
            entity.Property(e => e.NomTabla)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOM_TABLA");
            entity.Property(e => e.NuevoValor)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("NUEVO_VALOR");
            entity.Property(e => e.NumTransaccion)
                .HasPrecision(12)
                .HasColumnName("NUM_TRANSACCION");
            entity.Property(e => e.TipoTransaccion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TIPO_TRANSACCION");
            entity.Property(e => e.UsuarioTransaccion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("USUARIO_TRANSACCION");
            entity.Property(e => e.ValorAnterior)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("VALOR_ANTERIOR");
        });

        modelBuilder.Entity<BitacoraPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BITACORA_PAGO_PK");
            entity
                .ToTable("BITACORA_PAGO");

            entity.Property(e => e.CodigoPrestamo)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_PRESTAMO");
            entity.Property(e => e.FechaPago)
                .HasColumnType("DATE")
                .HasColumnName("FECHA_PAGO");
            entity.Property(e => e.FechaTransaccion)
                .HasColumnType("DATE")
                .HasColumnName("FECHA_TRANSACCION");
            entity.Property(e => e.MesesPendiente)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MESES_PENDIENTE");
            entity.Property(e => e.MontoPago)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("MONTO_PAGO");
            entity.Property(e => e.NumTransaccion)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("NUM_TRANSACCION");
            entity.Property(e => e.SaldoAnterior)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("SALDO_ANTERIOR");
            entity.Property(e => e.SaldoNuevo)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("SALDO_NUEVO");
            entity.Property(e => e.TipoTransaccion)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TIPO_TRANSACCION");
            entity.Property(e => e.UsuarioTransaccion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("USUARIO_TRANSACCION");
        });

        modelBuilder.Entity<CajaAhorro>(entity =>
        {
            entity.HasKey(e => e.CodigoCaja).HasName("CAJA_AHORRO_PK");
            entity.ToTable("CAJA_AHORRO");

            // Solo la clave primaria debe tener ValueGeneratedOnAdd
            entity.Property(e => e.CodigoCaja)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_CAJA");

            // Estas propiedades NO deben tener ValueGeneratedOnAdd
            entity.Property(e => e.CodigoCliente)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_CLIENTE");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");

            entity.Property(e => e.SaldoCaja)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("SALDO_CAJA");

            entity.HasOne(d => d.CodigoClienteNavigation)
                .WithMany(p => p.CajaAhorros)
                .HasForeignKey(d => d.CodigoCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("CAJA_AHORRO_CLIENTE_FK");
        });
        modelBuilder.Entity<Cajero>(entity =>
        {
            entity.HasKey(e => e.CodigoCajero).HasName("CAJERO_PK");

            entity.ToTable("CAJERO");

            entity.Property(e => e.CodigoCajero)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_CAJERO");
            entity.Property(e => e.Saldo)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("SALDO");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UBICACION");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.CodigoCliente).HasName("CLIENTE_PK");

            entity.ToTable("CLIENTE");

            entity.Property(e => e.CodigoCliente)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_CLIENTE");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DIRECCION");
            entity.Property(e => e.Edad)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("EDAD");
            entity.Property(e => e.PrimerApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PRIMER_APELLIDO");
            entity.Property(e => e.PrimerNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PRIMER_NOMBRE");
            entity.Property(e => e.SegundoApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SEGUNDO_APELLIDO");
            entity.Property(e => e.SegundoNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SEGUNDO_NOMBRE");
            entity.Property(e => e.TercerNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TERCER_NOMBRE");
        });

        modelBuilder.Entity<InicioSesion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("INICIO_SESION_PK");

            entity
                .ToTable("INICIO_SESION");

            entity.Property(e => e.CodigoCaja)
                .HasColumnType("NUMBER")
                .HasColumnName("CODIGO_CAJA");
            entity.Property(e => e.CodigoCliente)
                .HasColumnType("NUMBER")
                .HasColumnName("CODIGO_CLIENTE");
            entity.Property(e => e.CodigoTitular)
                .HasColumnType("NUMBER")
                .HasColumnName("CODIGO_TITULAR");
            entity.Property(e => e.FechaHora)
                .HasPrecision(6)
                .HasColumnName("FECHA_HORA");
            entity.Property(e => e.NumeroTarjeta)
                .HasColumnType("NUMBER")
                .HasColumnName("NUMERO_TARJETA");
            entity.Property(e => e.Secuencia)
                .HasColumnType("NUMBER")
                .HasColumnName("SECUENCIA");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.CodigoMovimiento).HasName("MOVIMIENTO_PK");

            entity.ToTable("MOVIMIENTO");

            entity.Property(e => e.CodigoMovimiento)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_MOVIMIENTO");
            entity.Property(e => e.CodigoCajero)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_CAJERO");
            entity.Property(e => e.CodigoTitular)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_TITULAR");
            entity.Property(e => e.CuentaAcreditar)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CUENTA_ACREDITAR");
            entity.Property(e => e.CuentaDebitar)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CUENTA_DEBITAR");
            entity.Property(e => e.Fecha)
                .HasColumnType("DATE")
                .HasColumnName("FECHA");
            entity.Property(e => e.Monto)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("MONTO");
            entity.Property(e => e.TipoOperacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TIPO_OPERACION");

            entity.HasOne(d => d.CodigoCajeroNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.CodigoCajero)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MOVIMIENTO_CAJERO_FK");

            entity.HasOne(d => d.CodigoTitularNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.CodigoTitular)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MOVIMIENTO_TITULAR_FK");
        });

        modelBuilder.Entity<Operacion>(entity =>
        {
            entity.HasKey(e => e.CodigoOperacion).HasName("OPERACION_PK");

            entity.ToTable("OPERACION");

            entity.Property(e => e.CodigoOperacion)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_OPERACION");
            entity.Property(e => e.CodigoCajero)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_CAJERO");
            entity.Property(e => e.NombreOperacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NOMBRE_OPERACION");

            entity.HasOne(d => d.CodigoCajeroNavigation).WithMany(p => p.Operacions)
                .HasForeignKey(d => d.CodigoCajero)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OPERACION_CAJERO_FK");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.NumeroPago).HasName("PAGO_PK");

            entity.ToTable("PAGO");

            entity.Property(e => e.NumeroPago)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("NUMERO_PAGO");
            entity.Property(e => e.CodigoPrestamo)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_PRESTAMO");
            entity.Property(e => e.FechaPago)
                .HasColumnType("DATE")
                .HasColumnName("FECHA_PAGO");
            entity.Property(e => e.MontoPago)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("MONTO_PAGO");

            entity.HasOne(d => d.CodigoPrestamoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.CodigoPrestamo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PAGO_PRESTAMO_FK");
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(e => e.CodigoPrestamo).HasName("PRESTAMO_PK");

            entity.ToTable("PRESTAMO");

            entity.Property(e => e.CodigoPrestamo)

                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_PRESTAMO");
            entity.Property(e => e.CodigoCliente)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_CLIENTE");
            entity.Property(e => e.EstadoPrestamo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ESTADO_PRESTAMO");
            entity.Property(e => e.FechaOtorgado)
                .HasColumnType("DATE")
                .HasColumnName("FECHA_OTORGADO");
            entity.Property(e => e.FechaVencimiento)
                .HasColumnType("DATE")
                .HasColumnName("FECHA_VENCIMIENTO");
            entity.Property(e => e.Interes)
                .HasColumnType("NUMBER(6,2)")
                .HasColumnName("INTERES");
            entity.Property(e => e.MesesPendiente)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("MESES_PENDIENTE");
            entity.Property(e => e.MontoInicial)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("MONTO_INICIAL");
            entity.Property(e => e.MontoPagado)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("MONTO_PAGADO");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("MONTO_TOTAL");
            entity.Property(e => e.SaldoPendiente)
                .HasColumnType("NUMBER(12,2)")
                .HasColumnName("SALDO_PENDIENTE");

            entity.HasOne(d => d.CodigoClienteNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.CodigoCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PRESTAMO_CLIENTE_FK");
        });

        modelBuilder.Entity<Tarjetum>(entity =>
        {
            entity.HasKey(e => e.NumeroTarjeta).HasName("TARJETA_PK");

            entity.ToTable("TARJETA");

            entity.HasIndex(e => e.CodigoTitular, "SYS_C008902").IsUnique();

            entity.Property(e => e.NumeroTarjeta)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("NUMERO_TARJETA");
            entity.Property(e => e.CodigoCaja)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_CAJA");
            entity.Property(e => e.CodigoTitular)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_TITULAR");
            entity.Property(e => e.FechaVencimiento)
                .HasColumnType("DATE")
                .HasColumnName("FECHA_VENCIMIENTO");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MARCA");
            entity.Property(e => e.Pin)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PIN");

            entity.HasOne(d => d.CodigoCajaNavigation).WithMany(p => p.Tarjeta)
                .HasForeignKey(d => d.CodigoCaja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TARJETA_CAJA_AHORRO_FK");

            entity.HasOne(d => d.CodigoTitularNavigation).WithOne(p => p.Tarjetum)
                .HasForeignKey<Tarjetum>(d => d.CodigoTitular)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TARJETA_TITULAR_FK");
        });

        modelBuilder.Entity<Titular>(entity =>
        {
            entity.HasKey(e => e.CodigoTitular).HasName("TITULAR_PK");

            entity.ToTable("TITULAR");

            entity.Property(e => e.CodigoTitular)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CODIGO_TITULAR");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DIRECCION");
            entity.Property(e => e.Edad)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("EDAD");
            entity.Property(e => e.PrimerApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PRIMER_APELLIDO");
            entity.Property(e => e.PrimerNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PRIMER_NOMBRE");
            entity.Property(e => e.SegundoApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SEGUNDO_APELLIDO");
            entity.Property(e => e.SegundoNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SEGUNDO_NOMBRE");
            entity.Property(e => e.TercerNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TERCER_NOMBRE");
        });
        modelBuilder.HasSequence("BITACORA_PAGO_SEQ");
        modelBuilder.HasSequence("INICIO_SESION_SECUENCIA");
        modelBuilder.HasSequence("SEQUENCE");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
