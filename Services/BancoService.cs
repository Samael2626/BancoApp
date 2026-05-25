using BancoApp.Models;

namespace BancoApp.Services;

public class BancoService
{
    private readonly List<Cliente> _clientes = new();
    private int _clienteId = 1;
    private int _cuentaNum = 1000;

    public Cliente? ClienteActual { get; private set; }
    public bool Autenticado => ClienteActual != null;
    public string? LastLoginError { get; private set; }

    public BancoService() => SeedData();

    private void SeedData()
    {
        var cliente = new Cliente(_clienteId++, "1234567890", "Ana García", "3001234567", "ana", "1234");
        var ahorros = new CuentaAhorros($"CA-{_cuentaNum++}", 500_000);
        var corriente = new CuentaCorriente($"CC-{_cuentaNum++}", 1_000_000);
        var tc = new TarjetaCredito($"TC-{_cuentaNum++}", 3_000_000);
        cliente.Cuentas.AddRange(new Cuenta[] { ahorros, corriente, tc });
        _clientes.Add(cliente);
    }

    public bool Login(string usuario, string contrasena)
    {
        LastLoginError = null;
        var cliente = _clientes.FirstOrDefault(c => c.Usuario == usuario);
        if (cliente == null)
        {
            LastLoginError = "Usuario o contraseña incorrectos.";
            return false;
        }
        if (cliente.Bloqueado)
        {
            if (cliente.BloqueadoHasta.HasValue)
            {
                var rem = cliente.BloqueadoHasta.Value - DateTime.UtcNow;
                if (rem.TotalSeconds > 0)
                {
                    LastLoginError = $"Cuenta bloqueada temporalmente. Intenta de nuevo en {rem.Minutes} minutos y {rem.Seconds} segundos.";
                    return false;
                }
            }
        }
        if (!cliente.Autenticar(usuario, contrasena))
        {
            LastLoginError = "Usuario o contraseña incorrectos.";
            return false;
        }
        ClienteActual = cliente;
        LastLoginError = null;
        return true;
    }

    public void Logout() => ClienteActual = null;

    public Cliente Registrar(string id, string nombre, string celular, string usuario, string pass)
    {
        if (_clientes.Any(c => c.Usuario == usuario))
            throw new InvalidOperationException("Usuario ya existe.");
        var cliente = new Cliente(_clienteId++, id, nombre, celular, usuario, pass);
        var ahorros = new CuentaAhorros($"CA-{_cuentaNum++}");
        cliente.Cuentas.Add(ahorros);
        _clientes.Add(cliente);
        return cliente;
    }
}
