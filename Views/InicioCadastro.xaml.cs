namespace LumeClient.Views;

public partial class InicioCadastro : ContentPage
{
    public List<string> Instrucoes { get; set; }
    public int InstrucoesIndex { get; set; }
    public EtapasCadastroEnum Etapa { get; set; }
    public enum EtapasCadastroEnum
    {
        PrePerguntas,
        PreFilmes,
        PreCadastro
    }

    List<int>? ExtraAnswerIds = null;
    List<int>? ThemeAnswerIds = null;
    List<int>? ChosenMovieIds = null;

    public InicioCadastro(EtapasCadastroEnum etapa, List<int>? extraAnswerIds = null, List<int>? themeAnswerIds = null, List<int>? chosenMovieIds = null)
    {
        ExtraAnswerIds = extraAnswerIds ?? new List<int>();
        ThemeAnswerIds = themeAnswerIds ?? new List<int>();
        ChosenMovieIds = chosenMovieIds ?? new List<int>();

        InstrucoesIndex = 0;
        Etapa = etapa;
        if (etapa == EtapasCadastroEnum.PrePerguntas)
        {
            Instrucoes = new List<string>
            {
                "Bem vindo ao Lume!",
                "Somos uma plataforma de recomendação de filmes focada em mostrar o que há de melhor no cinema levando em conta seus gostos pessoais",
                "Antes de continuar, por favor, responda algumas perguntas, é rapidinho!"
            };
        }
        else if (etapa == EtapasCadastroEnum.PreFilmes)
        {
            Instrucoes = new List<string>
            {
                "Perfeito! Nosso cadastro está quase lá, que tal jogarmos um joguinho agora?",
            };
        }
        else if (etapa == EtapasCadastroEnum.PreCadastro)
        {
            Instrucoes = new List<string>
            {
                "Você foi muito bem! Agora sim, pra finalizar, precisamos apenas que você preencha seu email e defina uma senha.",
            };
        }
        
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
            if (Etapa == EtapasCadastroEnum.PrePerguntas)
            {
                // Navega para a próxima página de perguntas
                await Navigation.PushAsync(new Perguntas());
            }
            else if (Etapa == EtapasCadastroEnum.PreFilmes)
            {
                // Navega para a página de filmes
                await Navigation.PushAsync(new SwipeTutorial(ExtraAnswerIds, ThemeAnswerIds));
            }
            else if (Etapa == EtapasCadastroEnum.PreCadastro)
            {
                // Navega para a página de cadastro
                await Navigation.PushAsync(new Cadastro(ExtraAnswerIds, ThemeAnswerIds, ChosenMovieIds));
            }
            
        }
    }
}