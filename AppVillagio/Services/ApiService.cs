using System.Net.Http.Json;
using AppVillagio.Services;
using AppVillagio.Models; // Importante para reconhecer a classe Atividade

namespace AppVillagio.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly UserSession _session;

    // CONFIGURAÇÃO DE URL (CRUCIAL PARA ANDROID)
#if ANDROID
    private const string BaseUrl = "http://10.0.2.2:5014"; // Android Emulator
#else
    private const string BaseUrl = "http://localhost:5014"; // Windows Machine
#endif

    public ApiService(UserSession session)
    {
        _session = session;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    // --- 1. LOGIN ---
    public async Task<LoginResponseDto> LoginAsync(string identificador, string senha)
    {
        try
        {
            var loginData = new { Identificador = identificador, Senha = senha };
            var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro no Login: {ex.Message}");
        }
        return null;
    }

    // --- 2. CADASTRAR USUÁRIO ---
    public async Task<bool> CadastrarAsync(CadastroRequestDto dados)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Auth/register", dados);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro no Cadastro: {ex.Message}");
            return false;
        }
    }

    // --- 3. BUSCAR ATIVIDADES (O QUE FALTAVA!) ---
    public async Task<List<Atividade>> GetAtividadesAsync()
    {
        try
        {
            // Busca a lista de atividades do banco de dados via API
            var response = await _httpClient.GetFromJsonAsync<List<Atividade>>("/api/Atividades");
            return response ?? new List<Atividade>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao buscar atividades: {ex.Message}");
            return new List<Atividade>(); // Retorna vazio para não travar a tela
        }
    }

    // --- 4. CRIAR RESERVA ---
    public async Task<bool> CriarReservaAsync(NovaReservaDto reserva)
    {
        try
        {
            AdicionarTokenNoHeader();
            var response = await _httpClient.PostAsJsonAsync("/api/Reservas", reserva);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao criar reserva: {ex.Message}");
            return false;
        }
    }

    // --- 5. BUSCAR RESERVAS DO CLIENTE ---
    public async Task<List<ReservaDto>> GetMinhasReservasAsync(int clienteId)
    {
        try
        {
            AdicionarTokenNoHeader();
            return await _httpClient.GetFromJsonAsync<List<ReservaDto>>($"/api/Reservas/cliente/{clienteId}");
        }
        catch
        {
            return new List<ReservaDto>();
        }
    }

    // --- HELPER: Autenticação ---
    private void AdicionarTokenNoHeader()
    {
        if (!string.IsNullOrEmpty(_session.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
        }
    }
}

// --- DTOs ---

public class LoginResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Token { get; set; }
    public string TipoUsuario { get; set; }
}

public class CadastroRequestDto
{
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string Senha { get; set; }
    public string TipoUsuario { get; set; }
    public string Email { get; set; }
    public string Cnpj { get; set; }
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