namespace AppVillagio.Models;

public class ReservaDto
{
    public int Id { get; set; }
    public string NomeCliente { get; set; }

    // IMPORTANTE: Use DateTime e TimeSpan para casar com o SQL Server
    public DateTime DataReserva { get; set; }
    public TimeSpan HoraInicio { get; set; }

    // Formatação visual para a tela
    public string DataFormatada => $"{DataReserva:dd/MM} às {HoraInicio:hh\\:mm}";
}