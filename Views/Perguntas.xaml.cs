using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace LumeClient.Views
{
    public partial class PerguntasPage : ContentPage
    {
        private int perguntaAtual = 0;
        private List<string> respostas = new();

        private readonly List<string> introducoes = new()
        {
            "Oi, tudo bem? Me conta uma coisa:",
            "Entendi! Agora me diz:",
            "Boa! E me conta rapidinho:",
            "Legal, agora me fala sobre:",
            "Estamos quase lá...",
            "Faltam só mais algumas...",
            "Última perguntinha, juro!"
        };

        private readonly List<(string pergunta, List<string> opcoes)> quiz = new()
        {
            ("Quem é você quando assiste filmes?", new()
            {
                "Aquele que dorme na primeira oportunidade.",
                "Aquele que conhece todos os atores e diretores, super cinéfilo.",
                "Aquele que chora até em comercial de margarina.",
                "O caçador de plot twist: se não tiver reviravolta, nem perco tempo.",
                "Aquele que ama uma fantasia, até por que, a realidade já é difícil.",
                "Aquele que ama um filme bem gráfico (quanto mais sangue melhor).",
                "O fã de explosões, perseguições e muita bagunça na tela."
            }),
            ("Na hora de escolher um filme, você prefere histórias que:", new()
            {
                "Fica mais curioso e quer ver na hora.",
                "Acha interessante, mas prefere decidir sozinho.",
                "Vai pelo seu próprio feeling, nota não é tudo.",
                "Nem repara, só assiste."
            }),
            ("Você diria que os seus filmes preferidos, são:", new()
            {
                "Bem conhecidos e queridinhos do público.",
                "Pequenos tesouros escondidos",
                "Super premiados, cheios de troféu.",
                "Tão esquisitos que só você mesmo entende.",
                "Uma mistura geral de tudo que é bom."
            }),
            ("Pra você, um filme perfeito seria:", new()
            {
                "De preferência em outra realidade, outro planeta, outra dimensão.",
                "Aqueles nos tempos da antiguidade, bem 'Babilônico', 'Faraônico', 'Mesopotâmico' e tudo 'ônico'.",
                "Aqueles que até em Preto e Branco eram filmados.",
                "Bem medieval, com reis, rainhas e até pragas.",
                "Bem Vintage, Retrô, Nostálgico. Aquilo que era época.",
                "O aqui e agora é bem melhor!",
                "Um universo mágico cheio de seres fantásticos.",
                "Ou naquelas cabanas do terror clássico, com assassino maluco na floresta."
            }),
            ("Quando você termina um filme, você geralmente:", new()
            {
                "Cria finais alternativos na cabeça.",
                "Fica viajando na história por horas.",
                "Corre pra contar pra alguém e indicar.",
                "Já emendo outro filme na sequência.",
                "Gosto e tal, mas logo esqueço e vou viver a vida",
                "Vou buscar se tem uma sequência, adoro uma franquia."
            }),
            ("Na sua opinião, um final perfeito é:", new()
            {
                "Aquele que dá um nó na cabeça, me deixa paralisado, sem reação e deixa mil interpretações.",
                "Um filme redondinho, bem fechado, sem pontas soltas.",
                "Super emocionante, mesmo que triste (me deixa chorar em paz).",
                "Alegre e pra cima, porque drama já basta a vida.",
                "O que importa é a experiência inteira, final é detalhe."
            }),
            ("Se tivesse que descrever seu gosto para filmes em uma palavra, seria:", new()
            {
                "Intenso", "Profundo", "Divertido", "Surpreendente", "Sonhador", "Realista",
                "Não tenho, eu só quero me entreter durante umas horinhas."
            }),
            ("Na hora de escolher o que assistir, o que pesa mais?", new()
            {
                "Ou o trailer me conquista, ou me faz desistir.",
                "A vibe do dia, sem muito critério.",
                "O gênero que já amo e conheço bem.",
                "A indicação daquela pessoa que entende meu gosto.",
                "A curiosidade de ver algo novo e diferente.",
                "O ator ou atriz que está no filme."
            }),
            ("Se você fosse um protagonista de um filme, o que você gostaria de ser:", new()
            {
                "Gostaria de ter super poderes ou ser um super-herói.",
                "Viver romances impossíveis e inesquecíveis.",
                "Ser um super vilão.",
                "Ser aquele protagonista bonzinho e que todo mundo adora",
                "Aquele protagonista típico de filme de ação, que quando pisam no calo dele se transforma no CARA."
            })
        };

        public PerguntasPage()
        {
            InitializeComponent();
            MostrarPergunta();
        }

        private void MostrarPergunta()
        {
            if (perguntaAtual >= quiz.Count)
            {
                DisplayAlert("Fim", "Vamos criar sua conta agora", "?");
                // Navigation.PushAsync(new CadastroPage());
                return;
            }

            var (texto, opcoes) = quiz[perguntaAtual];

            // Pega introdução correspondente, ou reaproveita a última
            string intro = introducoes.Count > perguntaAtual ? introducoes[perguntaAtual] : introducoes[^1];

            PerguntaLabel.Text = $"{intro}\n\n{texto}";
            OpcoesCollection.ItemsSource = opcoes;
        }

        private void OpcoesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0)
                return;

            var resposta = e.CurrentSelection[0].ToString();
            respostas.Add(resposta);
            perguntaAtual++;

            OpcoesCollection.SelectedItem = null;
            MostrarPergunta();
        }
    }
}
