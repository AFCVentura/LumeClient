namespace LumeClient.Views;

public partial class InicioCadastro : ContentPage
{
    public List<string> Instrucoes { get; set; }
    public int InstrucoesIndex { get; set; }

    public InicioCadastro()
    {
        InstrucoesIndex = 0;
        Instrucoes = new List<string>
        {
            "Bem vindo ao Lume!",
            "Somos uma plataforma de recomendação de filmes focada em mostrar o que há de melhor no cinema levando em conta seus gostos pessoais",
            "Antes de continuar, por favor, responda algumas perguntas, é rapidinho!"
        };
        InitializeComponent();
        txt_instrucoes.Text = Instrucoes[0];
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        InstrucoesIndex += 1;

        if (InstrucoesIndex < Instrucoes.Count)
        {
            txt_instrucoes.Text = Instrucoes[InstrucoesIndex];
        }
        else
        {
            // Navega para a próxima página de cadastro
            await Navigation.PushAsync(new PerguntasPage());
        }
    }
}