using AppVillagio.ViewModels;

namespace AppVillagio.Views;

public partial class CalendarioPage : ContentPage
{
    // O construtor TEM que receber o ViewModel
    public CalendarioPage(CalendarioViewModel viewModel)
    {
        InitializeComponent();

        // A mágica acontece aqui:
        BindingContext = viewModel;
    }
}