using CommunityToolkit.Maui.Views;

namespace LumeClient.Views
{
    public class TutorialPopup : Popup
    {
        private readonly List<string> messages;
        private readonly List<string?> mediaFiles;
        private int currentIndex = 0;

        private readonly Label messageLabel;
        private readonly MediaElement mediaElement;
        private readonly Button backButton;
        private readonly Button nextButton;

        public TutorialPopup(List<string> messages, List<string?> mediaFiles)
        {
            if (messages == null || mediaFiles == null || messages.Count != mediaFiles.Count)
                throw new ArgumentException("messages e mediaFiles devem ter mesmo tamanho.");

            this.messages = messages;
            this.mediaFiles = mediaFiles;

            // 1) Cria os elementos de UI
            messageLabel = new Label
            {
                FontSize = 16,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center
            };

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

            // 2) Layout dos botões (sempre na base)
            var buttonLayout = new HorizontalStackLayout
            {
                Spacing = 20,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(0, 8, 0, 0),
                Children = { backButton, nextButton }
            };

            // 3) Conteúdo rolável (texto + mídia)
            var contentStack = new VerticalStackLayout
            {
                Spacing = 12,
                Children = { messageLabel, mediaElement },
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };

            var scroll = new ScrollView
            {
                Content = contentStack,
                VerticalOptions = LayoutOptions.Center
            };

            // 4) Grid com 2 linhas: scroll na linha 0 e botões na linha 1
            var grid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };
            grid.Add(scroll, 0, 0);
            grid.Add(buttonLayout, 0, 1);

            // 5) Frame principal
            var frame = new Frame
            {
                WidthRequest = 250,
                HeightRequest = 330,    
                BackgroundColor = Colors.White,
                Padding = 12,
                HasShadow = true,
                Content = grid,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            Content = frame;

            // 6) Exibe a primeira mensagem
            UpdateContent();
        }

        private void UpdateContent()
        {
            // Texto
            messageLabel.Text = messages[currentIndex];

            // GIF (se houver)
            var file = mediaFiles[currentIndex];
            if (!string.IsNullOrEmpty(file))
            {
                mediaElement.Source = MediaSource.FromResource(file);
                mediaElement.IsVisible = true;
                mediaElement.Stop();
                mediaElement.Play();
            }
            else
            {
                mediaElement.IsVisible = false;
                mediaElement.Stop();
            }

            // Botões
            backButton.IsEnabled = (currentIndex > 0);
            nextButton.Text = (currentIndex == messages.Count - 1)
                ? "Fechar"
                : "Próximo →";
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
                Close();
            }
        }
    }
}
