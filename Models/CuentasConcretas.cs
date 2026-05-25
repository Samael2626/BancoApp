namespace BancoApp.Models;

public class CuentaAhorros : Cuenta
{
    public decimal TasaInteres { get; set; }

    public CuentaAhorros(string numeroCuenta, decimal saldoInicial = 0, decimal tasaInteres = 0.02m)
        : base(numeroCuenta, saldoInicial)
    {
        TasaInteres = tasaInteres;
    }

    public void AplicarIntereses()
    {
        var interes = Saldo * TasaInteres;
        Consignar(interes);
        Movimientos[^1].Descripcion = $"Intereses aplicados ({TasaInteres:P})";
    }

    public decimal CalcularIntereses() => Saldo * TasaInteres;
}

public class CuentaCorriente : Cuenta
{
    public decimal PorcentajeSobregiro { get; set; }
    public decimal LimiteSobregiro { get; set; }

    public CuentaCorriente(string numeroCuenta, decimal saldoInicial = 0,
        decimal limiteSobregiro = 500_000, decimal porcentajeSobregiro = 0.02m)
        : base(numeroCuenta, saldoInicial)
    {
        LimiteSobregiro = limiteSobregiro;
        PorcentajeSobregiro = porcentajeSobregiro;
    }

    public override void Retirar(decimal monto)
    {
        ValidarEstado();
        if (monto <= 0) throw new ArgumentException("Monto debe ser positivo.");
        decimal disponible = Saldo + LimiteSobregiro;
        if (monto > disponible) throw new InvalidOperationException("Excede límite de sobregiro.");
        Saldo -= monto;
        RegistrarMovimiento(new Movimiento
        {
            Tipo = TipoMovimiento.RETIRO,
            Valor = monto,
            SaldoPosterior = Saldo,
            Descripcion = $"Retiro CC ${monto:N2}"
        });
    }

    public decimal CalcularLimiteSobregiro() => Saldo + LimiteSobregiro;
}

public class TarjetaCredito : Cuenta
{
    public decimal Cupo { get; set; }
    public decimal Deuda { get; set; }
    public int NumeroCuotas { get; set; }

    public decimal CupoDisponible => Cupo - Deuda;

    public TarjetaCredito(string numeroCuenta, decimal cupo)
        : base(numeroCuenta, 0)
    {
        Cupo = cupo;
        Deuda = 0;
    }

    public void Comprar(decimal monto, int cuotas)
    {
        ValidarEstado();
        if (monto > CupoDisponible) throw new InvalidOperationException("Cupo insuficiente.");
        Deuda += monto;
        NumeroCuotas = cuotas;
        RegistrarMovimiento(new Movimiento
        {
            Tipo = TipoMovimiento.COMPRA_TC,
            Valor = monto,
            SaldoPosterior = Deuda,
            Descripcion = $"Compra TC {cuotas} cuotas ${monto:N2}"
        });
    }

    public void Pagar(decimal monto)
    {
        ValidarEstado();
        if (monto <= 0) throw new ArgumentException("Monto debe ser positivo.");
        if (monto > Deuda) monto = Deuda;
        Deuda -= monto;
        RegistrarMovimiento(new Movimiento
        {
            Tipo = TipoMovimiento.PAGO_TC,
            Valor = monto,
            SaldoPosterior = Deuda,
            Descripcion = $"Pago TC ${monto:N2}"
        });
    }

    public decimal CalcularTasa() => Cupo * 0.018m;
    public decimal CalcularCuotaMensual() => NumeroCuotas > 0 ? (Deuda + CalcularTasa()) / NumeroCuotas : 0;
    public override void Retirar(decimal monto) => throw new InvalidOperationException("TC no permite retiros.");
    public override void Consignar(decimal monto) => Pagar(monto);
}
