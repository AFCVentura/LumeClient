using LumeClient.Extensions;
using System.Threading.Tasks;

namespace LumeClient.Views;

public partial class Header : ContentView
{
    private bool _isBackConfirmationOpen = false;


    public Header()
	{
		InitializeComponent();
	}

    private async void GoToMainPage(object sender, EventArgs e)
    {
        if (Shell.Current.CurrentPage is MainPage)
            return;

        bool answer = await OnGameScreen();

        if (!answer)
            return;

        await Navigation.PushAsync(new MainPage());
    }

    private async Task<bool> OnGameScreen()
    {
        bool isSwipe = Shell.Current.CurrentPage is Swipe;
        bool isPerguntasDia = Shell.Current.CurrentPage is PerguntasDia;
        bool isLoading = Shell.Current.CurrentPage is Loading;

        // Se não tá numa das telas de jogo, não precisa confirmar, pode sair
        if (!isSwipe && !isPerguntasDia && !isLoading)
            return true;

        // Se tá com a confirmação aberta, não faz nada
        if (_isBackConfirmationOpen)
            return false;

        _isBackConfirmationOpen = true;

        // Se tá numa das telas de jogo, pergunta se quer sair
        var parentPage = this.GetParentPage();
        if (parentPage != null)
        {
            bool confirmar = await parentPage.DisplayAlert("Atenção",
                "Você irá perder todo o progresso. Deseja realmente voltar para a tela inicial?",
                "Sim", "Cancelar");

            _isBackConfirmationOpen = false;
            return confirmar;
        }

        _isBackConfirmationOpen = false;
        return false;
    }
}