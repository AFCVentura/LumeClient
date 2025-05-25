using System;
using Microsoft.Maui.Controls;

namespace LumeClient.Views
{
    public partial class TelaCadastro : ContentPage
    {
        public TelaCadastro()
        {
            InitializeComponent();
        }

        private async void Cadastrar_Clicked(object sender, EventArgs e)
        {
            if (!chkMaiorIdade.IsChecked)
            {
                await DisplayAlert("Atenção", "É necessário confirmar que você é maior de 18 anos.", "OK");
                return;
            }

            if (!chkAceitoTermos.IsChecked)
            {
                await DisplayAlert("Atenção", "Você precisa aceitar os termos de uso para continuar.", "OK");
                return;
            }

            // Aqui você pode adicionar validações dos campos, como email, senha, etc.

            await DisplayAlert("Sucesso", "Cadastro realizado com sucesso!", "OK");
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void Voltar_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Perguntas");
        }

        private async void AbrirTermos(object sender, EventArgs e)
        {
            bool aceito = await DisplayAlert(
                "Termos de Uso",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Termos genéricos aqui para simular o pop-up dos termos de uso. Role até o final para aceitar.",
                "Aceito",
                "Não aceito"
            );

            if (aceito)
            {
                chkAceitoTermos.IsChecked = true;
            }
            else
            {
                chkAceitoTermos.IsChecked = false;
            }
        }
    }
}
