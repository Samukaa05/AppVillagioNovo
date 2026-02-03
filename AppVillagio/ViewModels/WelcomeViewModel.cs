using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppVillagio.Views; // Ajuste o namespace se precisar

namespace AppVillagio.ViewModels;

public partial class WelcomeViewModel : ObservableObject
{
    [RelayCommand]
    private async Task NavigateToLoginAsync(string tipoUsuario)
    {
		// tipoUsuario vai ser "Familia" ou "Agencia"
		// Navega para a tela de Login passando esse dado (se quiser usar depois)
		// Por enquanto, vamos só navegar simples:
		await Shell.Current.GoToAsync($"{nameof(LoginPage)}?tipoUsuario={tipoUsuario}");
	}
}