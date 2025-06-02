using System;
using Microsoft.Maui.Controls;

namespace LumeClient.Views
{
    public partial class Config : ContentPage
    {
        public Config()
        {
            InitializeComponent();
        }

        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(".."); // Voltar para a tela anterior
        }

        private async void OnAlterarNomeClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Alterar Nome", "Funcionalidade em desenvolvimento", "OK");
        }

        private async void OnAlterarSenhaClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Alterar Senha", "Funcionalidade em desenvolvimento", "OK");
        }

        private async void OnExcluirContaClicked(object sender, EventArgs e)
        {
            bool confirmar = await DisplayAlert("Excluir Conta", "Tem certeza que deseja excluir sua conta?", "Sim", "Cancelar");
            if (confirmar)
            {
                await DisplayAlert("Conta Exclu�da", "Sua conta foi exclu�da.", "OK");
                await Shell.Current.GoToAsync("//Login");
            }
        }

        private async void OnSairClicked(object sender, EventArgs e)
        {
            bool confirmar = await DisplayAlert("Sair", "Deseja sair da conta?", "Sim", "Cancelar");
            if (confirmar)
            {
                await Shell.Current.GoToAsync("//Login");
            }
        }
    }
}
