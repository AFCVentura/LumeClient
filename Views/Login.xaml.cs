using LumeClient.Config;
using LumeClient.DTOs;
using System.Text;
using System.Text.Json;
using static LumeClient.Views.InicioCadastro;

namespace LumeClient.Views;

public partial class Login : ContentPage
{
    public Login()
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

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            string emailDigitado = txt_email.Text.Trim();
            string senhaDigitada = txt_senha.Text;

            var loginData = new
            {
                email = emailDigitado,
                password = senhaDigitada
            };

            var httpClient = new HttpClient();
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = APIConfig.LoginEndpoint;
            var response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(responseContent, new JsonSerializerOptions
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
        await Navigation.PushAsync(new InicioCadastro(EtapasCadastroEnum.PrePerguntas));
    }
}