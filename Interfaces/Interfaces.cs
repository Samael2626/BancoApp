using BancoApp.Models;

namespace BancoApp.Interfaces;

public interface IAutenticable
{
    bool Autenticar(string usuario, string contrasena);
    void CerrarSesion();
    void CambiarContrasena(string old, string @new);
}

public interface ITransaction
{
    void Consignar(decimal monto);
    void Retirar(decimal monto);
    decimal ConsultarSaldo();
    List<Movimiento> ObtenerMovimientos();
}

public interface ITransferible
{
    void Transferir(Cuenta destino, decimal monto);
    bool ValidarDestino(Cuenta c);
}
