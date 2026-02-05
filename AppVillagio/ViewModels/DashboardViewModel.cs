using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AppVillagio.Services;
using AppVillagio.Views; // Para achar a ReservaPage

namespace AppVillagio.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly UserSession _session;

    public DashboardViewModel(UserSession session)
    {
        _session = session;
    }

    // --- COMANDOS PRINCIPAIS (Cards) ---

    [RelayCommand]
    private async Task ReservarHorario()
    {
        // Navega para a tela de calendário que vamos criar/conectar
        await Shell.Current.GoToAsync(nameof(ReservaPage));
    }

    [RelayCommand]
    private async Task VerReservas()
    {
        await Shell.Current.GoToAsync(nameof(MinhasReservasPage));
    }

    // --- COMANDOS DO RODAPÉ (Footer) ---

    [RelayCommand]
    private async Task IrParaConfiguracoes()
    {
        // Exemplo: Opção de Sair do App pode ficar aqui
        bool sair = await Shell.Current.DisplayAlert("Configurações", "Deseja sair da conta?", "Sim", "Não");
        if (sair)
        {
            _session.Token = null;
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    [RelayCommand]
    private async Task RecarregarHome()
    {
        // Como já estamos na Home, pode servir para atualizar dados da API
        // await CarregarDadosDoUsuario();
        await Shell.Current.DisplayAlert("Home", "Dados atualizados!", "OK");
    }

    [RelayCommand]
    private async Task IrParaPedidos()
    {
        await Shell.Current.DisplayAlert("Pedidos", "Sua sacola de compras (Em breve).", "OK");
    }
}