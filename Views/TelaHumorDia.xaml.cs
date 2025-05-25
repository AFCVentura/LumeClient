using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace LumeClient.Views;

public partial class TelaHumorDia : ContentPage
{
    private int perguntaIndex = 0;
    private Dictionary<string, string> respostas = new();
    private List<Pergunta> perguntas;

    public TelaHumorDia()
    {
        InitializeComponent();
        perguntas = GerarPerguntas();
        ExibirPerguntaAtual();
    }

    private List<Pergunta> GerarPerguntas()
    {
        return new List<Pergunta>
        {
            new Pergunta
            {
                Texto = "HOJE, QUAL TIPO DE HISTÓRIA TE CHAMA MAIS ATENÇÃO?",
                Opcoes = new List<string>
                {
                    "Algo leve e divertido, me faça rir.",
                    "Algo intenso e profundo, quero refletir hoje.",
                    "Algo que me envolva emocionalmente.",
                    "Algo cheio de ação e aventura.",
                    "Algo que me faça sentir medo ou tensão.",
                    "Algo fora do comum, surreal."
                },
                Chave = "PreferenciaUsuario"
            },
            new Pergunta
            {
                Texto = "COMO ESTÁ SEU NÍVEL DE RESISTÊNCIA A FILMES LONGOS?",
                Opcoes = new List<string>
                {
                    "Quanto maior, melhor! Me dá filme de 3 horas!",
                    "Até 2 horas tá ótimo.",
                    "Prefiro rapidinhos, estilo 1h30 e olhe lá.",
                    "Se for bom, nem percebo o tempo passar.",
                    "Tô pronto pra maratonar uma saga inteira!"
                },
                Chave = "DuracaoFilme"
            },
            new Pergunta
            {
                Texto = "HOJE ESTOU ME SENTINDO UM POUCO MAIS:",
                Opcoes = new List<string>
                {
                    "Alegre e pra cima",
                    "Pensativo",
                    "Meio tristinho.",
                    "Cansado",
                    "Apaixonado",
                    "Ansioso"
                },
                Chave = "Humor"
            },
            new Pergunta
            {
                Texto = "VOCÊ VAI ASSISTIR COM MAIS ALGUÉM?",
                Opcoes = new List<string>
                {
                    "Estou acompanhado!",
                    "Hoje a sessão é solo."
                },
                Chave = "Acompanhado"
            }
        };
    }

    private void ExibirPerguntaAtual()
    {
        if (perguntaIndex >= perguntas.Count)
        {
            FinalizarPerguntas();
            return;
        }

        var perguntaAtual = perguntas[perguntaIndex];

        PerguntaLabel.Text = perguntaAtual.Texto;
        OpcoesLayout.Children.Clear();

        foreach (var opcao in perguntaAtual.Opcoes)
        {
            var botao = new Button
            {
                Text = opcao,
                BackgroundColor = Colors.Yellow,
                TextColor = Colors.Black,
                FontAttributes = FontAttributes.Bold,
                CornerRadius = 12
            };
            botao.Clicked += OnOpcaoSelecionada;
            OpcoesLayout.Children.Add(botao);
        }
    }

    private void OnOpcaoSelecionada(object sender, EventArgs e)
    {
        var botao = sender as Button;
        var perguntaAtual = perguntas[perguntaIndex];

        respostas[perguntaAtual.Chave] = botao.Text;

        // Verificar se deve adicionar a pergunta extra para o parceiro
        if (perguntaAtual.Chave == "Acompanhado" && botao.Text == "Estou acompanhado!")
        {
            perguntas.Insert(perguntaIndex + 1, new Pergunta
            {
                Texto = "QUE TIPO DE HISTÓRIA SEU PARCEIRO(A) DE FILME PREFERE HOJE?",
                Opcoes = new List<string>
                {
                    "Algo leve e divertido, me faça rir.",
                    "Algo intenso e profundo, quero refletir hoje.",
                    "Algo que me envolva emocionalmente.",
                    "Algo cheio de ação e aventura.",
                    "Algo que me faça sentir medo ou tensão.",
                    "Algo fora do comum, surreal."
                },
                Chave = "PreferenciaCompanheiro"
            });
        }

        perguntaIndex++;
        ExibirPerguntaAtual();
    }

    private async void FinalizarPerguntas()
    {
        string resultado = "";
        foreach (var item in respostas)
        {
            resultado += $"{item.Key}: {item.Value}\n";
        }

        await DisplayAlert("Respostas", resultado, "OK");
        await Shell.Current.GoToAsync("//MainPage");
    }
}

public class Pergunta
{
    public string Texto { get; set; }
    public List<string> Opcoes { get; set; }
    public string Chave { get; set; }
}
