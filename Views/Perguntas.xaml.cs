using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using LumeClient.DTOs;

namespace LumeClient.Views
{
    public partial class Perguntas : ContentPage
    {
        // =====================================================
        // 1) Campos de estado
        // =====================================================
        private int extraIndex = 0;            // índice atual de Perguntas Extras
        private int themeIndex = 0;            // índice atual de Perguntas de Tema
        private bool inThemePhase = false;     // indica quando já passamos pelas extras e entramos nos temas

        // guarda os IDs selecionados
        private readonly List<int> selectedExtraAnswerIds = new();
        private readonly List<int> selectedThemeAnswerIds = new();

        // Listas carregadas da API
        private List<ExtraQuestionDTO> extraQuestions = new();
        private List<ThemeQuestionDTO> themeQuestions = new();

        // HttpClient para chamadas
        private readonly HttpClient _httpClient;

        // ==============================================
        // 2) Construtor / OnAppearing
        // ==============================================
        public Perguntas()
        {
            InitializeComponent();

            // Inicializa HttpClient 
            _httpClient = new HttpClient();

            // Vincular o BindingContext para poder usar Binding (não estritamente necessário aqui, mas mantido)
            BindingContext = this;

            // Inicia carregamento das perguntas da API
            _ = CarregarPerguntasDaApi();
        }

        // ==============================================
        // 3) CarregarPerguntasDaApi()
        // ==============================================
        private async Task CarregarPerguntasDaApi()
        {
            try
            {
                // URL da API
                var url = "http://192.168.0.105:5249/api/v1/questions/general-questions";
                // Faz GET
                var resp = await _httpClient.GetAsync(url);

                if (!resp.IsSuccessStatusCode)
                {
                    await DisplayAlert("Erro", "Não foi possível obter perguntas do servidor.", "OK");
                    return;
                }

                var json = await resp.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dto = JsonSerializer.Deserialize<GeneralQuestionsResponseDTO>(json, options);

                if (dto == null ||
                    dto.ExtraQuestions == null || dto.ExtraQuestions.Count == 0 ||
                    dto.ThemeQuestions == null || dto.ThemeQuestions.Count == 0)
                {
                    await DisplayAlert("Aviso", "Não há perguntas disponíveis.", "OK");
                    return;
                }

                // Guarda localmente
                extraQuestions = dto.ExtraQuestions;
                themeQuestions = dto.ThemeQuestions;

                // Inicia na fase Extras
                extraIndex = 0;
                inThemePhase = false;

                // Desabilita botão Voltar no início
                BtnVoltar.IsEnabled = false;

                // Ajusta SelectionMode do CollectionView para a primeira pergunta de extras
                var primeiraExtra = extraQuestions[0];
                if (primeiraExtra.IsMultipleChoice)
                {
                    OpcoesCollection.SelectionMode = SelectionMode.Multiple;
                }
                else
                {
                    OpcoesCollection.SelectionMode = SelectionMode.Single;
                }

                // Exibe a primeira Pergunta Extra
                MostrarPerguntaAtual();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exceção", $"{ex.Message}\nExceção Interna:\n{ex.InnerException}", "OK");
            }
        }

        // ==============================================
        // 4) MostrarPerguntaAtual()
        // ==============================================
        private void MostrarPerguntaAtual()
        {
            // ---------- Fase Extras ----------
            if (!inThemePhase)
            {
                // Se acabaram as extras, passamos para Tema
                if (extraIndex >= extraQuestions.Count)
                {
                    inThemePhase = true;
                    themeIndex = 0;

                    // Ajusta SelectionMode para a primeira pergunta de tema
                    var primeiraTema = themeQuestions[0];
                    OpcoesCollection.SelectionMode = primeiraTema.IsMultipleChoice
                        ? SelectionMode.Multiple
                        : SelectionMode.Single;

                    // Habilita Voltar (para poder voltar à última extra)
                    BtnVoltar.IsEnabled = true;

                    // Mostra a primeira pergunta de tema
                    MostrarPerguntaAtual();
                    return;
                }

                // Ainda estamos em uma pergunta de extras
                var questExtra = extraQuestions[extraIndex];
                PerguntaLabel.Text = questExtra.Text;

                // Define se é Single ou Multiple
                OpcoesCollection.SelectionMode = questExtra.IsMultipleChoice
                    ? SelectionMode.Multiple
                    : SelectionMode.Single;

                // Popula com as alternativas de ExtraAnswerDTO
                OpcoesCollection.ItemsSource = questExtra.ExtraAnswers;

                // Botão Voltar habilitado apenas se extraIndex > 0
                BtnVoltar.IsEnabled = (extraIndex > 0);

                // Limpa seleções anteriores
                OpcoesCollection.SelectedItem = null;
                OpcoesCollection.SelectedItems?.Clear();
            }
            // ---------- Fase Tema ----------
            else
            {
                // Se acabaram as temas, navegar pra Cadastro
                if (themeIndex >= themeQuestions.Count)
                {
                    NavegarParaCadastro();
                    return;
                }

                var questTema = themeQuestions[themeIndex];
                PerguntaLabel.Text = questTema.Text;

                // Define se é Single ou Multiple
                OpcoesCollection.SelectionMode = questTema.IsMultipleChoice
                    ? SelectionMode.Multiple
                    : SelectionMode.Single;

                // Popula com alternativas de ThemeAnswerDTO
                OpcoesCollection.ItemsSource = questTema.ThemeAnswers;


                // Botão Voltar sempre habilitado na fase de tema
                BtnVoltar.IsEnabled = true;

                // Limpa seleções anteriores
                OpcoesCollection.SelectedItem = null;
                OpcoesCollection.SelectedItems?.Clear();
            }
        }

        // ==============================================
        // 5) OnNextClicked
        // ==============================================
        private void OnNextClicked(object sender, EventArgs e)
        {
            // Se for multiple
            var selecionados = OpcoesCollection.SelectedItems;
            // Se for single
            var selecionado = OpcoesCollection.SelectedItem;

            // ---------- se for fase Extras ----------
            if (!inThemePhase)
            {
                var questExtra = extraQuestions[extraIndex];

                if (questExtra.IsMultipleChoice)
                {
                    if (selecionados == null || selecionados.Count == 0)
                    {
                        DisplayAlert("Atenção", "Por favor, escolha ao menos uma opção para prosseguir.", "OK");
                        return;
                    }

                    // Multi → adiciona todos os IDs selecionados
                    foreach (var obj in selecionados)
                    {
                        var extraEscolhido = (ExtraAnswerDTO)obj;
                        if (!selectedExtraAnswerIds.Contains(extraEscolhido.Id))
                            selectedExtraAnswerIds.Add(extraEscolhido.Id);
                    }
                }
                else
                {
                    if (selecionado == null)
                    {
                        DisplayAlert("Atenção", "Por favor, escolha ao menos uma opção para prosseguir.", "OK");
                        return;
                    }

                    // Single → pega o selecionado
                    var extraEscolhido = (ExtraAnswerDTO)selecionado;
                    selectedExtraAnswerIds.Add(extraEscolhido.Id);
                }

                extraIndex++;
                MostrarPerguntaAtual();
            }
            // ---------- se for fase Tema ----------
            else
            {
                var questTema = themeQuestions[themeIndex];

                if (questTema.IsMultipleChoice)
                {
                    if (selecionados == null || selecionados.Count == 0)
                    {
                        DisplayAlert("Atenção", "Por favor, escolha ao menos uma opção para prosseguir.", "OK");
                        return;
                    }

                    foreach (var obj in selecionados)
                    {
                        var temaEscolhido = (ThemeAnswerDTO)obj;
                        if (!selectedThemeAnswerIds.Contains(temaEscolhido.Id))
                            selectedThemeAnswerIds.Add(temaEscolhido.Id);
                    }
                }
                else
                {
                    if (selecionado == null)
                    {
                        DisplayAlert("Atenção", "Por favor, escolha ao menos uma opção para prosseguir.", "OK");
                        return;
                    }

                    var temaEscolhido = (ThemeAnswerDTO)selecionado;
                    selectedThemeAnswerIds.Add(temaEscolhido.Id);
                }

                themeIndex++;
                MostrarPerguntaAtual();
            }
        }

        // ==============================================
        // 6) OnBackClicked
        // ==============================================
        private void OnBackClicked(object sender, EventArgs e)
        {
            try
            {
                // ---------- Fase Extras ----------
                if (!inThemePhase)
                {
                    // Se tava na primeira pergunta
                    if (extraIndex <= 0) return;

                    // Questão atual
                    var questAtual = extraQuestions[extraIndex];
                    // Remove todas as respostas dessa pergunta dos IDs selecionados
                    foreach (var ans in questAtual.ExtraAnswers)
                    {
                        selectedExtraAnswerIds.Remove(ans.Id);
                    }

                    // Questão anterior
                    var questAnterior = extraQuestions[extraIndex - 1];
                    // Remove todas as respostas dessa pergunta dos IDs selecionados
                    foreach (var ans in questAnterior.ExtraAnswers)
                    {
                        selectedExtraAnswerIds.Remove(ans.Id);
                    }

                    // Limpa seleções anteriores
                    OpcoesCollection.SelectedItem = null;
                    OpcoesCollection.SelectedItems?.Clear();

                    extraIndex--;
                    MostrarPerguntaAtual();
                }
                // ---------- Fase Tema ----------
                else
                {
                    if (themeIndex <= 0)
                    {
                        // voltar para a última pergunta Extra
                        inThemePhase = false;
                        extraIndex = extraQuestions.Count - 1;

                        // Questão extra anterior
                        var extraQuestAnterior = extraQuestions[extraIndex];
                        // Remove todas as respostas dessa pergunta dos IDs selecionados
                        foreach (var ans in extraQuestAnterior.ExtraAnswers)
                        {
                            selectedExtraAnswerIds.Remove(ans.Id);
                        }

                        MostrarPerguntaAtual();
                        return;
                    }

                    // Questão atual
                    var questAtual = themeQuestions[themeIndex];
                    // Remove todas as respostas dessa pergunta dos IDs selecionados
                    foreach (var ans in questAtual.ThemeAnswers)
                    {
                        selectedThemeAnswerIds.Remove(ans.Id);
                    }

                    // Questão anterior
                    var questAnterior = themeQuestions[themeIndex - 1];
                    // Remove todas as respostas dessa pergunta dos IDs selecionados
                    foreach (var ans in questAnterior.ThemeAnswers)
                    {
                        selectedThemeAnswerIds.Remove(ans.Id);
                    }

                    // Limpa seleções anteriores
                    OpcoesCollection.SelectedItem = null;
                    OpcoesCollection.SelectedItems?.Clear();

                    themeIndex--;
                    MostrarPerguntaAtual();
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Exceção", ex.Message, "Ok");
            }
            
        }

        // ==============================================
        // 7) NavegarParaCadastro
        // ==============================================
        private void NavegarParaCadastro()
        {
            // Exibe apenas um log de depuração
            var msg =
                $"Extras selecionados: {string.Join(",", selectedExtraAnswerIds)}\n" +
                $"Tema selecionados: {string.Join(",", selectedThemeAnswerIds)}";

            // Em produção, faça algo como:
            // Navigation.PushAsync(new Cadastro(selectedExtraAnswerIds, selectedThemeAnswerIds));
            DisplayAlert("Respostas concluídas", msg, "OK");
        }

        // ==============================================
        // 7) Frame_Tapped
        // ==============================================
        private void Frame_Tapped(object sender, EventArgs e)
        {
            // Verifica se o sender é um Frame e se tem BindingContext
            if (sender is Frame selectedFrame && selectedFrame.BindingContext != null)
            {
                

                // Se for um Frame, pega o BindingContext
                var item = selectedFrame.BindingContext;

                // Verifica se é múltipla escolha ou única
                if (OpcoesCollection.SelectionMode == SelectionMode.Single)
                {
                    // Se já estava selecionado, desseleciona; caso contrário, seleciona
                    if (OpcoesCollection.SelectedItem == item)
                    {
                        OpcoesCollection.SelectedItem = null;
                    }
                    else
                    {
                        OpcoesCollection.SelectedItem = item;
                    }
                }
                else if (OpcoesCollection.SelectionMode == SelectionMode.Multiple)
                {
                    var selected = OpcoesCollection.SelectedItems;

                    if (selected.Contains(item))
                        selected.Remove(item);  
                    else
                        selected.Add(item);
                }
            }
        }
    }
}
