namespace AppVillagio.Models;

public class Atividade
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal PrecoPadrao { get; set; } // Importante: Tem que ser igual ao JSON da API
    public int? DuracaoMinutos { get; set; }
}