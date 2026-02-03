using AppVillagio.ViewModels;

namespace AppVillagio.Views;

public partial class LoginPage : ContentPage
{
	// O MAUI injeta a ViewModel aqui
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();

		// ESTA LINHA É A MAIS IMPORTANTE:
		BindingContext = vm;
	}
}