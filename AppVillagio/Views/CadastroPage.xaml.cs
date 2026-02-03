using AppVillagio.ViewModels;

namespace AppVillagio.Views;

public partial class CadastroPage : ContentPage
{
	// Tem que receber a ViewModel aqui!
	public CadastroPage(CadastroViewModel vm)
	{
		InitializeComponent();

		// ESSA LINHA É OBRIGATÓRIA PARA O CNPJ SUMIR:
		BindingContext = vm;
	}
}