using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppVillagio.Services; // Importante para achar ApiService e UserSession
using AppVillagio.Views;    // Importante para achar DashboardPage
using AppVillagio.Models;

namespace AppVillagio.ViewModels;

public partial class LoginViewModel : ObservableObject, IQueryAttributable
{
	// --- 1. DECLARAR OS SERVIÇOS AQUI ---
	private readonly UserSession _session;
	private readonly ApiService _apiService; // <--- Faltava declarar isso!

	private string _tipoUsuarioAtual;

	[ObservableProperty]
	private string tituloIdentificador = "Insira seu Telefone:";

	[ObservableProperty]
	private string placeholderIdentificador = "11 99999-9999";

	[ObservableProperty]
	private string identificador;

	[ObservableProperty]
	private string senha;

	[ObservableProperty]
	private bool isPasswordSecret = true;

	// --- 2. INJETAR NO CONSTRUTOR ---
	// O App entrega a Sessão e a API prontas para uso aqui
	public LoginViewModel(UserSession session, ApiService apiService)
	{
		_session = session;
		_apiService = apiService; // <--- Faltava guardar o valor aqui!
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey("tipoUsuario"))
		{
			var tipo = query["tipoUsuario"].ToString();
			_tipoUsuarioAtual = tipo;

			if (tipo == "Agencia")
			{
				TituloIdentificador = "Insira seu CNPJ:";
				PlaceholderIdentificador = "00.000.000/0001-00";
			}
			else
			{
				TituloIdentificador = "Insira seu Telefone:";
				PlaceholderIdentificador = "(11) 99999-9999";
			}
		}
	}

	[RelayCommand]
	private void ToggleSenha()
	{
		IsPasswordSecret = !IsPasswordSecret;
	}

	[RelayCommand]
	private async Task VoltarHome()
	{
		await Shell.Current.GoToAsync("//WelcomePage");
	}

	[RelayCommand]
	private async Task IrParaCadastro()
	{
		// Navega para o Cadastro passando o tipo (Agencia ou Familia)
		await Shell.Current.GoToAsync($"{nameof(CadastroPage)}?tipoUsuario={_tipoUsuarioAtual}");
	}

	[RelayCommand]
    private async Task Login()
    {
        // Feedback visual simples
        // await Shell.Current.DisplayAlert("Aguarde", "Conectando...", "OK");

        // Chama a API e guarda na variável 'resultadoLogin'
        var resultadoLogin = await _apiService.LoginAsync(Identificador, Senha);

        if (resultadoLogin != null)
        {
            // Salva os dados na memória do App
            _session.Identificador = Identificador;
            _session.Nome = resultadoLogin.Nome;

            // CORREÇÃO AQUI:
            _session.Id = resultadoLogin.Id;

            _session.Token = resultadoLogin.Token;
            _session.TipoUsuario = _tipoUsuarioAtual;

            // Vai para a Dashboard
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }
        else
        {
            await Shell.Current.DisplayAlert("Erro", "Login ou senha inválidos!", "OK");
        }
    }
}