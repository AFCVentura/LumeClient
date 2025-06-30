using CommunityToolkit.Maui.Views;
using LumeClient.Config;
using LumeClient.DTOs.Movies;
using System.Text.Json;

namespace LumeClient.Views
{
    public partial class SwipeResult : ContentPage
    {
        // Lista de itens escolhidos (já com PosterFullUrl preenchido)
        private readonly List<MovieDetailsDTO> _chosenMovies;

        // Se precisar de HttpClient para buscar detalhes, injete ou crie
        private readonly HttpClient _httpClient = new HttpClient();

        // Cache de detalhes para não refazer requisição
        private readonly Dictionary<int, MovieDetailsDTO> _detailsCache = new();

        // userId, caso necessário para detalhes
        private readonly string _userId;

        // Construtor recebe lista de MovieItem e userId (se precisar)
        public SwipeResult(List<MovieDetailsDTO> chosenMovies, string userId)
        {
            InitializeComponent();

            _chosenMovies = chosenMovies ?? new List<MovieDetailsDTO>();
            _userId = userId;

            // Ajusta header com contagem
            lblHeader.Text = $"Você escolheu {_chosenMovies.Count} filme{(_chosenMovies.Count == 1 ? "" : "s")}";

            // Define ItemsSource
            string baseImageUrl = "https://image.tmdb.org/t/p/w500";

            _chosenMovies.ForEach(m => m.PosterPath = string.IsNullOrEmpty(m.PosterPath)
                ? "no_poster_card.png"
                : $"{baseImageUrl}{m.PosterPath}");

            ChosenCollection.ItemsSource = _chosenMovies;
            
        }
        protected override bool OnBackButtonPressed()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new MainPage());
                // Impede o comportamento padrão do botão Voltar
            });
            return true;
        }

        // Evento de tap em cada poster
        private async void OnMovieTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is MovieDetailsDTO item)
            {
                // Exibe detalhes: ou navega para página de detalhes, ou popup
                await ShowMovieDetailsAsync(item.Id);
            }
        }

        // Exemplo de exibir detalhes: navega para uma página ou usa popup
        private async Task ShowMovieDetailsAsync(int movieId)
        {
            // Se cache tem, use diretamente
            if (_detailsCache.TryGetValue(movieId, out var cached))
            {
                // Use cached para exibir
                MovieInfoPopup popup = null;

                try
                {
                    string baseImageUrl = "https://image.tmdb.org/t/p/w500";

                    var movie = _chosenMovies.FirstOrDefault(m => m.Id == movieId);
                    movie.PosterPath = string.IsNullOrEmpty(movie.PosterPath)
                        ? "no_poster_card.png"
                        : $"{baseImageUrl}{movie.PosterPath}";
                        
                    popup = new MovieInfoPopup(movie);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao criar popup: {ex.Message}");
                    await DisplayAlert("Erro", "Falha ao criar popup: " + ex.Message, "OK");
                    return;
                }

                // Exibe o popup
                await this.ShowPopupAsync(popup);
                return;
            }

            // Caso precise buscar da API:
            try
            {
                // Obtenha token salvo se necessário
                var token = await SecureStorage.Default.GetAsync("access_token");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                // Monta URL de detalhes conforme seu endpoint: ex:
                // /api/v1/movies/wishlist-movies/{userId}/wishlisted-movies/{movieId}
                var url = $"{APIConfig.WishlistMoviesByIdFirstPartEndpoint}{_userId}{APIConfig.WishlistMoviesByIdLastPartEndpoint}{movieId}";
                var resp = await _httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    await DisplayAlert("Erro", "Não foi possível obter detalhes do filme.", "OK");
                    return;
                }
                var json = await resp.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<MovieDetailsDTO>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (dto != null)
                {
                    _detailsCache[movieId] = dto;
                    

                    MovieInfoPopup popup = null;
                    try
                    {
                        var movie = _chosenMovies.FirstOrDefault(m => m.Id == movieId);
                        popup = new MovieInfoPopup(movie);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao criar popup: {ex.Message}");
                        await DisplayAlert("Erro", "Falha ao criar popup: " + ex.Message, "OK");
                        return;
                    }

                    // Exibe o popup
                    await this.ShowPopupAsync(popup);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao carregar detalhes: {ex.Message}", "OK");
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // Botão “Ir para Wishlist”
        private async void OnGoWishlistClicked(object sender, EventArgs e)
        {
            // Navega para a página de wishlist
            await Navigation.PushAsync(new Wishlist());
        }
    }

}