using LumeClient.Config;
using LumeClient.DTOs.Movies;
using LumeClient.DTOs.Users; // FindMyIdDTO, se aplicável
using System.Net.Http.Headers;
using System.Text.Json;

namespace LumeClient.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private string userId;
        private List<MovieItemDTO> recommended = new();
        private Dictionary<int, MovieDetailsDTO> detailsCache = new();


        public MainPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool sair = await DisplayAlert("Sair do Lume?",
                    "Deseja fechar o aplicativo?",
                    "Sim", "Cancelar");

                if (sair)
                {
#if ANDROID
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#endif
                }
            });

            return true; // cancela o comportamento padrão
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCarouselAsync();
        }

        private async Task LoadCarouselAsync()
        {
            CarouselLoadingOverlay.IsVisible = true;
            frameNoRecommendations.IsVisible = false;
            RecommendationsCollection.ItemsSource = null;
            detailsCache.Clear();
            DetailsContainer.Children.Clear();

            try
            {
                // 1) obter token
                var token = await SecureStorage.Default.GetAsync("access_token");
                if (string.IsNullOrEmpty(token))
                {
                    // talvez redirecionar ao login
                    await DisplayAlert("Atenção", "Usuário não autenticado.", "OK");
                    return;
                }
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // 2) /users/me
                var respMe = await _httpClient.GetAsync(APIConfig.MyIdEndpoint);
                if (!respMe.IsSuccessStatusCode)
                {
                    // sem permissão ou token inválido
                    await DisplayAlert("Erro", "Não foi possível obter dados do usuário.", "OK");
                    return;
                }
                var jsonMe = await respMe.Content.ReadAsStringAsync();
                var meDto = JsonSerializer.Deserialize<FindMyIdDTO>(jsonMe, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                userId = meDto?.Id;
                if (string.IsNullOrEmpty(userId))
                {
                    await DisplayAlert("Erro", "Resposta inválida ao obter ID do usuário.", "OK");
                    return;
                }

                // 3) /movies/carousel-movies/{userId}
                var carouselUrl = $"{APIConfig.CarouselMoviesEndpoint}{userId}";
                var respCar = await _httpClient.GetAsync(carouselUrl);
                if (!respCar.IsSuccessStatusCode)
                {
                    await DisplayAlert("Erro", "Falha ao obter recomendações.", "OK");
                    return;
                }
                var jsonCar = await respCar.Content.ReadAsStringAsync();
                // desserializa array de objetos com id, title, posterPath
                var listDto = JsonSerializer.Deserialize<List<MovieItemDTO>>(jsonCar,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                // MovieItemDTO: supomos uma classe com Id, Title, PosterPath
                if (listDto == null || listDto.Count == 0)
                {
                    frameNoRecommendations.IsVisible = true;
                    return;
                }

                string baseImageUrl = "https://image.tmdb.org/t/p/w500";

                // converte para MovieItem
                recommended = listDto.Select(m => new MovieItemDTO
                {
                    Id = m.Id,
                    Title = m.Title ?? "",
                    PosterPath = string.IsNullOrEmpty(m.PosterPath)
                        ? "no_poster_card.png"
                        : $"{baseImageUrl}{m.PosterPath}"
                }).ToList();

                RecommendationsCollection.ItemsSource = recommended;

                await ShowMovieDetailsAsync(recommended.FirstOrDefault()?.Id ?? 0);
            }
            catch (Exception ex)
            { 
            }
            finally
            {
                CarouselLoadingOverlay.IsVisible = false;
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private async void OnRecommendationTapped(object sender, EventArgs e)
        {
            if (!(sender is Frame frame && frame.BindingContext is MovieItemDTO item))
                return;

            await ShowMovieDetailsAsync(item.Id);
        }

        private async Task ShowMovieDetailsAsync(int movieId)
        {
            // Limpa container antes de exibir novo (ou você pode manter anteriores, mas aqui substituímos)
            DetailsContainer.Children.Clear();

            // Se já no cache, use direto
            if (detailsCache.TryGetValue(movieId, out var cached))
            {
                PopulateDetails(cached);
                return;
            }

            // Senão, busca na API
            // Exibe indicador local de loading de detalhes (por ex. um Label “Carregando…”)
            var loadingLabel = new Label
            {
                Text = "Carregando detalhes...",
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center
            };
            DetailsContainer.Children.Add(loadingLabel);

            try
            {
                // Adiciona token
                var token = await SecureStorage.Default.GetAsync("access_token");
                if (string.IsNullOrEmpty(token))
                {
                    await DisplayAlert("Atenção", "Usuário não autenticado.", "OK");
                    DetailsContainer.Children.Clear();
                    return;
                }
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // Monta URL de detalhes:
                // conforme especificado: /api/v1/movies/wishlisted-movies/{userId}/wishlisted-movies/{movieId}
                var detailUrl = $"{APIConfig.WishlistMoviesByIdFirstPartEndpoint}{userId}{APIConfig.WishlistMoviesByIdLastPartEndpoint}{movieId}";
                var resp = await _httpClient.GetAsync(detailUrl);
                if (!resp.IsSuccessStatusCode)
                {
                    await DisplayAlert("Erro", "Falha ao obter detalhes do filme.", "OK");
                    DetailsContainer.Children.Clear();
                    return;
                }
                var json = await resp.Content.ReadAsStringAsync();
                var movieDetails = JsonSerializer.Deserialize<MovieDetailsDTO>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (movieDetails == null)
                {
                    await DisplayAlert("Erro", "Resposta inválida dos detalhes.", "OK");
                    DetailsContainer.Children.Clear();
                    return;
                }
                // armazena em cache
                detailsCache[movieId] = movieDetails;
                // remove loadingLabel e popula
                DetailsContainer.Children.Clear();
                PopulateDetails(movieDetails);
            }
            catch (Exception ex)
            {
                DetailsContainer.Children.Clear();
                await DisplayAlert("Erro", $"Erro ao buscar detalhes: {ex.Message}", "OK");
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private void PopulateDetails(MovieDetailsDTO m)
        {
            // Exibe título
            DetailsContainer.Children.Add(new Label
            {
                Text = m.Title ?? "(Sem título)",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black
            });
            // Linha de info: ano | duração | gêneros
            string year = m.ReleaseDate?.Year.ToString() ?? "-";
            string dur = m.Runtime > 0 ? $"{m.Runtime} min" : "-";
            string genres = (m.Genres != null && m.Genres.Any()) ? string.Join(", ", m.Genres) : "-";
            DetailsContainer.Children.Add(new Label
            {
                Text = $"{year} | {dur} | {genres}",
                FontSize = 14,
                TextColor = Colors.Gray
            });
            // Overview
            DetailsContainer.Children.Add(new Label
            {
                Text = m.Overview ?? "Sem sinopse disponível.",
                FontSize = 14,
                TextColor = Colors.Black
            });
            // Se quiser, adicionar mais seções conforme DTO:
            // Tagline
            if (!string.IsNullOrWhiteSpace(m.Tagline))
            {
                DetailsContainer.Children.Add(new Label
                {
                    Text = $"\"{m.Tagline}\"",
                    FontSize = 14,
                    FontAttributes = FontAttributes.Italic,
                    TextColor = Colors.DarkGray
                });
            }
            // Popularidade e avaliação
            DetailsContainer.Children.Add(new Label
            {
                Text = $"⭐ Nota: {m.VoteAverage:F1} ({m.VoteCount} votos) | Popularidade: {m.Popularity}",
                FontSize = 14,
                TextColor = Colors.Black
            });
            // Produção: orçamento e receita
            if (m.Budget > 0 || m.Revenue > 0)
            {
                string buf = m.Budget > 0 ? $"Orçamento: ${m.Budget:N0}" : "Orçamento não disponível.";
                string rev = m.Revenue > 0 ? $"Receita: ${m.Revenue:N0}" : "Receita não disponível.";
                DetailsContainer.Children.Add(new Label
                {
                    Text = string.Join(" | ", new[] { buf, rev }.Where(s => !string.IsNullOrEmpty(s))),
                    FontSize = 14,
                    TextColor = Colors.Black
                });
            }
            // Homepage como link, se houver
            if (!string.IsNullOrWhiteSpace(m.Homepage))
            {
                var lblLink = new Label
                {
                    Text = "Site oficial",
                    FontSize = 14,
                    TextColor = Colors.Blue,
                    GestureRecognizers =
                    {
                        new TapGestureRecognizer {
                            Command = new Command(async () => {
                                try {
                                    await Launcher.OpenAsync(m.Homepage);
                                } catch { }
                            })
                        }
                    }
                };
                DetailsContainer.Children.Add(lblLink);
            }
            // Outras seções opcionais: spokenLanguages, productionCompanies, etc.
            if (m.ProductionCompanies?.Any() == true)
            {
                DetailsContainer.Children.Add(new Label
                {
                    Text = "Produtoras: " + string.Join(", ", m.ProductionCompanies),
                    FontSize = 14,
                    TextColor = Colors.Black
                });
            }
            if (m.ProductionCountries?.Any() == true)
            {
                DetailsContainer.Children.Add(new Label
                {
                    Text = "Países: " + string.Join(", ", m.ProductionCountries),
                    FontSize = 14,
                    TextColor = Colors.Black
                });
            }
            if (m.SpokenLanguages?.Any() == true)
            {
                DetailsContainer.Children.Add(new Label
                {
                    Text = "Idiomas: " + string.Join(", ", m.SpokenLanguages),
                    FontSize = 14,
                    TextColor = Colors.Black
                });
            }
            if (m.Keywords?.Any() == true)
            {
                DetailsContainer.Children.Add(new Label
                {
                    Text = "Palavras-chave: " + string.Join(", ", m.Keywords),
                    FontSize = 14,
                    TextColor = Colors.Black
                });
            }
            // etc conforme necessidade
        }
    }

    // DTO resultado do carousel
    public class MovieItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PosterPath { get; set; }
    }
}
