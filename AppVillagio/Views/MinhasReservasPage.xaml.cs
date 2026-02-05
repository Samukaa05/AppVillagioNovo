using AppVillagio.ViewModels;

namespace AppVillagio.Views; // <--- O ERRO COSTUMA ESTAR AQUI (Confira se tem o .Views)

public partial class MinhasReservasPage : ContentPage
{
    public MinhasReservasPage(MinhasReservasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}