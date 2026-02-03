using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppVillagio.Services; // <--- Importante
using AppVillagio.Views;

namespace AppVillagio.ViewModels;

public partial class CadastroViewModel : ObservableObject, IQueryAttributable
{
	private readonly ApiService _apiService; // Serviço da API

	// Propriedades do Formulário
	[ObservableProperty] private string nome;
	[ObservableProperty] private string telefone;
	[ObservableProperty] private string email;
	[ObservableProperty] private string cnpj;
	[ObservableProperty] private string senha;

	// Controla a tela (Visual)
	[ObservableProperty] private bool isAgencia;
	[ObservableProperty] private bool isPasswordSecret = true;

	// Construtor com Injeção de Dependência
	public CadastroViewModel(ApiService apiService)
	{
		_apiService = apiService;
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		if (query.ContainsKey("tipoUsuario"))
		{
			var tipo = query["tipoUsuario"].ToString();
			// Verifica se é Agencia para mostrar/esconder campos
			IsAgencia = string.Equals(tipo, "Agencia", StringComparison.OrdinalIgnoreCase);
		}
	}

	[RelayCommand]
	private void ToggleSenha() => IsPasswordSecret = !IsPasswordSecret;

	[RelayCommand]
	private async Task Cadastrar()
	{
		// 1. Validação Básica (pra não mandar lixo pro banco)
		if (string.IsNullOrEmpty(Nome) || string.IsNullOrEmpty(Telefone) || string.IsNullOrEmpty(Senha))
		{
			await Shell.Current.DisplayAlert("Atenção", "Preencha os campos obrigatórios!", "OK");
			return;
		}

		// 2. Montar o Objeto de Envio (DTO)
		var cadastroDto = new CadastroRequestDto
		{
			Nome = Nome,
			Telefone = Telefone,
			Senha = Senha,
			TipoUsuario = IsAgencia ? "Agencia" : "Familia",
			// Se for Agência manda os dados, se não manda null ou vazio
			Email = IsAgencia ? Email : null,
			Cnpj = IsAgencia ? Cnpj : null
		};

		// 3. Chamar a API
		// await Shell.Current.DisplayAlert("Aguarde", "Criando conta...", "OK"); // Feedback opcional

		bool sucesso = await _apiService.CadastrarAsync(cadastroDto);

		if (sucesso)
		{
			await Shell.Current.DisplayAlert("Sucesso", "Conta criada! Faça login para continuar.", "OK");

			// Volta para a tela de Login para a pessoa entrar
			await Shell.Current.GoToAsync("..");
		}
		else
		{
			await Shell.Current.DisplayAlert("Erro", "Não foi possível cadastrar. Verifique os dados ou tente mais tarde.", "OK");
		}
	}

	[RelayCommand]
	private async Task VoltarLogin()
	{
		await Shell.Current.GoToAsync("..");
	}

	[RelayCommand]
	private async Task VoltarHome()
	{
		await Shell.Current.GoToAsync("//WelcomePage");
	}
}