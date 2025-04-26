namespace LumeClient.Views;

public partial class Login : ContentPage
{
    public Login()
    {
        InitializeComponent();
    }

    private void OnGoogleLoginTapped(object sender, EventArgs e)
    {
        DisplayAlert("Login com Google", "Você clicou em 'Continue with Google'", "OK");
    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Futuramente precisamos trocar isso para enviar pro backend
        try
        {
            // Lista de usuários mockados de teste
            Dictionary<string, string> usuarios = new Dictionary<string, string>
            {
                { "email1", "senha1" },
                { "email2", "senha2" },
                { "email3", "senha3" }
            };

            // Pegando email e senha digitados na tela
            string emailDigitado = txt_email.Text;
            string senhaDigitada = txt_senha.Text;

            if (usuarios.Any(u => u.Key == emailDigitado && u.Value == senhaDigitada))
            {
                await SecureStorage.Default.SetAsync("email_logado", emailDigitado);
                await SecureStorage.Default.SetAsync("senha_logada", senhaDigitada);

                // Se o usuário existe, navega para a página inicial
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                // Se o usuário não existe, lança uma exceção que vai pro catch
                throw new Exception("Usuário ou senha inválidos.");
            }


        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }

    private async void OnEsqueceuSenhaTap(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new EsqueceuSenha());
    }

    private async void OnCriarContaTap(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new Registro());
    }
}