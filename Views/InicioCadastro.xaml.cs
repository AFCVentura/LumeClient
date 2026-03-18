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
        PreCadastro,
        PrePerguntasDia
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
        else if (etapa == EtapasCadastroEnum.PrePerguntasDia)
        {
            Instrucoes = new List<string>()
            {
                "Agora sim você vai poder começar a jogar, faremos algumas perguntas rápidas e iremos recomendar alguns filmes com base nelas!",
            };
        }

            InitializeComponent();
        txt_instrucoes.Text = Instrucoes[0];
    }

    private bool _isBackConfirmationOpen = false;

    protected override bool OnBackButtonPressed()
    {
        if (_isBackConfirmationOpen)
            return true;

        _isBackConfirmationOpen = true;
        if (InstrucoesIndex == 0 && Etapa != EtapasCadastroEnum.PrePerguntasDia)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool confirmar = await DisplayAlert("Atenção",
                    "Você irá perder todo o progresso. Deseja realmente voltar ao login?",
                    "Sim", "Cancelar");

                if (confirmar)
                    await Navigation.PushAsync(new Login());

                _isBackConfirmationOpen = false;
            });
        }
        else if (InstrucoesIndex == 0 && Etapa == EtapasCadastroEnum.PrePerguntasDia)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool confirmar = await DisplayAlert("Atenção",
                    "Você irá perder todo o progresso. Deseja realmente voltar à tela inicial?",
                    "Sim", "Cancelar");

                if (confirmar)
                    await Navigation.PushAsync(new MainPage());

                _isBackConfirmationOpen = false;
            });
        }
        else
        {
            _isBackConfirmationOpen = false;

            InstrucoesIndex -= 1;
            txt_instrucoes.Text = Instrucoes[InstrucoesIndex];
        }

        return true;
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
                await Navigation.PushAsync(new PerguntasGerais());
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
            else if (Etapa == EtapasCadastroEnum.PrePerguntasDia)
            {
                // navega para as perguntas do dia
                await Navigation.PushAsync(new PerguntasDia());
            }
            
        }
    }
}