using System.Net.Http.Json;
using System.Text.Json;


namespace AppVillagio.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    // URL Base:
   
    // Se estiver rodando no Windows Machine: use "https://localhost:7058"
    private const string BaseUrl = "http://10.0.2.2:5050"; // <--- CONFIRA A PORTA DO SWAGGER

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    // --- 1. LOGIN ---
    public async Task<LoginResponseDto> LoginAsync(string identificador, string senha)
    {
        var loginData = new { Identificador = identificador, Senha = senha };

        var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginData);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        }

        return null; // Ou lance uma exceção para tratar na ViewModel
    }

    // --- 2. CRIAR RESERVA ---
    public async Task<bool> CriarReservaAsync(NovaReservaDto reserva)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/Reservas", reserva);
        return response.IsSuccessStatusCode;
    }

    // --- 3. BUSCAR RESERVAS DO CLIENTE ---
    public async Task<List<ReservaDto>> GetMinhasReservasAsync(int clienteId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<ReservaDto>>($"/api/Reservas/cliente/{clienteId}");
        }
        catch
        {
            return new List<ReservaDto>();
        }
    }
}

// --- DTOs RÁPIDOS PARA O MAUI (Pode por em arquivos separados depois) ---
public class LoginResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Token { get; set; }
}

public class NovaReservaDto
{
    public int ClienteId { get; set; }
    public DateOnly DataReserva { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public string Observacoes { get; set; }
    public List<ItemPedidoDto> Itens { get; set; } = new();
}

public class ItemPedidoDto
{
    public int AtividadeId { get; set; }
    public int Quantidade { get; set; }
}

public class ReservaDto
{
    public int Id { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; }
    public DateOnly DataReserva { get; set; }
}