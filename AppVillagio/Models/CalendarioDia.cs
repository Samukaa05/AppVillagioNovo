using CommunityToolkit.Mvvm.ComponentModel;

namespace AppVillagio.Models;

// Agora herda de ObservableObject para a borda azul funcionar!
public partial class CalendarioDia : ObservableObject
{
    public DateTime Data { get; set; }

    // Antes era TextoDia, agora é NumeroDia para bater com a ViewModel
    public string NumeroDia { get; set; }

    public bool IsDisponivel { get; set; }
    public Color CorFundo { get; set; }

    // Propriedade que avisa a tela quando muda (pra ficar azul ao clicar)
    [ObservableProperty]
    private bool isSelecionado;
}