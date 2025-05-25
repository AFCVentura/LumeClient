namespace LumeClient.Views
{
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
            await Navigation.PushAsync(new PerguntasPage());
        }

        private async void OnEsqueciSenhaTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TelaEsqueciSenha");
        }

        private async void OnCriarContaTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Perguntas");
        }
    }
}
