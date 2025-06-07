using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace LumeClient.Views
{
    public partial class Swipe : ContentPage
    {
        private double xOffset, yOffset;
        private List<string> favoritos = new();

        public Swipe()
        {
            InitializeComponent();
            CarregarFilme();
        }

        private void CarregarFilme()
        {
            // s� pra testar moka
            MovieTitle.Text = "Ainda Estou Aqui";
            MovieImage.Source = "filme3.jpg";
            MovieDescription.Text = "No in�cio da d�cada de 1970, o Brasil enfrenta o endurecimento da ditadura militar...";
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    // Atualiza as posi��es de deslocamento enquanto o gesto est� em andamento
                    CardFrame.TranslationX = e.TotalX;
                    CardFrame.TranslationY = e.TotalY;
                    xOffset = e.TotalX;
                    yOffset = e.TotalY;
                    break;

                case GestureStatus.Completed:
                    // A��es com base nos gestos realizados
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

                    // Restaura a posi��o do cart�o
                    CardFrame.TranslationX = 0;
                    CardFrame.TranslationY = 0;
                    xOffset = yOffset = 0;

                    // Carregar um novo filme ap�s a a��o
                    CarregarFilme();
                    break;
            }
        }

        private void OnConfigClicked(object sender, EventArgs e)
        {
            // Navegar para configura��es
        }

        private void OnBellClicked(object sender, EventArgs e)
        {
            // Notifica��es
        }

        private void OnFavClicked(object sender, EventArgs e)
        {
            // Futuro: Navegar para tela de favoritos
        }
    }
}
