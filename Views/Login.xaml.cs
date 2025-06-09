using LumeClient.DTOs;
using System.Text;
using System.Text.Json;

namespace LumeClient.Views;

public partial class Login : ContentPage
{
    public Login()
    {
        InitializeComponent();
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
            var response = await httpClient.PostAsync("http://192.168.0.105:5249/login", content);

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
                    await Shell.Current.GoToAsync("//MainPage");
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
        await Navigation.PushAsync(new EsqueciSenha());
    }

    private async void OnCriarContaTap(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InicioCadastro());
    }
}