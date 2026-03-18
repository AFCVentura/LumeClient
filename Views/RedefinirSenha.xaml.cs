using LumeClient.Config;
using Microsoft.Maui.Controls;
using System;
using System.Net.Http.Json;

namespace LumeClient.Views
{
    public partial class RedefinirSenha : ContentPage
    {

        private readonly string _email;

        public RedefinirSenha(string email)
        {
            InitializeComponent();
            btnContinuar.IsEnabled = true;
            btnVoltar.IsEnabled = true;
            _email = email;
        }

        private async void OnContinuarClicked(object sender, EventArgs e)
        {
            btnContinuar.IsEnabled = false; 
            btnVoltar.IsEnabled = false;

            string codigo = codigoEntry.Text;
            string novaSenha = novaSenhaEntry.Text;
            string confirmarSenha = confirmarSenhaEntry.Text;

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(confirmarSenha))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                btnContinuar.IsEnabled = true;
                btnVoltar.IsEnabled = true;
                return;
            }

            if (novaSenha != confirmarSenha)
            {
                await DisplayAlert("Erro", "As senhas n�o coincidem.", "OK");
                btnContinuar.IsEnabled = true;
                btnVoltar.IsEnabled = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(_email))
            {
                await DisplayAlert("Erro", "E-mail do usuário não encontrado. Tente novamente.", "OK");
                btnContinuar.IsEnabled = true;
                btnVoltar.IsEnabled = true;
                return;
            }

            var resetModel = new
            {
                Email = _email,
                Token = codigo,
                NewPassword = novaSenha
            };

            var url = APIConfig.ResetPasswordEndpoint;

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(url, resetModel);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sucesso", "Senha redefinida com sucesso!", "OK");
                await Navigation.PushAsync(new Login());
            }
            else
            {
                var erro = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Erro", "Não foi possível redefinir a senha.\n" + erro, "OK");
                btnContinuar.IsEnabled = true;
                btnVoltar.IsEnabled = true;
            }
        }
        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            btnContinuar.IsEnabled = false;
            btnVoltar.IsEnabled = false;
            await Navigation.PopAsync();
        }

    }
}
