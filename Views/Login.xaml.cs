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
        // Aqui voc� pode colocar valida��o de email/senha depois
        await Navigation.PushAsync(new Perguntas());
    }
}