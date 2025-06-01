using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LumeClient.Views
{
    public partial class EsqueciSenha : ContentPage
    {
        public EsqueciSenha()
        {
            InitializeComponent();
        }

        private async void OnContinuarClicked(object sender, EventArgs e)
        {
            try
            {
                string email = emailEntry.Text;

                if (string.IsNullOrEmpty(email))
                {
                    await DisplayAlert("Erro", "Por favor, insira seu e-mail.", "OK");
                    return;
                }

                var httpClient = new HttpClient();
                var json = JsonSerializer.Serialize(email);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://localhost:7141/api/v1/users/forgot-password", content);

                if (response.IsSuccessStatusCode)
                {
                    bool confirm = await DisplayAlert(
                        "Sucesso",
                        "E-mail de redefinição enviado com sucesso. Clique em OK para redefinir sua senha.",
                        "OK",
                        "Cancelar");

                    if (confirm)
                    {
                        await Navigation.PushAsync(new RedefinirSenha());
                    }
                }
                else
                {
                    var erro = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha ao enviar e-mail: {erro}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
            }
        }

        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Login");
        }
    }
}
