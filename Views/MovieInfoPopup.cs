using CommunityToolkit.Maui.Views;
using LumeClient.DTOs.Movies;

namespace LumeClient.Views
{
    // Essa classe define a estética do Popup que exibe as informações de um filme
    public class MovieInfoPopup : Popup
    {
        // URL base para as imagens dos posters
        private string basePosterPathURL = "https://image.tmdb.org/t/p/w500";
        // Construtor que recebe um objeto MovieDetailsDTO e constrói o Popup
        public MovieInfoPopup(MovieDetailsDTO movie)
        {
            // Frame onde vão ficar todas as informações
            var frame = new Frame
            {
                MaximumWidthRequest = 400,
                CornerRadius = 10,
                BackgroundColor = Colors.White,
                Padding = 10,
                HasShadow = true,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            // Define o caminho do poster do filme
            var posterPath = basePosterPathURL + movie.PosterPath;

            // Elemento que permite scroll dentro do frame, onde vão ficar todos os dados
            var scroll = new ScrollView
            {
                Content = new StackLayout
                {
                    Spacing = 16,
                    Padding = new Thickness(12, 8),
                    Children =
        {
            // Poster centralizado
            new Image
            {
                Source = string.IsNullOrEmpty(movie.PosterPath)
                    ? "no_poster_card.png"
                    : basePosterPathURL + movie.PosterPath,
                Aspect = Aspect.AspectFill,
                HeightRequest = 280,
                WidthRequest = 190,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 0, 12)
            },

            // Título
            new Label
            {
                Text = movie.Title ?? "(Título indisponível)",
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.Black,
                HorizontalTextAlignment = TextAlignment.Center
            },

            // Tagline
            new Label
            {
                Text = string.IsNullOrEmpty(movie.Tagline) ? "" : $"\"{movie.Tagline}\"",
                FontSize = 14,
                FontAttributes = FontAttributes.Italic,
                TextColor = Colors.DarkSlateGray,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 8)
            },

            // Overview
            new Label
            {
                Text = movie.Overview ?? "Sinopse não disponível.",
                FontSize = 14,
                TextColor = Colors.Black,
                LineBreakMode = LineBreakMode.WordWrap
            },

            // Seção: Info técnica
            CreateSectionLabel("📌 Informações Gerais"),
            CreateDetailLabel($"Ano de lançamento: {movie.ReleaseDate?.Year}" ?? "Ano de lançamento não disponível."),
            CreateDetailLabel($"Duração: {movie.Runtime} min" ?? "Duração não disponível."),
            CreateDetailLabel($"Idioma original: {movie.OriginalLanguage?.ToUpper()}" ?? "Idioma original não disponível."),

            // Seção: Avaliação
            CreateSectionLabel("⭐ Avaliação"),
            CreateDetailLabel($"Nota média: {movie.VoteAverage:F2} ({movie.VoteCount} votos)" ?? "Avaliação não disponível"),

            // Seção: Produção
            CreateSectionLabel("🎬 Produção"),
            CreateDetailLabel($"Orçamento: ${movie.Budget:N0}" ?? "Orçamento não disponível."),
            CreateDetailLabel($"Receita: ${movie.Revenue:N0}" ?? "Receita não disponível."),
            CreateDetailLabel($"Produtoras: {string.Join(", ", movie.ProductionCompanies)}"),
            CreateDetailLabel($"Países: {string.Join(", ", movie.ProductionCountries)}"),

            // Seção: Gêneros e Palavras-chave
            CreateSectionLabel("🏷️ Gêneros e Temas"),
            CreateDetailLabel($"Gêneros: {string.Join(", ", movie.Genres)}"),
            CreateDetailLabel($"Palavras-chave: {string.Join(", ", movie.Keywords)}"),

            // Seção: Idiomas falados
            CreateSectionLabel("🗣️ Idiomas falados"),
            CreateDetailLabel(string.Join(", ", movie.SpokenLanguages)),

            // Seção: Links
            CreateSectionLabel("🔗 Links úteis"),
            new Label
            {
                Text = movie.Homepage ?? "Site oficial não disponível.",
                TextColor = Colors.Blue,
                FontSize = 14,
                GestureRecognizers =
                {
                    new TapGestureRecognizer
                    {
                        Command = new Command(async () =>
                        {
                            if (!string.IsNullOrWhiteSpace(movie.Homepage))
                                await Launcher.Default.OpenAsync(movie.Homepage);
                        })
                    }
                }
            }
        }
                }
            };

            // Botão de fechar
            var closeBtn = new Button
            {
                Text = "Fechar",
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromArgb("#FFD700"),
                FontAttributes = FontAttributes.Bold

            };

            // Versão curta do evento Clicked no botão de fechar
            closeBtn.Clicked += (s, e) => Close();

            var scrollContainer = new Grid
            {
                HeightRequest = 500, // ou calcule dinamicamente com DeviceDisplay.MainDisplayInfo.Height
                Children = { scroll }
            };

            // Definição do layout
            var layout = new VerticalStackLayout
            {
                Spacing = 10,
                Children = { scrollContainer, closeBtn },
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };



            // Adiciona o layout ao frame
            frame.Content = layout;
            // Define o conteúdo do Popup
            Content = frame;
        }
        private Label CreateSectionLabel(string title)
        {
            return new Label
            {
                Text = title,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.DarkSlateBlue,
                Margin = new Thickness(0, 12, 0, 4)
            };
        }

        private Label CreateDetailLabel(string text)
        {
            return new Label
            {
                Text = text,
                FontSize = 14,
                TextColor = Colors.Black
            };
        }
    }
}
