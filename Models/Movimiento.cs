namespace BancoApp.Models;

public class Movimiento
{
    public int Id { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.Now;
    public TipoMovimiento Tipo { get; set; }
    public decimal Valor { get; set; }
    public decimal SaldoPosterior { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
