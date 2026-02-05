using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AppVillagio.Services;
using AppVillagio.Models;

namespace AppVillagio.ViewModels;

public partial class CalendarioViewModel : ObservableObject, IQueryAttributable
{
    private readonly UserSession _session;
    private readonly ApiService _apiService;
    private DateTime _dataAtualCalendario;

    // --- DADOS DA RESERVA (Vindos da tela anterior) ---
    private int _qtdAdultos;
    private int _qtdCriancas;
    private int _qtdBebes;
    private Atividade _atividadeSelecionada;
    private decimal _valorTotal;

    public CalendarioViewModel(UserSession session, ApiService apiService)
    {
        _session = session;
        _apiService = apiService;
        _dataAtualCalendario = DateTime.Today;
        CarregarCalendario();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("QtdAdultos")) _qtdAdultos = int.Parse(query["QtdAdultos"].ToString());
        if (query.ContainsKey("QtdCriancas")) _qtdCriancas = int.Parse(query["QtdCriancas"].ToString());
        if (query.ContainsKey("QtdBebes")) _qtdBebes = int.Parse(query["QtdBebes"].ToString());
        if (query.ContainsKey("Atividade")) _atividadeSelecionada = (Atividade)query["Atividade"];
        if (query.ContainsKey("ValorTotal")) _valorTotal = decimal.Parse(query["ValorTotal"].ToString());
    }

    [ObservableProperty] string mesAnoTexto;
    [ObservableProperty] string frutaSafra;

    public ObservableCollection<CalendarioDia> Dias { get; } = new();
    public ObservableCollection<HorarioAgendamento> HorariosDoDia { get; } = new();

    [ObservableProperty] CalendarioDia diaSelecionado;

    [RelayCommand]
    void MesAnterior()
    {
        _dataAtualCalendario = _dataAtualCalendario.AddMonths(-1);
        CarregarCalendario();
    }

    [RelayCommand]
    void ProximoMes()
    {
        _dataAtualCalendario = _dataAtualCalendario.AddMonths(1);
        CarregarCalendario();
    }

    [RelayCommand]
    void SelecionarDia(CalendarioDia diaClicado)
    {
        if (diaClicado == null || !diaClicado.IsDisponivel || string.IsNullOrEmpty(diaClicado.NumeroDia)) return;

        foreach (var dia in Dias) dia.IsSelecionado = false;

        diaClicado.IsSelecionado = true;
        DiaSelecionado = diaClicado;

        CarregarHorarios(diaClicado.Data);
    }

    [RelayCommand]
    async Task AgendarHorario(HorarioAgendamento horario)
    {
        if (DiaSelecionado == null || _atividadeSelecionada == null) return;

        bool confirmar = await Shell.Current.DisplayAlert("Confirmar Agendamento",
            $"Atividade: {_atividadeSelecionada.Nome}\n" +
            $"Data: {DiaSelecionado.NumeroDia}/{MesAnoTexto}\n" +
            $"Horário: {horario.Hora}\n" +
            $"Total: R$ {_valorTotal:F2}",
            "Confirmar", "Cancelar");

        if (!confirmar) return;

        try
        {
            var partesHora = horario.Hora.Split(':');
            int hora = int.Parse(partesHora[0]);
            int minuto = int.Parse(partesHora[1]);

            TimeOnly horaInicio = new TimeOnly(hora, minuto);
            int duracao = _atividadeSelecionada.DuracaoMinutos.HasValue && _atividadeSelecionada.DuracaoMinutos > 0
                          ? _atividadeSelecionada.DuracaoMinutos.Value
                          : 60;

            TimeOnly horaFim = horaInicio.AddMinutes(duracao);

            var novaReserva = new NovaReservaDto
            {
                ClienteId = _session.Id,
                DataReserva = DateOnly.FromDateTime(DiaSelecionado.Data),
                HoraInicio = horaInicio,
                HoraFim = horaFim,
                Observacoes = $"App: {_qtdAdultos} Adt, {_qtdCriancas} Cri, {_qtdBebes} Beb.",
                Itens = new List<ItemPedidoDto>
                {
                    new ItemPedidoDto
                    {
                        AtividadeId = _atividadeSelecionada.Id,
                        Quantidade = _qtdAdultos + _qtdCriancas + _qtdBebes
                    }
                }
            };

            bool sucesso = await _apiService.CriarReservaAsync(novaReserva);

            if (sucesso)
            {
                await Shell.Current.DisplayAlert("Sucesso!", "Sua reserva foi gravada no banco!", "OK");
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", "Ocorreu um erro ao salvar. Verifique sua conexão.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro Técnico", ex.Message, "OK");
        }
    }

    private void CarregarCalendario()
    {
        Dias.Clear();
        HorariosDoDia.Clear();

        MesAnoTexto = _dataAtualCalendario.ToString("MMMM yyyy").ToUpper();

        // --- AQUI ESTÁ A LISTA NOVA E OFICIAL ---
        FrutaSafra = ObterSafraDoMes(_dataAtualCalendario.Month);

        var primeiroDiaDoMes = new DateTime(_dataAtualCalendario.Year, _dataAtualCalendario.Month, 1);
        int diasVazios = (int)primeiroDiaDoMes.DayOfWeek;

        for (int i = 0; i < diasVazios; i++)
        {
            Dias.Add(new CalendarioDia { NumeroDia = "", CorFundo = Colors.Transparent, IsDisponivel = false });
        }

        int diasNoMes = DateTime.DaysInMonth(_dataAtualCalendario.Year, _dataAtualCalendario.Month);
        for (int i = 1; i <= diasNoMes; i++)
        {
            var dataDia = new DateTime(_dataAtualCalendario.Year, _dataAtualCalendario.Month, i);
            bool disponivel = ValidarRegraDeNegocio(dataDia);
            var cor = disponivel ? Color.FromArgb("#6E8B68") : Color.FromArgb("#4E2A1E");

            Dias.Add(new CalendarioDia
            {
                Data = dataDia,
                NumeroDia = i.ToString(),
                IsDisponivel = disponivel,
                CorFundo = cor
            });
        }
    }

    private async void CarregarHorarios(DateTime data)
    {
        HorariosDoDia.Clear();

        // 1. Pergunta pra API quais horas já elvis
        // (Usei .Result ou Wait() aqui só por simplicidade no void, 
        // mas o ideal seria mudar a assinatura para async Task)
        var ocupados = await _apiService.GetHorariosOcupadosAsync(data);

        // 2. Cria a lista das 08h às 16h verificando a disponibilidade
        for (int hora = 8; hora <= 16; hora++)
        {
            // Cria um TimeSpan pra comparar (ex: 08:00:00)
            TimeSpan horarioAtual = new TimeSpan(hora, 0, 0);

            // Verifica se esse horário está na lista de ocupados
            bool estaLivre = !ocupados.Contains(horarioAtual);

            string horaFormatada = $"{hora:D2}:00";

            HorariosDoDia.Add(new HorarioAgendamento
            {
                Hora = horaFormatada,
                IsDisponivel = estaLivre // <--- AQUI É O PULO DO GATO
            });
        }
    }

    private bool ValidarRegraDeNegocio(DateTime data)
    {
        var hoje = DateTime.Today;
        if (data < hoje) return false;

        bool isAgencia = _session?.TipoUsuario == "Agencia";

        if (isAgencia)
            return data >= hoje.AddDays(15);
        else
        {
            if (data < hoje.AddDays(2)) return false;
            var dw = data.DayOfWeek;
            return dw == DayOfWeek.Saturday || dw == DayOfWeek.Sunday;
        }
    }

    // --- LISTA OFICIAL DE SAFRAS ---
    private string ObterSafraDoMes(int mes)
    {
        return mes switch
        {
            1 => "Uva, Goiaba, Morango e Lichia",
            2 => "Uva, Goiaba e Morango",
            3 => "Uva e Goiaba",
            4 => "Morango e Goiaba",
            5 or 6 or 7 => "Goiaba, Uva e Morango",
            8 or 9 => "Morango",
            10 or 11 => "Pêssego, Morango e Goiaba",
            12 => "Uva, Goiaba, Morango e Lichia",
            _ => "Safra Variada"
        };
    }
}