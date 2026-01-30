using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using AppVillagio.Services;
using AppVillagio.ViewModels; // <--- Importante
using AppVillagio.Views;      // <--- Importante

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

        // --- AQUI ESTAVA O PROBLEMA ---

        // 1. Serviços
      

        // 2. ViewModels (O erro aconteceu porque faltava essa linha aqui!)
        builder.Services.AddTransient<WelcomeViewModel>();
      

        // 3. Pages
        builder.Services.AddTransient<WelcomePage>();
     

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}