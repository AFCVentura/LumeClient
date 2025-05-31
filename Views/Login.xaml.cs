using LumeClient.Models;
using System.Text;
using System.Text.Json;

namespace LumeClient.Views;

public partial class Login : ContentPage
{
    public Login()
    {
        InitializeComponent();
    }

    private void OnGoogleLoginTapped(object sender, EventArgs e)
    {
        DisplayAlert("Login com Google", "Você clicou em 'Continue with Google'", "OK");
    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            string emailDigitado = txt_email.Text;
            string senhaDigitada = txt_senha.Text;

            var loginData = new
            {
                email = emailDigitado,
                password = senhaDigitada
            };

            var httpClient = new HttpClient();
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Troque para o endpoint correto se estiver diferente
            var response = await httpClient.PostAsync("https://localhost:7141/api/v1/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null)
                {
                    await SecureStorage.Default.SetAsync("access_token", loginResponse.AccessToken);
                    await Navigation.PushAsync(new MainPage());
                }
            }
            else
            {
                await DisplayAlert("Erro", "Usuário ou senha inválidos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }

    private async void OnEsqueceuSenhaTap(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new EsqueceuSenha());
    }

    private async void OnCriarContaTap(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new Registro());
    }
}