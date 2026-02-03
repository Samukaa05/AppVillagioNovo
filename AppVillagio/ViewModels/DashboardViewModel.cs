using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppVillagio.Services; // <--- Não esqueça de importar os Serviços

namespace AppVillagio.ViewModels;

// REMOVEMOS O [QueryProperty] pois agora lemos da memória global
public partial class DashboardViewModel : ObservableObject
{
	// Acessamos o serviço de sessão
	private readonly UserSession _session;

	// Propriedades para a Tela usar (Binding)
	[ObservableProperty]
	private string saudacaoUsuario;

	[ObservableProperty]
	private bool isAgencia;

	// CONSTRUTOR: O App entrega a Sessão pronta aqui
	public DashboardViewModel(UserSession session)
	{
		_session = session;
		CarregarDados();
	}

	private void CarregarDados()
	{
		// Pega os dados direto da memória segura
		IsAgencia = _session.TipoUsuario == "Agencia";

		// Monta uma frase bonita pro topo da tela
		// Se Identificador for nulo (teste), coloca "Visitante"
		var nome = string.IsNullOrEmpty(_session.Identificador) ? "Visitante" : _session.Identificador;
		SaudacaoUsuario = $"Olá, {nome}";
	}

	[RelayCommand]
	private async Task ReservarHorario()
	{
		// Exemplo: Se for Agência faz uma coisa, se for Família faz outra
		string acao = IsAgencia ? "Venda Atacado" : "Reserva Varejo";

		await Shell.Current.DisplayAlert(acao, $"Iniciando ação para: {_session.Identificador}", "OK");
	}

	[RelayCommand]
	private async Task VerReservas()
	{
		await Shell.Current.DisplayAlert("Histórico", $"Buscando histórico de {_session.TipoUsuario}...", "OK");
	}

	// --- Navegação do Rodapé ---

	[RelayCommand]
	private async Task IrParaConfiguracoes()
	{
		await Shell.Current.DisplayAlert("Config", "Tela de Configurações", "OK");
	}

	[RelayCommand]
	private async Task IrParaPedidos()
	{
		await Shell.Current.DisplayAlert("Pedidos", "Tela de Pedidos/Sacola", "OK");
	}

	[RelayCommand]
	private void RecarregarHome()
	{
		// Recarrega os dados caso tenha mudado algo
		CarregarDados();
	}
}