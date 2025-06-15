using LumeClient.Config;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using LumeClient.DTOs;
using LumeClient.DTOs.Users; // SecureStorage

namespace LumeClient.Views
{
    public partial class Cadastro : ContentPage
    {
        private readonly List<int> selectedExtraIds;
        private readonly List<int> selectedThemeIds;
        private readonly List<int> chosenMovieIds;

        private readonly HttpClient _httpClient = new HttpClient();
        public Cadastro(List<int> selectedExtraIds, List<int> selectedThemeIds, List<int> chosenMovieIds)
        {
            InitializeComponent();
            this.selectedExtraIds = selectedExtraIds;
            this.selectedThemeIds = selectedThemeIds;
            this.chosenMovieIds = chosenMovieIds;
        }

        // PARA DEBUG
        public Cadastro()
        {
            InitializeComponent();
        }

        private async void OnCadastroTap(object sender, EventArgs e)
        {
            btn_registro.IsEnabled = false;
            // Validação básica
            var email = txt_email.Text?.Trim();
            var senha = txt_senha.Text;
            var confirmar = txt_confirmar_senha.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(confirmar))
            {
                await DisplayAlert("Atenção", "Preencha todos os campos.", "OK");
                return;
            }
            if (!IsValidEmail(email))
            {
                await DisplayAlert("Atenção", "E-mail inválido.", "OK");
                return;
            }
            if (senha != confirmar)
            {
                await DisplayAlert("Atenção", "Senha e confirmação não coincidem.", "OK");
                return;
            }
            if (!chkAceitoTermos.IsChecked)
            {
                await DisplayAlert("Atenção", "Você deve aceitar os Termos de Uso.", "OK");
                return;
            }
            if (!chkMaiorIdade.IsChecked)
            {
                await DisplayAlert("Atenção", "Você deve confirmar que tem mais de 18 anos.", "OK");
                return;
            }

            try
            {
                // 1) Registro
                var registerSuccess = await RegisterAsync(email, senha);
                if (!registerSuccess)
                {
                    // RegisterAsync já exibe alerta
                    return;
                }

                // 2) Login
                var token = await LoginAsync(email, senha);
                if (string.IsNullOrEmpty(token))
                {
                    // LoginAsync já exibe alerta
                    return;
                }

                // 3) Extrai userId do token
                var userId = await FindMyId(token);
                if (string.IsNullOrEmpty(userId))
                {
                    await DisplayAlert("Erro", "Não foi possível extrair ID do usuário do token.", "OK");
                    return;
                }

                // Armazena token
                await SecureStorage.Default.SetAsync("access_token", token);

                // 4) Enviar perfil (terceira requisição)
                await SendUserProfileAsync(userId, token);

                // 5) Ir para HomePage
                // Aqui você pode limpar navegação ou ajustar conforme sua estrutura
                // Exemplo: substitui a página atual pela HomePage
                Application.Current.Dispatcher.Dispatch(async () =>
                {
                    // Ajuste: se quiser remover esta página da pilha:
                    // await Navigation.PushAsync(new HomePage());
                    // Navigation.RemovePage(this); // ou outra lógica
                    await Navigation.PushAsync(new MainPage());
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
            }
            finally
            {
                // Reabilita UI e esconde loading
                btn_registro.IsEnabled = true;
                activityIndicator.IsRunning = false;
                activityIndicator.IsVisible = false;
                lblLoading.IsVisible = false;
            }
        }

        private async Task<bool> RegisterAsync(string email, string password)
        {
            try
            {
                var payload = new { email = email, password = password };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = await _httpClient.PostAsync(APIConfig.RegisterEndpoint, content);
                if (!resp.IsSuccessStatusCode)
                {
                    var msg = await resp.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha no registro: {resp.StatusCode}\n{msg}", "OK");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Exceção no registro: {ex.Message}", "OK");
                return false;
            }
        }


        private bool IsValidEmail(string email)
        {
            // Validação simples; ajuste regex conforme necessidade
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                var payload = new { email = email, password = password };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = await _httpClient.PostAsync(APIConfig.LoginEndpoint, content);
                if (!resp.IsSuccessStatusCode)
                {
                    var msg = await resp.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha no login: {resp.StatusCode}\n{msg}", "OK");
                    return null;
                }

                var respJson = await resp.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var loginResp = JsonSerializer.Deserialize<LoginResponseDTO>(respJson, options);
                return loginResp?.AccessToken;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Exceção no login: {ex.Message}", "OK");
                return null;
            }
        }

        private async Task<string> FindMyId(string jwt)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jwt);

                var resp = await _httpClient.GetAsync(APIConfig.MyIdEndpoint);
                if (!resp.IsSuccessStatusCode)
                {
                    var msg = await resp.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha ao buscar login: {resp.StatusCode}\n{msg}", "OK");
                    return null;
                }
                var respJson = await resp.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var idResp = JsonSerializer.Deserialize<FindMyIdDTO>(respJson, options);
                return idResp.Id;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Exceção ao encontrar usuário: {ex.Message}", "OK");
                return null;
            }
            finally
            {
                // Remova o header para não afetar outras requisições
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private async Task SendUserProfileAsync(string userId, string token)
        {
            try
            {
                // Monta URL com userId
                var url = $"{APIConfig.GeneralAnswersEndpoint}{userId}";

                // Prepara payload
                var payload = new
                {
                    themeAnswerIds = selectedThemeIds,
                    chosenMovieIds = chosenMovieIds,
                    extraAnswerIds = selectedExtraIds
                };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Adiciona header Authorization
                _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

                var resp = await _httpClient.PostAsync(url, content);
                if (!resp.IsSuccessStatusCode)
                {
                    var msg = await resp.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha ao enviar perfil: {resp.StatusCode}\n{msg}", "OK");
                    return;
                }
                // opcional: processe resposta se houver
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Exceção ao enviar perfil: {ex.Message}", "OK");
            }
            finally
            {
                // Remova o header para não afetar outras requisições
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private async void AbrirTermos(object sender, EventArgs e)
        {
            bool aceito = await DisplayAlert(
                "Termos de Uso",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Termos gen�ricos aqui para simular o pop-up dos termos de uso. Role at� o final para aceitar.",
                "Aceito",
                "Não aceito"
            );

            chkAceitoTermos.IsChecked = aceito;
        }
    }
}
