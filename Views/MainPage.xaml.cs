namespace LumeClient.Views
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
        }

        private void OnGoogleLoginTapped(object sender, EventArgs e)
        {
            DisplayAlert("Login com Google", "Você clicou em 'Continue with Google'", "OK");
        }

    }

}
