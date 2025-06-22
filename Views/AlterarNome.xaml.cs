using LumeClient.Config;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LumeClient.Views
{
    public partial class AlterarNome : ContentPage
    {
        public AlterarNome()
        {
            InitializeComponent();
        }
        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Config");
        }

        private async void OnAlterarClicked(object sender, EventArgs e)
        {
            var novoNome = txt_nome.Text?.Trim();

            if (string.IsNullOrWhiteSpace(novoNome))
            {
                await DisplayAlert("Aviso", "Nome de usuário inválido.", "OK");
                return;
            }

            var httpClient = new HttpClient();
            var token = await SecureStorage.Default.GetAsync("access_token");

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var url = APIConfig.ChangeUsernameEndpoint;

            var json = JsonSerializer.Serialize(novoNome);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PatchAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sucesso", "Nome de usuário alterado com sucesso.", "OK");
                    await Shell.Current.GoToAsync("//Config");
                }
                else
                {
                    var erro = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erro ao alterar nome: {erro}");
                    await DisplayAlert("Erro", "Falha ao alterar o nome de usuário.", "OK");
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
}
