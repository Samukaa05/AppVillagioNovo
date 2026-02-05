using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using AppVillagio.Services;   // <--- Confira se tem esse using
using AppVillagio.ViewModels; // <--- Confira se tem esse using
using AppVillagio.Views;      // <--- Confira se tem esse using

namespace AppVillagio;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		

		// 1. Serviço (Necessário para a ViewModel funcionar)
		builder.Services.AddSingleton<ApiService>();

		// 2. ViewModels
		builder.Services.AddTransient<WelcomeViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<CadastroViewModel>();
		builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<MinhasReservasViewModel>();
        builder.Services.AddTransient<ReservaViewModel>();
		builder.Services.AddTransient<CalendarioViewModel>();
        builder.Services.AddSingleton<UserSession>();


		// 3. Pages
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<WelcomePage>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<CadastroPage>();
        builder.Services.AddTransient<ReservaPage>();
		builder.Services.AddTransient<CalendarioPage>();
        builder.Services.AddTransient<MinhasReservasPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}