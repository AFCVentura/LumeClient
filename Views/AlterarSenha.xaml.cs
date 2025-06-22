using LumeClient.Config;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LumeClient.Views;

public partial class AlterarSenha : ContentPage
{
    public AlterarSenha()
    {
        InitializeComponent();
    }
    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Config");
    }

    private async void OnAlterarClicked(object sender, EventArgs e)
    {
        var senhaAtual = txt_senha_atual.Text?.Trim();
        var novaSenha = txt_nova_senha.Text?.Trim();
        var confirmarSenha = txt_confirmar_senha.Text?.Trim();

        if (string.IsNullOrWhiteSpace(senhaAtual) || string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(confirmarSenha))
        {
            await DisplayAlert("Aviso", "Preencha todos os campos.", "OK");
            return;
        }

        if (novaSenha != confirmarSenha)
        {
            await DisplayAlert("Aviso", "As novas senhas não coincidem.", "OK");
            return;
        }

        var httpClient = new HttpClient();
        var token = await SecureStorage.Default.GetAsync("access_token");

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var url = APIConfig.ChangePasswordEndpoint;

        var body = new
        {
            CurrentPassword = senhaAtual,
            NewPassword = novaSenha
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PatchAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sucesso", "Senha alterada com sucesso.", "OK");
                await Shell.Current.GoToAsync("//Config");
            }
            else
            {
                var erro = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erro ao alterar senha: {erro}");
                await DisplayAlert("Erro", "Falha ao alterar senha.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exceção: {ex.Message}");
            await DisplayAlert("Erro", "Ocorreu um erro inesperado.", "OK");
        }
        finally
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
