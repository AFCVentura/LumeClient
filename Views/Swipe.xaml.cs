using CommunityToolkit.Maui.Views;
using LumeClient.Config;
using LumeClient.DTOs.Movies;
using LumeClient.DTOs.Users;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LumeClient.Views
{
    public partial class Swipe : ContentPage
    {
        // Campo com os filmes famosos vindos do LumeServer
        List<MovieDetailsDTO> Movies { get; set; } = new();
        // Campos para armazenar os ids dos filmes escolhidos, recusados e já vistos pelo usuário
        List<int> WishlistedMovieIds { get; set; } = new();
        List<int> RefusedMovieIds { get; set; } = new();
        List<int> WatchedMovieIds { get; set; } = new();

        // HttpClient para chamadas
        private readonly HttpClient _httpClient = new HttpClient();

        // Atributos usados no swipe
        private double startX, startY;

        // Atributo usado para controlar o índice do filme atual
        private int currentIndex = 0;

        private string basePosterPathURL = "https://image.tmdb.org/t/p/w500";

        public Swipe()
        {
            InitializeComponent();         
        }

        protected override async void OnAppearing()
        {

            base.OnAppearing();


            // Monta as mensagens e GIFs
            var messages = new List<string>
            {
                "Bem vindo ao Swipe do dia a dia, aqui as coisas são parecidas com quando você criou a conta, mas é importante ressaltar as diferenças",
                "Agora os filmes irão aparecer com base nas respostas que você deu no questionário anterior",
                "Além disso, você terá três possíveis ações ao invés de duas",
                "Para a direita caso goste da sugestão",
                "Para a esquerda caso não goste da sugestão",
                "E para cima caso já tenha visto o filme",
                "Divirta-se!"
             };
            // GIFs correspondentes
            var gifFiles = new List<string?>
            {
                null, // passo 1: só texto
                null, // passo 2: só texto
                null, // passo 3: ou coloque "tap_card.gif"
                "lume_swipe_direita.mp4",  // passo 4
                "lume_swipe_esquerda.mp4", // passo 5
                "lume_swipe_cima.mp4",  // passo 6
                null
            };

            // Exibe o popup de tutorial
            var tutorial = new TutorialPopup(messages, gifFiles);
            await this.ShowPopupAsync(tutorial);

            await CarregarFilmesDaAPI();
        }

        private bool _isBackConfirmationOpen = false;

        protected override bool OnBackButtonPressed()
        {
            if (_isBackConfirmationOpen)
                return true;

            _isBackConfirmationOpen = true;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool confirmar = await DisplayAlert("Atenção",
                    "Você irá perder todo o progresso. Deseja realmente voltar ao início?",
                    "Sim", "Cancelar");

                if (confirmar)
                    await Navigation.PushAsync(new MainPage());

                _isBackConfirmationOpen = false;
            });

            return true;
        }

        // ==============================================
        // CarregarFilmesDaApi()
        // ==============================================
        private async Task CarregarFilmesDaAPI()
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("access_token");
                var userId = await FindMyId(token);

                _httpClient.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("Bearer", token);

                // Define a URL
                var url = $"{APIConfig.RecommendedMoviesGetEndpoint}{userId}";

                // Faz a requisição HTTP
                var resp = await _httpClient.GetAsync(url);

                // Se a requisição tiver Status Code diferente de 200 (OK)
                if (!resp.IsSuccessStatusCode)
                {
                    await DisplayAlert("Erro", "Não foi possível obter os filmes no servidor.", "OK");
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

                await CarregarFilme();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Exceção", $"{ex.Message}\nExceção Interna:\n{ex.InnerException}", "OK");
            }
            finally
            {
                // Remova o header para não afetar outras requisições
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private async Task CarregarFilme()
        {
            // Caso haja problemas com o índice
            if (currentIndex < 0) currentIndex = 0;

            // Se acabou a lista, por enquanto reinicia
            if (currentIndex >= Movies.Count)
            {
                // Se esgotou a lista, você pode:
                // - Reiniciar: currentIndex = 0; CarregarFilme();
                // - Ou exibir mensagem/finalizar fluxo
                // Por enquanto, vamos reiniciar:
                //string watched = WatchedMovies.Count > 0 ? string.Join(", ", WatchedMovies) : "Nenhum";
                //string wished = WishlistedMovies.Count > 0 ? string.Join(", ", WishlistedMovies) : "Nenhum";
                //string refused = RefusedMovies.Count > 0 ? string.Join(", ", RefusedMovies) : "Nenhum";
                //DisplayAlert("Fim da Lista", $"Wishlist: {wished}. Watched: {watched}. Recusados: {refused}", "Ok");

                //Navigation.PushAsync(new InicioCadastro(EtapasCadastroEnum.PreCadastro, selectedExtraAnswerIds, selectedThemeAnswerIds, WishlistedMovies));

                await EnviarFilmes();


                currentIndex = 0;
            }

            // Carrega o filme atual
            var m = Movies[currentIndex];
            MovieTitle.Text = m.Title;
            var posterPath = basePosterPathURL + m.PosterPath;
            MovieImage.Source = posterPath;
        }

        private async Task EnviarFilmes()
        {
            var token = await SecureStorage.Default.GetAsync("access_token");
            var userId = await FindMyId(token);

            _httpClient.DefaultRequestHeaders.Authorization =
           new AuthenticationHeaderValue("Bearer", token);

            // Define a URL
            var url = $"{APIConfig.RecommendedMoviesPostEndpoint}{userId}";

            var payload = new List<RecommendedMovieStatusDTO>();

            foreach (var movieId in RefusedMovieIds) 
            {
                payload.Add(new RecommendedMovieStatusDTO
                {
                    MovieId = movieId,
                    Watched = false,
                    Liked = false
                });
            }

            foreach (var movieId in WatchedMovieIds)
            {
                payload.Add(new RecommendedMovieStatusDTO
                {
                    MovieId = movieId,
                    Watched = true,
                    Liked = false
                });
            }

            foreach (var movieId in WishlistedMovieIds)
            {
                payload.Add(new RecommendedMovieStatusDTO
                {
                    MovieId = movieId,
                    Watched = false,
                    Liked = true
                });
            }
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Faz a requisição HTTP
            var resp = await _httpClient.PostAsync(url, content);

            // Se a requisição tiver Status Code diferente de 200 (OK)
            if (!resp.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", "Não foi possível enviar os filmes ao servidor.", "OK");
                return;
            }

            var chosenMovies = new List<MovieDetailsDTO>();

            foreach (var movieId in WishlistedMovieIds)
            {
                var movie = Movies.FirstOrDefault(m => m.Id == movieId);
                if (movie is not null)
                {
                    chosenMovies.Add(movie);
                }
            }

            await Navigation.PushAsync(new SwipeResult(chosenMovies, userId));

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
                await DisplayAlert("Erro", "Falha ao criar popup: " + ex.Message, "OK");
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

                    if (finalDy < -75)
                    {
                        await WachedMovie(Movies[currentIndex].Id, parentHeight);
                    }
                    if (finalDx > 50)
                    {
                        await LikeMovie(Movies[currentIndex].Id, parentWidth);
                    }
                    else if (finalDx < -50)
                    {
                        await DislikeMovie(Movies[currentIndex].Id, parentWidth);
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
            WishlistedMovieIds.Add(movieId);
            await CardFrame.TranslateTo(parentWidth, 0, 250, Easing.CubicOut);
            currentIndex++;
            await CarregarFilme();
        }

        private async Task DislikeMovie(int movieId, double parentWidth)
        {
            RefusedMovieIds.Add(movieId);
            await CardFrame.TranslateTo(-parentWidth, 0, 250, Easing.CubicOut);
            currentIndex++;
            await CarregarFilme();
        }
        private async Task WachedMovie(int movieId, double parentHeight)
        {
            WatchedMovieIds.Add(movieId);
            await CardFrame.TranslateTo(0, -parentHeight, 250, Easing.CubicOut);
            currentIndex++;
            await CarregarFilme();
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
            await DislikeMovie(Movies[currentIndex].Id, parentWidth);
            ResetCardPosition();
        }
        private async void OnWatchedClicked(object sender, EventArgs e)
        {
            double parentHeight = ((VisualElement)CardFrame.Parent).Height;
            await WachedMovie(Movies[currentIndex].Id, parentHeight);
            ResetCardPosition();
        }

        private void ResetCardPosition()
        {
            CardFrame.TranslationX = 0;
            CardFrame.TranslationY = 0;
            CardFrame.Rotation = 0;
            CardFrame.Opacity = 1;
        }

        private async Task<string> FindMyId(string jwt)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);

                var resp = await _httpClient.GetAsync(APIConfig.MyIdEndpoint);
                if (!resp.IsSuccessStatusCode)
                {
                    var msg = await resp.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha ao buscar login: {resp.StatusCode}\n{msg}", "OK");
                    return null;
                }
                var respJson = await resp.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var idResp = JsonSerializer.Deserialize<FindMyIdDTO>(respJson, options);
                return idResp.Id;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Exceção ao encontrar usuário: {ex.Message}", "OK");
                return null;
            }
            finally
            {
                // Remova o header para não afetar outras requisições
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
}
