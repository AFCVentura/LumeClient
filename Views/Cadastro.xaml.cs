using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace LumeClient.Views
{
    public partial class Cadastro : ContentPage
    {

        public Cadastro(List<int> selectedExtraIds, List<int> SelectedThemeIds)
        {
            InitializeComponent();

        }

        private async void OnCadastroClicked(object sender, EventArgs e)
        {
            string email = emailEntry?.Text?.Trim();
            string senha = senhaEntry?.Text;
            string confirmarSenha = confirmarSenhaEntry?.Text;

            if (!chkMaiorIdade.IsChecked)
            {
                await DisplayAlert("Aten��o", "� necess�rio confirmar que voc� � maior de 18 anos.", "OK");
                return;
            }

            if (!chkAceitoTermos.IsChecked)
            {
                await DisplayAlert("Aten��o", "Voc� precisa aceitar os termos de uso para continuar.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(confirmarSenha))
            {
                await DisplayAlert("Erro", "Preencha todos os campos.", "OK");
                return;
            }

            if (senha != confirmarSenha)
            {
                await DisplayAlert("Erro", "As senhas n�o coincidem.", "OK");
                return;
            }

            try
            {
                var httpClient = new HttpClient();
                var cadastroRequest = new
                {
                    Email = email,
                    Password = senha,
                    ConfirmPassword = confirmarSenha
                };

                // Substitua pela URL da sua API que faz o cadastro no Identity
                string url = "https://192.168.0.105:5249/register";

                var response = await httpClient.PostAsJsonAsync(url, cadastroRequest);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sucesso", "Cadastro realizado com sucesso!", "Fazer login");
                    await Shell.Current.GoToAsync("//Login");
                }
                else
                {
                    var erro = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha no cadastro: {erro}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro inesperado: {ex.Message}", "OK");
            }
        }

        private async void Voltar_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Perguntas");
        }

        private async void AbrirTermos(object sender, EventArgs e)
        {
            bool aceito = await DisplayAlert(
                "Termos de Uso",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Termos gen�ricos aqui para simular o pop-up dos termos de uso. Role at� o final para aceitar.",
                "Aceito",
                "N�o aceito"
            );

            chkAceitoTermos.IsChecked = aceito;
        }
    }
}
