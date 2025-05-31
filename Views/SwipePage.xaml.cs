using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace LumeClient.Views
{
    public partial class SwipePage : ContentPage
    {
        private double xOffset, yOffset;
        private List<string> favoritos = new();

        public SwipePage()
        {
            InitializeComponent();
            CarregarFilme();
        }

        private void CarregarFilme()
        {
            // só pra testar moka
            MovieTitle.Text = "Ainda Estou Aqui";
            MovieImage.Source = "filme3.jpg";
            MovieDescription.Text = "No início da década de 1970, o Brasil enfrenta o endurecimento da ditadura militar...";
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    // Atualiza as posições de deslocamento enquanto o gesto está em andamento
                    CardFrame.TranslationX = e.TotalX;
                    CardFrame.TranslationY = e.TotalY;
                    xOffset = e.TotalX;
                    yOffset = e.TotalY;
                    break;

                case GestureStatus.Completed:
                    // Ações com base nos gestos realizados
                    if (xOffset > 100)
                    {
                        // Filme curtido
                        favoritos.Add(MovieTitle.Text);
                        DisplayAlert("Like", $"{MovieTitle.Text} adicionado aos favoritos!", "OK");
                    }
                    else if (xOffset < -100)
                    {
                        // Filme descartado
                        DisplayAlert("Dislike", $"{MovieTitle.Text} descartado.", "OK");
                    }
                    else if (yOffset < -100)
                    {
                        // Filme marcado como assistido (gesto para cima)
                        DisplayAlert("Assistido", $"{MovieTitle.Text} marcado como assistido.", "OK");
                    }
                    else if (yOffset > 100)
                    {
                        // Filme salvo para ver depois (gesto para baixo)
                        DisplayAlert("Talvez", $"{MovieTitle.Text} salvo para ver depois.", "OK");
                    }

                    // Restaura a posição do cartão
                    CardFrame.TranslationX = 0;
                    CardFrame.TranslationY = 0;
                    xOffset = yOffset = 0;

                    // Carregar um novo filme após a ação
                    CarregarFilme();
                    break;
            }
        }

        private void OnConfigClicked(object sender, EventArgs e)
        {
            // Navegar para configurações
        }

        private void OnBellClicked(object sender, EventArgs e)
        {
            // Notificações
        }

        private void OnFavClicked(object sender, EventArgs e)
        {
            // Futuro: Navegar para tela de favoritos
        }
    }
}
