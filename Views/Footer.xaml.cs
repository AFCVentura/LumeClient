using LumeClient.Config;
using LumeClient.DTOs.Users;
using LumeClient.Extensions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LumeClient.Views;

public partial class Footer : ContentView
{
    private bool _isBackConfirmationOpen = false;
    private HttpClient _httpClient;
    private string userId;


    public Footer()
	{
		InitializeComponent();
        _httpClient = new HttpClient();
    }

    private async void OnSettingsTapped(object sender, EventArgs e)
    {
        if (Shell.Current.CurrentPage is Config)
            return;

        bool answer = await OnGameScreen();

        if (!answer)
            return;

        await Navigation.PushAsync(new Config());
    }

    private async void OnPlayTapped(object sender, EventArgs e)
    {
        try
        {
            if (Shell.Current.CurrentPage is PerguntasDia || Shell.Current.CurrentPage is Swipe)
                return;

            bool answer = await OnGameScreen();

            if (!answer)
                return;

            if (await DoIHaveRecommendations())
            {
                // Se já tem recomendações, vai direto para a tela de jogo
                await Navigation.PushAsync(new PerguntasDia());
                return;
            }

            await Navigation.PushAsync(new InicioCadastro(InicioCadastro.EtapasCadastroEnum.PrePerguntasDia));
        }
        catch (Exception ex)
        {
            return;
        }
    }
    
    private async Task<bool> DoIHaveRecommendations()
    {
        try
        {
            var parentPage = this.GetParentPage();
            // 1) obter token
            var token = await SecureStorage.Default.GetAsync("access_token");
            if (string.IsNullOrEmpty(token))
            {
                // talvez redirecionar ao login
                if (parentPage != null)
                {
                    await parentPage.DisplayAlert("Atenção",
                        "Usuário não autenticado.",
                        "OK");

                    _isBackConfirmationOpen = false;
                    throw new Exception();
                }
            }
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // 2) /users/me
            var respMe = await _httpClient.GetAsync(APIConfig.MyIdEndpoint);
            if (!respMe.IsSuccessStatusCode)
            {
                // sem permissão ou token inválido
                // Se tá numa das telas de jogo, pergunta se quer sair
                if (parentPage != null)
                {
                    await parentPage.DisplayAlert("Erro",
                        "Não foi possível obter dados do usuário.",
                        "OK");

                    _isBackConfirmationOpen = false;
                    throw new Exception();
                }
            }
            var jsonMe = await respMe.Content.ReadAsStringAsync();
            var meDto = JsonSerializer.Deserialize<FindMyIdDTO>(jsonMe, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            userId = meDto?.Id;
            if (string.IsNullOrEmpty(userId))
            {
                if (parentPage != null)
                {
                    await parentPage.DisplayAlert("Erro",
                        "Resposta inválida ao obter ID do usuário.",
                        "OK");

                    _isBackConfirmationOpen = false;
                    throw new Exception();
                }
            }

            // 3) /users/{id}/do-i-have-recommendations
            var urlDoIHaveRecommendations = $"{APIConfig.DoIHaveRecommendationsFirstPartEndpoint}{userId}{APIConfig.DoIHaveRecommendationsLastPartEndpoint}";
            var respDoIHaveRecommendations = await _httpClient.GetAsync(urlDoIHaveRecommendations);

            if (!respDoIHaveRecommendations.IsSuccessStatusCode)
            {
                // sem permissão ou token inválido
                if (parentPage != null)
                {
                    await parentPage.DisplayAlert("Erro",
                        "Não foi possível verificar recomendações.",
                        "OK");
                    _isBackConfirmationOpen = false;
                    throw new Exception();
                }
            }

            // content: true ou false se tem recomendações
            var content = Boolean.Parse(await respDoIHaveRecommendations.Content.ReadAsStringAsync());

            return content;
        }
        catch (Exception ex)
        {
            return true;
        }
        finally
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    private async void OnWishlistTapped(object sender, EventArgs e)
    {
        if (Shell.Current.CurrentPage is Wishlist)
            return;

        bool answer = await OnGameScreen();

        if (!answer)
            return;

        await Navigation.PushAsync(new Wishlist());
    }

    private async Task<bool> OnGameScreen()
    {
        bool isSwipe = Shell.Current.CurrentPage is Swipe;
        bool isPerguntasDia = Shell.Current.CurrentPage is PerguntasDia;
        bool isLoading = Shell.Current.CurrentPage is Loading;

        // Se não tá numa das telas de jogo, não precisa confirmar, pode sair
        if (!isSwipe && !isPerguntasDia && !isLoading)
            return true;

        // Se tá com a confirmação aberta, não faz nada
        if (_isBackConfirmationOpen)
            return false;

        _isBackConfirmationOpen = true;

        // Se tá numa das telas de jogo, pergunta se quer sair
        var parentPage = this.GetParentPage();
        if (parentPage != null)
        {
            bool confirmar = await parentPage.DisplayAlert("Atenção",
                "Você irá perder todo o progresso. Deseja realmente sair?",
                "Sim", "Cancelar");

            _isBackConfirmationOpen = false;
            return confirmar;
        }

        _isBackConfirmationOpen = false;
        return false;
    }   
}