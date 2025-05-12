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
            "Estamos quase l�...",
            "Faltam s� mais algumas...",
            "�ltima perguntinha, juro!"
        };

        private readonly List<(string pergunta, List<string> opcoes)> quiz = new()
        {
            ("Quem � voc� quando assiste filmes?", new()
            {
                "Aquele que dorme na primeira oportunidade.",
                "Aquele que conhece todos os atores e diretores, super cin�filo.",
                "Aquele que chora at� em comercial de margarina.",
                "O ca�ador de plot twist: se n�o tiver reviravolta, nem perco tempo.",
                "Aquele que ama uma fantasia, at� por que, a realidade j� � dif�cil.",
                "Aquele que ama um filme bem gr�fico (quanto mais sangue melhor).",
                "O f� de explos�es, persegui��es e muita bagun�a na tela."
            }),
            ("Na hora de escolher um filme, voc� prefere hist�rias que:", new()
            {
                "Fica mais curioso e quer ver na hora.",
                "Acha interessante, mas prefere decidir sozinho.",
                "Vai pelo seu pr�prio feeling, nota n�o � tudo.",
                "Nem repara, s� assiste."
            }),
            ("Voc� diria que os seus filmes preferidos, s�o:", new()
            {
                "Bem conhecidos e queridinhos do p�blico.",
                "Pequenos tesouros escondidos",
                "Super premiados, cheios de trof�u.",
                "T�o esquisitos que s� voc� mesmo entende.",
                "Uma mistura geral de tudo que � bom."
            }),
            ("Pra voc�, um filme perfeito seria:", new()
            {
                "De prefer�ncia em outra realidade, outro planeta, outra dimens�o.",
                "Aqueles nos tempos da antiguidade, bem 'Babil�nico', 'Fara�nico', 'Mesopot�mico' e tudo '�nico'.",
                "Aqueles que at� em Preto e Branco eram filmados.",
                "Bem medieval, com reis, rainhas e at� pragas.",
                "Bem Vintage, Retr�, Nost�lgico. Aquilo que era �poca.",
                "O aqui e agora � bem melhor!",
                "Um universo m�gico cheio de seres fant�sticos.",
                "Ou naquelas cabanas do terror cl�ssico, com assassino maluco na floresta."
            }),
            ("Quando voc� termina um filme, voc� geralmente:", new()
            {
                "Cria finais alternativos na cabe�a.",
                "Fica viajando na hist�ria por horas.",
                "Corre pra contar pra algu�m e indicar.",
                "J� emendo outro filme na sequ�ncia.",
                "Gosto e tal, mas logo esque�o e vou viver a vida",
                "Vou buscar se tem uma sequ�ncia, adoro uma franquia."
            }),
            ("Na sua opini�o, um final perfeito �:", new()
            {
                "Aquele que d� um n� na cabe�a, me deixa paralisado, sem rea��o e deixa mil interpreta��es.",
                "Um filme redondinho, bem fechado, sem pontas soltas.",
                "Super emocionante, mesmo que triste (me deixa chorar em paz).",
                "Alegre e pra cima, porque drama j� basta a vida.",
                "O que importa � a experi�ncia inteira, final � detalhe."
            }),
            ("Se tivesse que descrever seu gosto para filmes em uma palavra, seria:", new()
            {
                "Intenso", "Profundo", "Divertido", "Surpreendente", "Sonhador", "Realista",
                "N�o tenho, eu s� quero me entreter durante umas horinhas."
            }),
            ("Na hora de escolher o que assistir, o que pesa mais?", new()
            {
                "Ou o trailer me conquista, ou me faz desistir.",
                "A vibe do dia, sem muito crit�rio.",
                "O g�nero que j� amo e conhe�o bem.",
                "A indica��o daquela pessoa que entende meu gosto.",
                "A curiosidade de ver algo novo e diferente.",
                "O ator ou atriz que est� no filme."
            }),
            ("Se voc� fosse um protagonista de um filme, o que voc� gostaria de ser:", new()
            {
                "Gostaria de ter super poderes ou ser um super-her�i.",
                "Viver romances imposs�veis e inesquec�veis.",
                "Ser um super vil�o.",
                "Ser aquele protagonista bonzinho e que todo mundo adora",
                "Aquele protagonista t�pico de filme de a��o, que quando pisam no calo dele se transforma no CARA."
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

            // Pega introdu��o correspondente, ou reaproveita a �ltima
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
