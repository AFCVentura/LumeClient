using CommunityToolkit.Maui.Views;

namespace LumeClient.Views
{
    public class TutorialPopup : Popup
    {
        private readonly List<string> messages;
        private readonly List<string?> mediaFiles; // caminho relativo em Resources/Images, ou null
        private int currentIndex = 0;

        // Elementos visuais
        private readonly Label messageLabel;
        private readonly MediaElement mediaElement;
        private readonly Button backButton;
        private readonly Button nextButton;

        public TutorialPopup(List<string> messages, List<string?> mediaFiles)
        {
            if (messages == null || mediaFiles == null || messages.Count != mediaFiles.Count)
                throw new ArgumentException("messages e gifFiles devem ter mesmo tamanho.");

            this.messages = messages;
            this.mediaFiles = mediaFiles;

            // Frame principal
            var frame = new Frame
            {
                MaximumWidthRequest = 250,
                MinimumHeightRequest = 300,
                CornerRadius = 0,
                BackgroundColor = Colors.White,
                Padding = 12,
                HasShadow = true,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            // Label de mensagem
            messageLabel = new Label
            {
                FontSize = 16,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center
            };

            // Image para GIF (visível apenas se houver GIF nesta etapa)
            mediaElement = new MediaElement
            {
                ShouldAutoPlay = true,
                ShouldLoopPlayback = true,
                ShouldShowPlaybackControls = false,
                HeightRequest = 150,
                MaximumWidthRequest = 150,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = false
            };

            // Botões
            backButton = new Button
            {
                Text = "← Voltar",
                IsEnabled = false,
                BackgroundColor = Colors.Gray

            };
            backButton.Clicked += OnBackClicked;

            nextButton = new Button
            {
                Text = "Próximo →",
                BackgroundColor = Color.FromArgb("#FFD700")
            };
            nextButton.Clicked += OnNextClicked;

            // Layout dos botões
            var buttonLayout = new HorizontalStackLayout
            {
                Spacing = 20,
                HorizontalOptions = LayoutOptions.Center,
                Children = { backButton, nextButton }
            };

            // Conteúdo interno: empilha texto, GIF e botões
            var contentStack = new VerticalStackLayout
            {
                Spacing = 12,
                Children = { messageLabel, mediaElement, buttonLayout },
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill
            };

            frame.Content = contentStack;
            Content = frame;

            // Inicializa com a primeira mensagem
            UpdateContent();
        }

        private void UpdateContent()
        {
            // Ajusta texto
            messageLabel.Text = messages[currentIndex];

            // Ajusta GIF
            var mediaFile = mediaFiles[currentIndex];
            if (!string.IsNullOrEmpty(mediaFile))
            {
                mediaElement.Source = MediaSource.FromResource(mediaFile);
                mediaElement.IsVisible = true;

                mediaElement.Stop();
                mediaElement.Play();
            }
            else
            {
                mediaElement.IsVisible = false;
                mediaElement.Stop();
            }

            // Ajusta botões
            backButton.IsEnabled = currentIndex > 0;
            nextButton.Text = (currentIndex == messages.Count - 1) ? "Fechar" : "Próximo →";
        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                UpdateContent();
            }
        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            if (currentIndex < messages.Count - 1)
            {
                currentIndex++;
                UpdateContent();
            }
            else
            {
                // Último: fecha o popup
                Close();
            }
        }
    }
}
