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

            // Mock: c�digo enviado (normalmente seria via backend)
            string resultado = await DisplayPromptAsync(
                "C�digo de verifica��o",
                "Enviamos um c�digo para seu e-mail. Digite-o abaixo:",
                "Confirmar", "Cancelar", "Digite o c�digo");

            if (resultado == "0000")
            {
                await Navigation.PushAsync(new RedefinirSenha());
            }
            else if (resultado != null)
            {
                await DisplayAlert("Erro", "C�digo incorreto.", "OK");
            }
        }

        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
