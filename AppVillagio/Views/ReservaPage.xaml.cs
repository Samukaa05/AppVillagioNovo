using AppVillagio.ViewModels;

namespace AppVillagio.Views;

public partial class ReservaPage : ContentPage
{
    // Construtor recebendo a ViewModel
    public ReservaPage(ReservaViewModel viewModel)
    {
        InitializeComponent();

        // AQUI ESTÁ A MÁGICA: Conecta a tela ao código
        BindingContext = viewModel;
    }

    // Garante que carrega os dados toda vez que a tela aparece
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ReservaViewModel vm)
        {
            await vm.Inicializar();
        }
    }
}