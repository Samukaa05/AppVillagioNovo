namespace AppVillagio.Models;

public class HorarioAgendamento
{
    public string Hora { get; set; }        // Ex: "08:00"
    public bool IsDisponivel { get; set; }  // Ex: true

    // Helper para exibir texto na tela
    public string StatusTexto => IsDisponivel ? "Disponível" : "Indisponível";
}