namespace LumeClient.Views;

public partial class Loading : ContentPage
{
    public string Message { get; set; }

    private static readonly Random _random = new Random();

    private static readonly string[] LoadingMessages = new[]
    {
        "Preparando a pipoca e ajustando o projetor 🍿🎬",
        "Verificando se a poltrona está macia o suficiente",
        "Aquecendo a câmera imaginária",
        "Enrolando a película no carretel",
        "Ajustando o foco da lente da imaginação",
        "Dando claquete interna… ação em breve!",
        "Passando trailer mental do que vem a seguir",
        "Testando som estéreo nos alto-falantes cerebrais",
        "Esticando o tapete vermelho virtual",
        "Recortando cenas desnecessárias antes de mostrar",
        "Afinando o roteirista interno",
        "Empurrando o camarim dos bastidores",
        "Ligando os holofotes do cinema interno",
        "Revisando takes antes da estreia",
        "Pulando os créditos iniciais… quase lá!",
        "Desembalando o balde extra de pipoca",
        "Procurando sinal de Wi-Fi no fundo do palco",
        "Voltando dos bastidores com novidades",
        "Mixando trilha sonora invisível",
        "Colocando legendas na colherada de pipoca",
        "Revisando roteiro secreto",
        "Ajustando a cena final… aguarde",
        "Organizando as cadeiras do auditório imaginário",
        "Afinando o microfone do narrador interno",
        "Iluminando o letreiro do cinema"
    };

    public Loading()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var message = SortearMensagem();
        Message = message;
        lblMessage.Text = Message;
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    public string SortearMensagem()
    {
        return LoadingMessages[_random.Next(LoadingMessages.Length)];
    }
}