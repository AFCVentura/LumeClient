using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
using LumeClient.Config;
using LumeClient.DTOs.Movies;
using LumeClient.DTOs.Users;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LumeClient.Views;

public partial class Wishlist : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private string _userId;
    private bool _isLoading;
    // ObservableCollection para notificar UI ao remover itens
    private ObservableCollection<WishlistItemViewModel> _items = new();

    // Cache de detalhes
    private readonly Dictionary<int, MovieDetailsDTO> _detailsCache = new();

    public Wishlist()
    {
        InitializeComponent();
        WishlistCollection.BindingContext = this;

        // Atribui ItemsSource
        WishlistCollection.ItemsSource = _items;

        // Cria o comando de remoção
        RemoveCommand = new Command<int>(async movieId => await RemoveFromWishlistAsync(movieId));
    }

    // Comando usado nos ImageButtons
    public Command<int> RemoveCommand { get; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_isLoading)
        {
            await LoadWishlistAsync();
        }
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

    // Carrega lista de filmes na wishlist
    private async Task LoadWishlistAsync()
    {
        _isLoading = true;
        _items.Clear();
        _detailsCache.Clear();

        try
        {
            // 1) obter token
            var token = await SecureStorage.Default.GetAsync("access_token");
            if (string.IsNullOrEmpty(token))
            {
                await DisplayAlert("Atenção", "Usuário não autenticado.", "OK");
                return;
            }
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // 2) obter userId
            var respMe = await _httpClient.GetAsync(APIConfig.MyIdEndpoint);
            if (!respMe.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", "Falha ao obter dados do usuário.", "OK");
                return;
            }
            var jsonMe = await respMe.Content.ReadAsStringAsync();
            var meDto = JsonSerializer.Deserialize<FindMyIdDTO>(jsonMe, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _userId = meDto?.Id;
            if (string.IsNullOrEmpty(_userId))
            {
                await DisplayAlert("Erro", "Resposta inválida ao obter ID do usuário.", "OK");
                return;
            }

            // 3) GET wishlist
            // Ex.: "/api/v1/movies/wishlist-movies/{userId}"
            var wishlistUrl = $"{APIConfig.WishlistMoviesAllEndpoint}{_userId}";
            var resp = await _httpClient.GetAsync(wishlistUrl);
            if (!resp.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", "Falha ao obter wishlist.", "OK");
                return;
            }
            var json = await resp.Content.ReadAsStringAsync();
            var listDto = JsonSerializer.Deserialize<List<MovieItemDTO>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (listDto == null || listDto.Count == 0)
            {
                // A CollectionView mostrará o EmptyView automaticamente
                return;
            }

            // 4) Preenche ObservableCollection com modelos iniciais
            const string baseImageUrl = "https://image.tmdb.org/t/p/w500";
            foreach (var m in listDto)
            {
                var vm = new WishlistItemViewModel
                {
                    Id = m.Id,
                    Title = m.Title ?? "(Sem título)",
                    PosterFullUrl = string.IsNullOrEmpty(m.PosterPath)
                        ? "no_poster_card.png"
                        : (m.PosterPath.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                            ? m.PosterPath
                            : $"{baseImageUrl}{m.PosterPath}"),
                    RatingText = "⭐ –", // texto provisório
                };
                _items.Add(vm);
            }

            // 5) Em segundo plano, para cada item, buscar detalhes e atualizar rating
            foreach (var vm in _items.ToList())
            {
                _ = FetchAndUpdateDetailsAsync(vm);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao carregar wishlist: {ex.Message}", "OK");
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _isLoading = false;
        }
    }

    // Busca detalhes para preencher RatingText (e armazenar em cache)
    private async Task FetchAndUpdateDetailsAsync(WishlistItemViewModel vm)
    {
        if (_detailsCache.ContainsKey(vm.Id))
            return;

        try
        {
            var token = await SecureStorage.Default.GetAsync("access_token");
            if (string.IsNullOrEmpty(token)) return;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // GET detalhes: "/api/v1/movies/wishlist-movies/{userId}/wishlisted-movies/{movieId}"
            var detailUrl = $"{APIConfig.WishlistMoviesByIdFirstPartEndpoint}{_userId}{APIConfig.WishlistMoviesByIdLastPartEndpoint}{vm.Id}";
            var resp = await _httpClient.GetAsync(detailUrl);
            if (!resp.IsSuccessStatusCode) return;
            var json = await resp.Content.ReadAsStringAsync();
            var details = JsonSerializer.Deserialize<MovieDetailsDTO>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (details != null)
            {
                _detailsCache[vm.Id] = details;
                // Atualiza RatingText no UI thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    vm.RatingText = $"⭐ {details.VoteAverage:F1} ({details.VoteCount})";
                });
            }
        }
        catch
        {
            // ignore erros individuais de detalhe
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    // Handler de toque em item (abre detalhes via popup)
    private async void OnItemTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is WishlistItemViewModel vm)
        {
            // Se tivermos detalhes em cache, use; senão, busque agora
            MovieDetailsDTO details = null;
            if (_detailsCache.TryGetValue(vm.Id, out var d))
            {
                details = d;
            }
            else
            {
                // opcional: mostrar loading
                try
                {
                    var token = await SecureStorage.Default.GetAsync("access_token");
                    if (!string.IsNullOrEmpty(token))
                    {
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token);
                        var detailUrl = $"{APIConfig.WishlistMoviesByIdFirstPartEndpoint}{_userId}{APIConfig.WishlistMoviesByIdLastPartEndpoint}{vm.Id}";
                        var resp = await _httpClient.GetAsync(detailUrl);
                        if (resp.IsSuccessStatusCode)
                        {
                            var json = await resp.Content.ReadAsStringAsync();
                            details = JsonSerializer.Deserialize<MovieDetailsDTO>(json,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            if (details != null)
                                _detailsCache[vm.Id] = details;
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro", $"Falha ao carregar detalhes: {ex.Message}", "OK");
                    return;
                }
                finally
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
            }

            if (details != null)
            {
                // Exibe popup de detalhes (assegure que MovieInfoPopup está implementado)
                var popup = new MovieInfoPopup(details);
                await this.ShowPopupAsync(popup);
            }
            else
            {
                await DisplayAlert("Atenção", "Não foi possível carregar detalhes.", "OK");
            }
        }
    }

    // Remove item da wishlist: envia DELETE e remove do ObservableCollection
    private async Task RemoveFromWishlistAsync(int movieId)
    {
        try
        {
            // 1) Obter token
            var token = await SecureStorage.Default.GetAsync("access_token");
            if (string.IsNullOrEmpty(token))
            {
                await DisplayAlert("Atenção", "Usuário não autenticado.", "OK");
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // 2) Obter userId
            var respMe = await _httpClient.GetAsync(APIConfig.MyIdEndpoint);
            if (!respMe.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", "Falha ao obter dados do usuário.", "OK");
                return;
            }

            var jsonMe = await respMe.Content.ReadAsStringAsync();
            var meDto = JsonSerializer.Deserialize<FindMyIdDTO>(jsonMe,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var userId = meDto?.Id;
            if (string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Erro", "Resposta inválida ao obter ID do usuário.", "OK");
                return;
            }

            // 3) Enviar requisição DELETE
            var deleteUrl = $"{APIConfig.WishlistMoviesByIdFirstPartEndpoint}{userId}{APIConfig.WishlistMoviesByIdLastPartEndpoint}{movieId}";
            var response = await _httpClient.DeleteAsync(deleteUrl);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Pronto!", "Filme removido da Wishlist com sucesso.", "OK");

            }
            else
            {
                var errorText = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Erro", $"Erro ao mover o filme: {errorText}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Exceção ao remover: {ex.Message}", "OK");
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    private async void OnGoMainPageClicked(object sender, EventArgs e)
    {
        // Navega para a tela de swipe (exemplo)
        await Navigation.PushAsync(new MainPage());
    }
}

// ViewModel do item para binding e notificação
public class WishlistItemViewModel : INotifyPropertyChanged
{
    public int Id { get; set; }

    string _title;
    public string Title
    {
        get => _title;
        set { if (_title != value) { _title = value; OnPropertyChanged(nameof(Title)); } }
    }

    string _posterFullUrl;
    public string PosterFullUrl
    {
        get => _posterFullUrl;
        set { if (_posterFullUrl != value) { _posterFullUrl = value; OnPropertyChanged(nameof(PosterFullUrl)); } }
    }

    string _ratingText;
    public string RatingText
    {
        get => _ratingText;
        set { if (_ratingText != value) { _ratingText = value; OnPropertyChanged(nameof(RatingText)); } }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
