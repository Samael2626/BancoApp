using BancoApp.Interfaces;

namespace BancoApp.Models;

public abstract class Cuenta : ITransaction, ITransferible
{
    private static int _idCounter = 1;

    protected string NumeroCuenta { get; set; }
    protected decimal Saldo { get; set; }
    protected DateTime FechaApertura { get; set; }
    protected EstadoCuenta Estado { get; set; }
    protected List<Movimiento> Movimientos { get; set; } = new();

    public string NumeroCuentaPublico => NumeroCuenta;
    public EstadoCuenta EstadoPublico => Estado;

    protected Cuenta(string numeroCuenta, decimal saldoInicial = 0)
    {
        NumeroCuenta = numeroCuenta;
        Saldo = saldoInicial;
        FechaApertura = DateTime.Now;
        Estado = EstadoCuenta.ACTIVA;
    }

    public decimal ConsultarSaldo() => Saldo;
    public List<Movimiento> ObtenerMovimientos() => new(Movimientos);

    public virtual void Consignar(decimal monto)
    {
        ValidarEstado();
        if (monto <= 0) throw new ArgumentException("Monto debe ser positivo.");
        Saldo += monto;
        RegistrarMovimiento(new Movimiento
        {
            Id = _idCounter++,
            Tipo = TipoMovimiento.CONSIGNACION,
            Valor = monto,
            SaldoPosterior = Saldo,
            Descripcion = $"Consignación ${monto:N2}"
        });
    }

    public virtual void Retirar(decimal monto)
    {
        ValidarEstado();
        if (monto <= 0) throw new ArgumentException("Monto debe ser positivo.");
        if (monto > Saldo) throw new InvalidOperationException("Saldo insuficiente.");
        Saldo -= monto;
        RegistrarMovimiento(new Movimiento
        {
            Id = _idCounter++,
            Tipo = TipoMovimiento.RETIRO,
            Valor = monto,
            SaldoPosterior = Saldo,
            Descripcion = $"Retiro ${monto:N2}"
        });
    }

    public virtual void Transferir(Cuenta destino, decimal monto)
    {
        ValidarEstado();
        if (!ValidarDestino(destino)) throw new InvalidOperationException("Destino inválido.");
        if (monto <= 0) throw new ArgumentException("Monto debe ser positivo.");
        if (monto > Saldo) throw new InvalidOperationException("Saldo insuficiente.");

        Saldo -= monto;
        RegistrarMovimiento(new Movimiento
        {
            Id = _idCounter++,
            Tipo = TipoMovimiento.TRANSFERENCIA_OUT,
            Valor = monto,
            SaldoPosterior = Saldo,
            Descripcion = $"Transferencia a {destino.NumeroCuenta} ${monto:N2}"
        });

        destino.Saldo += monto;
        destino.RegistrarMovimiento(new Movimiento
        {
            Id = _idCounter++,
            Tipo = TipoMovimiento.TRANSFERENCIA_IN,
            Valor = monto,
            SaldoPosterior = destino.Saldo,
            Descripcion = $"Transferencia desde {NumeroCuenta} ${monto:N2}"
        });
    }

    public virtual bool ValidarDestino(Cuenta c) =>
        c != this && c.Estado == EstadoCuenta.ACTIVA;

    protected void RegistrarMovimiento(Movimiento m) => Movimientos.Add(m);

    protected void ValidarEstado()
    {
        if (Estado != EstadoCuenta.ACTIVA)
            throw new InvalidOperationException($"Cuenta {Estado}.");
    }
}
