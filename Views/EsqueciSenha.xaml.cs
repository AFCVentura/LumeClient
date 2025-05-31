using System;
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
            string email = emailEntry.Text;

            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Erro", "Por favor, insira seu e-mail.", "OK");
                return;
            }

            // Mock: código enviado (normalmente seria via backend)
            string resultado = await DisplayPromptAsync(
                "Código de verificação",
                "Enviamos um código para seu e-mail. Digite-o abaixo:",
                "Confirmar", "Cancelar", "Digite o código");

            if (resultado == "0000")
            {
                await Navigation.PushAsync(new RedefinirSenha());
            }
            else if (resultado != null)
            {
                await DisplayAlert("Erro", "Código incorreto.", "OK");
            }
        }

        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
