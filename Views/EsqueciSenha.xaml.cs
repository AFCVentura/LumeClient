using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LumeClient.Config;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;

namespace LumeClient.Views
{
    public partial class EsqueciSenha : ContentPage
    {
        public EsqueciSenha()
        {
            InitializeComponent();
            btnContinuar.IsEnabled = true;
            btnVoltar.IsEnabled = true;
        }

        protected override bool OnBackButtonPressed()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await VoltarAsync();
            });

            return true; // cancela o comportamento padrão
        }

        private async void OnContinuarClicked(object sender, EventArgs e)
        {
            try
            {
                btnContinuar.IsEnabled = false;
                btnVoltar.IsEnabled = false;
                string email = emailEntry.Text;

                if (string.IsNullOrEmpty(email))
                {
                    await DisplayAlert("Erro", "Por favor, insira seu e-mail.", "OK");
                    btnContinuar.IsEnabled = true;
                    btnVoltar.IsEnabled = true;
                    return;
                }

                var httpClient = new HttpClient();
                var json = JsonSerializer.Serialize(email);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = APIConfig.ForgotPasswordUserEndpoint;
                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    bool confirm = await DisplayAlert(
                        "Sucesso",
                        "E-mail de redefinição enviado com sucesso. Clique em OK para redefinir sua senha.",
                        "OK",
                        "Cancelar");

                    if (confirm)
                    {
                        await Navigation.PushAsync(new RedefinirSenha(email));
                    }
                }
                else
                {
                    var erro = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha ao enviar e-mail: {erro}", "OK");
                    btnContinuar.IsEnabled = true;
                    btnVoltar.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
                btnContinuar.IsEnabled = true;
                btnVoltar.IsEnabled = true;
            }
        }

        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await VoltarAsync();   
        }

        private async Task VoltarAsync()
        {
            btnContinuar.IsEnabled = false;
            btnVoltar.IsEnabled = false;
            await Navigation.PushAsync(new Login());
        }
    }
}
