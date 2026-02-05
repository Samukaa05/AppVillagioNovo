using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppVillagio.Services;
using AppVillagio.Models; // Para o DTO

namespace AppVillagio.ViewModels;

public partial class MinhasReservasViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly UserSession _session;

    public ObservableCollection<ReservaDto> Reservas { get; } = new();

    public MinhasReservasViewModel(ApiService apiService, UserSession session)
    {
        _apiService = apiService;
        _session = session;

        // Já carrega assim que nasce
        CarregarReservas();
    }

    [RelayCommand]
    private async Task CarregarReservas()
    {
        // Pega do banco real
        var lista = await _apiService.GetMinhasReservasAsync(_session.Id);

        Reservas.Clear();
        foreach (var item in lista)
        {
            // Se o backend não mandar o nome, usamos o da sessão pra preencher visualmente
            if (string.IsNullOrEmpty(item.NomeCliente))
                item.NomeCliente = _session.Nome;

            Reservas.Add(item);
        }
    }

    [RelayCommand]
    private async Task Excluir(ReservaDto reserva)
    {
        bool confirmou = await Shell.Current.DisplayAlert("Cancelar",
            $"Deseja cancelar o agendamento de {reserva.DataFormatada}?", "Sim", "Não");

        if (!confirmou) return;

        bool sucesso = await _apiService.DeletarReservaAsync(reserva.Id);

        if (sucesso)
        {
            Reservas.Remove(reserva); // Tira da tela na hora
            await Shell.Current.DisplayAlert("Sucesso", "Agendamento cancelado.", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Erro", "Não foi possível cancelar.", "OK");
        }
    }

    [RelayCommand]
    private async Task Editar(ReservaDto reserva)
    {
        // Por enquanto só um alerta, editar é mais complexo (precisa voltar pro calendário)
        await Shell.Current.DisplayAlert("Editar", "Para editar, cancele e faça um novo agendamento.", "Entendi");
    }

    [RelayCommand]
    private async Task Voltar()
    {
        await Shell.Current.GoToAsync("..");
    }
}