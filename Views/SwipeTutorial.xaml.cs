using CommunityToolkit.Maui.Views;
using LumeClient.Config;
using LumeClient.DTOs.Movies;
using LumeClient.DTOs.Questions;
using System.Text.Json;
using static LumeClient.Views.InicioCadastro;

namespace LumeClient.Views;

public partial class SwipeTutorial : ContentPage
{
    // Campo que recebe os ids das perguntas da tela anterior
    private readonly List<int> selectedExtraAnswerIds = new();
    private readonly List<int> selectedThemeAnswerIds = new();

    // Campo com os filmes famosos vindos do LumeServer
    List<MovieDetailsDTO> Movies { get; set; } = new();
    // Campos para armazenar os ids dos filmes escolhidos, recusados e já vistos pelo usuário
    List<int> ChosenMovieIds { get; set; } = new();

    // HttpClient para chamadas
    private readonly HttpClient _httpClient;

    // Atributos usados no swipe
    private double startX, startY;

    // Atributo usado para controlar o índice do filme atual
    private int currentIndex = 0;

    private string basePosterPathURL = "https://image.tmdb.org/t/p/w500";

    public SwipeTutorial(List<int> selectedExtraAnswerIds, List<int> selectedThemeAnswerIds)
	{
		InitializeComponent();
        // Inicializa HttpClient 
        _httpClient = new HttpClient();

        this.selectedExtraAnswerIds = selectedExtraAnswerIds;
        this.selectedThemeAnswerIds = selectedThemeAnswerIds;
    }

    protected override async void OnAppearing()
    {
        
        base.OnAppearing();
        await CarregarFilmesDaAPI();
    }

    // ==============================================
    // CarregarFilmesDaApi()
    // ==============================================
    private async Task CarregarFilmesDaAPI()
    {
        try
        {
            // Define a URL
            var url = APIConfig.FamousMoviesEndpoint;

            // Faz a requisição HTTP
            var resp = await _httpClient.GetAsync(url);

            // Se a requisição tiver Status Code diferente de 200 (OK)
            if (!resp.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", "Não foi possível obter perguntas do servidor.", "OK");
                return;
            }

            // Desserializa o conteúdo da resposta para uma lista de MovieDetailsDTO
            var json = await resp.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dto = JsonSerializer.Deserialize<List<MovieDetailsDTO>>(json, options);

            // Se a lista estiver vazia ou nula
            if (dto is null || dto.Count == 0)
            {
                await DisplayAlert("Aviso", "Não há filmes disponíveis.", "OK");
                return;
            }

            // Guarda localmente os filmes 
            Movies = dto;

            // Garante que o índice começa zerado
            currentIndex = 0;

            CarregarFilme();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exceção", $"{ex.Message}\nExceção Interna:\n{ex.InnerException}", "OK");
        }
    }

    private void CarregarFilme()
    {
        // Caso haja problemas com o índice
        if (currentIndex < 0) currentIndex = 0;

        // Se acabou a lista, por enquanto reinicia
        if (currentIndex >= Movies.Count)
        {
            //List<MovieDetailsDTO> wishedMovies = ChosenMovieIds.Count > 0 ? Movies.Where(m => ChosenMovieIds.Contains(m.Id)).ToList() : new List<MovieDetailsDTO>();
            //var wishedTitles = wishedMovies.Select(m => m.Title).ToList();
            //string wished = wishedMovies.Count > 0 ? string.Join(", ", wishedTitles) : "Nenhum";
            //DisplayAlert("Fim da Lista", $"Wishlist: {wished}.", "Ok");

            Navigation.PushAsync(new InicioCadastro(EtapasCadastroEnum.PreCadastro, selectedExtraAnswerIds, selectedThemeAnswerIds, ChosenMovieIds));

            currentIndex = 0;
        }

        // Carrega o filme atual
        var m = Movies[currentIndex];
        MovieTitle.Text = m.Title;
        var posterPath = basePosterPathURL + m.PosterPath;
        MovieImage.Source = posterPath;
    }
    // Método OnCardTapped (quando clica):
    private async void OnCardTapped(object sender, EventArgs e)
    {
        // Impede abrir popup logo após um swipe: opcionalmente, verifique se não houve movimento recente
        // Mas na prática, um swipe engatilha Pan, não Tap. Então:
        var movie = Movies[currentIndex];
        // Crie popup com dados completos
        MovieInfoPopup popup = null;

        try
        {
            popup = new MovieInfoPopup(movie);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar popup: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert("Erro", "Falha ao criar popup: " + ex.Message, "OK");
            return;
        }

        // Exibe o popup
        await this.ShowPopupAsync(popup);
    }

    // Método PanUpdated (quando arrasta):
    private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                startX = CardFrame.TranslationX;
                startY = CardFrame.TranslationY;
                break;

            case GestureStatus.Running:
                double targetX = startX + e.TotalX;
                double targetY = startY + e.TotalY;
                double currentX = CardFrame.TranslationX;
                double currentY = CardFrame.TranslationY;
                const double smoothing = 0.2;
                CardFrame.TranslationX = currentX + (targetX - currentX) * smoothing;
                CardFrame.TranslationY = currentY + (targetY - currentY) * smoothing;
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                double finalDx = CardFrame.TranslationX - startX;
                double finalDy = CardFrame.TranslationY - startY;
                double parentWidth = ((VisualElement)CardFrame.Parent).Width;
                double parentHeight = ((VisualElement)CardFrame.Parent).Height;

                if (finalDx > 50)
                {
                    await LikeMovie(Movies[currentIndex].Id, parentWidth);
                }
                else if (finalDx < -50)
                {
                    await DislikeMovie(parentWidth);
                }
                else
                {
                    await CardFrame.TranslateTo(0, 0, 150, Easing.Linear);
                }

                ResetCardPosition();
                break;
        }
    }

    private async Task LikeMovie(int movieId, double parentWidth)
    {
        ChosenMovieIds.Add(movieId);
        await CardFrame.TranslateTo(parentWidth, 0, 250, Easing.CubicOut);
        currentIndex++;
        CarregarFilme();
    }

    private async Task DislikeMovie(double parentWidth)
    {
        await CardFrame.TranslateTo(-parentWidth, 0, 250, Easing.CubicOut);
        currentIndex++;
        CarregarFilme();
    }

    private async void OnLikeClicked(object sender, EventArgs e)
    {
        double parentWidth = ((VisualElement)CardFrame.Parent).Width;
        await LikeMovie(Movies[currentIndex].Id, parentWidth);
        ResetCardPosition();
    }

    private async void OnDislikeClicked(object sender, EventArgs e)
    {
        double parentWidth = ((VisualElement)CardFrame.Parent).Width;
        await DislikeMovie(parentWidth);
        ResetCardPosition();
    }
    private void ResetCardPosition()
    {
        CardFrame.TranslationX = 0;
        CardFrame.TranslationY = 0;
        CardFrame.Rotation = 0;
        CardFrame.Opacity = 1;
    }
}