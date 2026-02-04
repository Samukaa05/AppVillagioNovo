using AppVillagio.Views;

namespace AppVillagio;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		
		// Isso ensina ao GPS que a rota "LoginPage" leva para a tela LoginPage
		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(CadastroPage), typeof(CadastroPage));
		Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(ReservaPage), typeof(ReservaPage));
    }
}