using System;
using Microsoft.Maui.Controls;

namespace LumeClient.Views
{
    public partial class RedefinirSenha : ContentPage
    {
        public RedefinirSenha()
        {
            InitializeComponent();
        }

        private async void OnContinuarClicked(object sender, EventArgs e)
        {
            string novaSenha = novaSenhaEntry.Text;
            string confirmarSenha = confirmarSenhaEntry.Text;

            if (string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(confirmarSenha))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            if (novaSenha != confirmarSenha)
            {
                await DisplayAlert("Erro", "As senhas n�o coincidem.", "OK");
                return;
            }

            await DisplayAlert("Sucesso", "Senha redefinida com sucesso!", "OK");
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
