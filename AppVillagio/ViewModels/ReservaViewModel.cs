using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppVillagio.Services;
using AppVillagio.Models;
using AppVillagio.Views;

namespace AppVillagio.ViewModels;

public partial class ReservaViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly UserSession _session;

    public ReservaViewModel(ApiService apiService, UserSession session)
    {
        _apiService = apiService;
        _session = session;
        NomeRepresentante = !string.IsNullOrEmpty(_session.Nome) ? _session.Nome : _session.Identificador;
    }

    public async Task Inicializar()
    {
        await CarregarAtividades();
    }

    [ObservableProperty] string nomeRepresentante;

    // --- Quantidades (Com gatilho para recalcular) ---
    [ObservableProperty] int qtdAte5Anos = 0;
    [ObservableProperty] int qtd6a12Anos = 0;
    [ObservableProperty] int qtdAdultos = 0;

    [ObservableProperty] ObservableCollection<Atividade> listaAtividades = new();

    [ObservableProperty] Atividade atividadeSelecionada;

    // --- PROPRIEDADE NOVA: O TEXTO DO TOTAL ---
    [ObservableProperty] string textoTotal = "R$ 0,00";
    [ObservableProperty] decimal valorTotalNumerico = 0; // Para validação

    // Quando clicar na atividade, roda isso:
    [RelayCommand]
    private void SelecionarAtividade(Atividade atividade)
    {
        AtividadeSelecionada = atividade;
        CalcularTotal(); // <--- Mágica acontece aqui
    }

    // A MATEMÁTICA BRUTA
    private void CalcularTotal()
    {
        // Se não escolheu atividade ainda, o total é zero
        if (AtividadeSelecionada == null)
        {
            TextoTotal = "Selecione uma atividade";
            ValorTotalNumerico = 0;
            return;
        }

        decimal precoCheio = AtividadeSelecionada.PrecoPadrao;

        // 1. Até 5 anos = R$ 0,00
        decimal totalKidsPequenos = 0;

        // 2. 6 a 12 anos = Metade do Preço
        decimal totalKidsGrandes = Qtd6a12Anos * (precoCheio / 2);

        // 3. Adulto = Preço Cheio
        decimal totalAdultos = QtdAdultos * precoCheio;

        // Soma tudo
        ValorTotalNumerico = totalKidsPequenos + totalKidsGrandes + totalAdultos;

        // Atualiza a tela
        TextoTotal = $"Total: R$ {ValorTotalNumerico:F2}";
    }

    private async Task CarregarAtividades()
    {
        var dados = await _apiService.GetAtividadesAsync();
        ListaAtividades = new ObservableCollection<Atividade>(dados);
    }

    // --- BOTÕES DE QUANTIDADE ---
    // Sempre que mudar o número, chamamos CalcularTotal()

    [RelayCommand]
    private async Task SelecionarQtdAte5()
    {
        string res = await Shell.Current.DisplayPromptAsync("Até 5 anos", "Quantidade:", keyboard: Keyboard.Numeric);
        if (int.TryParse(res, out int q))
        {
            QtdAte5Anos = q;
            CalcularTotal(); // Recalcula
        }
    }

    [RelayCommand]
    private async Task SelecionarQtd6a12()
    {
        string res = await Shell.Current.DisplayPromptAsync("6 a 12 anos", "Quantidade:", keyboard: Keyboard.Numeric);
        if (int.TryParse(res, out int q))
        {
            Qtd6a12Anos = q;
            CalcularTotal(); // Recalcula
        }
    }

    [RelayCommand]
    private async Task SelecionarQtdAdulto()
    {
        string res = await Shell.Current.DisplayPromptAsync("Adultos", "Quantidade:", keyboard: Keyboard.Numeric);
        if (int.TryParse(res, out int q))
        {
            QtdAdultos = q;
            CalcularTotal(); // Recalcula
        }
    }

    [RelayCommand]
    private async Task Avancar()
    {
        // 1. REGRA: Mínimo 1 Adulto
        if (QtdAdultos < 1)
        {
            await Shell.Current.DisplayAlert("Regra", "É necessário pelo menos 1 adulto responsável.", "Entendi");
            return;
        }

        // 2. Validação de Atividade
        if (AtividadeSelecionada == null)
        {
            await Shell.Current.DisplayAlert("Atenção", "Selecione uma atividade!", "OK");
            return;
        }

        // 3. REGRA: Capacidade Máxima
        int totalPessoas = QtdAdultos + Qtd6a12Anos + QtdAte5Anos;
        int limite = 0;
        bool excedeu = false;

        // Verifica o nome da atividade (pode usar ID se preferir)
        if (AtividadeSelecionada.Nome.Contains("Café"))
        {
            limite = 120;
            if (totalPessoas > 120) excedeu = true;
        }
        else if (AtividadeSelecionada.Nome.Contains("Passeio") || AtividadeSelecionada.Nome.Contains("Combo"))
        {
            limite = 50; // Passeio e Combo limitados a 50
            if (totalPessoas > 50) excedeu = true;
        }

        if (excedeu)
        {
            await Shell.Current.DisplayAlert("Limite Excedido",
                $"Para esta atividade, o grupo máximo é de {limite} pessoas.", "OK");
            return;
        }

        // SE PASSOU POR TUDO: Vai para o Calendário levar esses dados
        // Usamos um Dicionário para passar os dados para a próxima tela
        var dadosNavegacao = new Dictionary<string, object>
        {
            { "QtdAdultos", QtdAdultos },
            { "QtdCriancas", Qtd6a12Anos },
            { "QtdBebes", QtdAte5Anos },
            { "Atividade", AtividadeSelecionada },
            { "ValorTotal", ValorTotalNumerico }
        };

        await Shell.Current.GoToAsync(nameof(CalendarioPage), dadosNavegacao);
    }
}