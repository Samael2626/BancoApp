namespace BancoApp.Models;

public enum TipoMovimiento
{
    CONSIGNACION,
    RETIRO,
    TRANSFERENCIA_OUT,
    TRANSFERENCIA_IN,
    COMPRA_TC,
    PAGO_TC
}

public enum EstadoCuenta
{
    ACTIVA,
    INACTIVA,
    BLOQUEADA,
    CERRADA
}
