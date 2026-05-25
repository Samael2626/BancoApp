using BancoApp.Interfaces;

namespace BancoApp.Models;

public class Cliente : IAutenticable
{
    public int Id { get; set; }
    public string Identificacion { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    private string Contrasena { get; set; } = string.Empty;
    public int IntentosFallidos { get; private set; }
    public DateTime? BloqueadoHasta { get; private set; }
    public bool Bloqueado => BloqueadoHasta.HasValue && BloqueadoHasta.Value > DateTime.UtcNow;

    public List<Cuenta> Cuentas { get; set; } = new();

    public Cliente() { }

    public Cliente(int id, string identificacion, string nombre, string celular,
        string usuario, string contrasena)
    {
        Id = id;
        Identificacion = identificacion;
        NombreCompleto = nombre;
        Celular = celular;
        Usuario = usuario;
        Contrasena = contrasena;
    }

    public bool Autenticar(string usuario, string contrasena)
    {
        if (Bloqueado) return false;
        if (Usuario == usuario && Contrasena == contrasena)
        {
            ResetearIntentos();
            return true;
        }
        IncrementarIntentos();
        return false;
    }

    public void CerrarSesion() { }

    public void CambiarContrasena(string old, string @new)
    {
        if (Contrasena != old) throw new UnauthorizedAccessException("Contraseña actual incorrecta.");
        Contrasena = @new;
    }

    public void IncrementarIntentos()
    {
        IntentosFallidos++;
        if (IntentosFallidos >= 3)
        {
            // bloquear por 5 minutos
            BloqueadoHasta = DateTime.UtcNow.AddMinutes(5);
            IntentosFallidos = 0;
        }
    }

    public void ResetearIntentos()
    {
        IntentosFallidos = 0;
        BloqueadoHasta = null;
    }

    public bool EditarPerfil(string nombre, string celular)
    {
        NombreCompleto = nombre;
        Celular = celular;
        return true;
    }
}
