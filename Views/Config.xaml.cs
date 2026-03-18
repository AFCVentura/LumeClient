    using LumeClient.Config;
    using System.Net.Http;
    using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

    namespace LumeClient.Views
    {
        public partial class Config : ContentPage
        {
            public Config()
            {
                InitializeComponent();
            }
            protected override bool OnBackButtonPressed()
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(new MainPage());
                    // Impede o comportamento padrão do botão Voltar
                });
                return true;
            }

        private async void OnAlterarNomeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AlterarNome");
        }

        private async void OnAlterarSenhaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AlterarSenha");
        }

            private async void OnExcluirContaClicked(object sender, EventArgs e)
            {
                var httpClient = new HttpClient();
                var token = await SecureStorage.Default.GetAsync("access_token");
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    bool confirmar = await DisplayAlert("Excluir Conta", "Tem certeza que deseja excluir sua conta? \nEsta ação não poderá ser desfeita", "Excluir", "Cancelar");
                    if (confirmar)
                    {
                        var url = APIConfig.DeleteAccountEndpoint;
                        var response = await httpClient.DeleteAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            await DisplayAlert("Conta Excluída", "Sua conta foi excluída com sucesso.", "OK");
                            await Navigation.PushAsync(new Login());
                        }
                        else
                        {
                            var erro = await response.Content.ReadAsStringAsync();
                            await DisplayAlert("Erro", $"Falha ao excluir conta: {erro}", "OK");
                        }
                    }
                }
                catch
                {
                    await DisplayAlert("Erro", $"Falha ao excluir conta", "OK");
                }
                finally
                {
                    httpClient.DefaultRequestHeaders.Authorization = null;
                }

            }

            private async void OnSairClicked(object sender, EventArgs e)
            {
                var httpClient = new HttpClient();
                var token = await SecureStorage.Default.GetAsync("access_token");
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    bool confirmar = await DisplayAlert("Sair", "Deseja sair da conta?", "Sim", "Cancelar");
                    if (confirmar)
                    {
                        var url = APIConfig.LogoutEndpoint;
                        var content = new StringContent("{}", Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync(url, content);

                        if (response.IsSuccessStatusCode)
                        {
                            await Navigation.PushAsync(new Login());
                        }
                        else
                        {
                            var erro = await response.Content.ReadAsStringAsync();
                            await DisplayAlert("Erro", $"Falha ao fazer logout: {erro}", "OK");
                        }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exceção: {ex.Message}");
                await DisplayAlert("Erro", $"Falha ao fazer logout", "OK");
            } 
            finally
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }

            }

            private async void OnVoltarClicked(object sender, EventArgs e)
            {
                await Navigation.PushAsync(new MainPage());
            }
        }
    }
