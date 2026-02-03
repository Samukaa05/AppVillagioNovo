using System.Net.Http.Json;
using AppVillagio.Services; // Para o UserSession

namespace AppVillagio.Services;

public class ApiService
{
	private readonly HttpClient _httpClient;
	private readonly UserSession _session; // Guardamos a sessão para usar o Token depois

	// URL Base: Ajusta sozinho se for Android ou Windows

    private const string BaseUrl = "http://localhost:5014";


	// Injetamos a Sessão aqui
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

			// Atenção: Confira se a rota no seu Backend é /api/Auth/login mesmo
			var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginData);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro na API: {ex.Message}");
		}

		return null;
	}

	// --- 2. CADASTRAR USUÁRIO (NOVO!) ---
	public async Task<bool> CadastrarAsync(CadastroRequestDto dados)
	{
		try
		{
			// Envia os dados para a rota de registro
			// Rota sugerida: /api/Auth/register
			var response = await _httpClient.PostAsJsonAsync("/api/Auth/register", dados);

			return response.IsSuccessStatusCode;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erro no cadastro: {ex.Message}");
			return false;
		}
	}

	// --- 3. CRIAR RESERVA ---
	public async Task<bool> CriarReservaAsync(NovaReservaDto reserva)
	{
		// DICA DE OURO: Adicionar o Token no Header antes de chamar
		AdicionarTokenNoHeader();

		var response = await _httpClient.PostAsJsonAsync("/api/Reservas", reserva);
		return response.IsSuccessStatusCode;
	}

	// --- 4. BUSCAR RESERVAS DO CLIENTE ---
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

	// Método auxiliar para garantir que o Token vai junto na requisição
	private void AdicionarTokenNoHeader()
	{
		if (!string.IsNullOrEmpty(_session.Token))
		{
			_httpClient.DefaultRequestHeaders.Authorization =
				new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
		}
	}
}

// --- DTOs (Data Transfer Objects) ---

public class LoginResponseDto
{
	public int Id { get; set; }
	public string Nome { get; set; }
	public string Token { get; set; }
	public string TipoUsuario { get; set; }
}

// ADICIONEI ESSE DTO PARA O CADASTRO FUNCIONAR:
public class CadastroRequestDto
{
	public string Nome { get; set; }
	public string Telefone { get; set; }
	public string Senha { get; set; }
	public string TipoUsuario { get; set; } // "Familia" ou "Agencia"
	public string Email { get; set; }       // Opcional (só Agencia)
	public string Cnpj { get; set; }        // Opcional (só Agencia)
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