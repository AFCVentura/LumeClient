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
            _email = email;
        }

        private async void OnContinuarClicked(object sender, EventArgs e)
        {
            string codigo = codigoEntry.Text;
            string novaSenha = novaSenhaEntry.Text;
            string confirmarSenha = confirmarSenhaEntry.Text;

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(confirmarSenha))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            if (novaSenha != confirmarSenha)
            {
                await DisplayAlert("Erro", "As senhas nï¿½o coincidem.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(_email))
            {
                await DisplayAlert("Erro", "E-mail do usuário não encontrado. Tente novamente.", "OK");
                return;
            }

            var resetModel = new
            {
                Email = _email,
                Token = codigo,
                NewPassword = novaSenha
            };

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("https://localhost:7141/api/v1/users/reset-password", resetModel);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Sucesso", "Senha redefinida com sucesso!", "OK");
                await Shell.Current.GoToAsync("//Login");
            }
            else
            {
                var erro = await response.Content.ReadAsStringAsync();
                await DisplayAlert("Erro", "Não foi possível redefinir a senha.\n" + erro, "OK");
            }
        }
        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}
