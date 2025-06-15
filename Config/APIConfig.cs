using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeClient.Config
{
    // Essa classe serve para facilitar a alteração da URL da API do LumeServer que o LumeClient consome
    // Isso está sendo feito pois não é possível executar o LumeServer no computador, o LumeClient no celular e consumir a API usando o endereço localhost, é preciso conectar ambos na mesma rede e especificar o endereço IP do computador que executa o LumeServer para que o celular consiga consumir a API

    // É necessário adicionar o endereço IP do seu dispositivo no campo PossibleIPAdresses e definir qual está sendo usado no momento no campo BaseUrl.
    
    public static class APIConfig
    {
        #region Configuração de endereços IP
        public static readonly Dictionary<string, string> PossibleIPAddresses = new Dictionary<string, string>
        {
            { "NotebookEthernetVenturaHttps", "https://192.168.0.105:7141" },
            { "NotebookEthernetVenturaHttp", "http://192.168.0.105:5249" },
        
        };
        public static string BaseUrl { get; set; } = PossibleIPAddresses["NotebookEthernetVenturaHttp"];
        #endregion

        #region Endpoints

        #region Identity
        // POST /register
        public static string RegisterEndpoint => $"{BaseUrl}/register";
        // POST /login
        public static string LoginEndpoint => $"{BaseUrl}/login";
        // POST /refresh
        public static string RefreshEndpoint => $"{BaseUrl}/refresh";
        // GET /confirmEmail
        public static string ConfirmEmailEndpoint => $"{BaseUrl}/confirmEmail";
        // POST /resendConfirmationEmail
        public static string ResendConfirmationEmailEndpoint => $"{BaseUrl}/resendConfirmationEmail";
        // POST /forgotPassword
        public static string ForgotPasswordEndpoint => $"{BaseUrl}/forgotPassword";
        // POST /resetPassword
        public static string ResetPasswordIdentityEndpoint => $"{BaseUrl}/resetPassword";
        // POST /manage/2fa
        public static string Manage2faEndpoint => $"{BaseUrl}/manage/2fa";
        // GET /manage/info
        public static string ManageInfoGetEndpoint => $"{BaseUrl}/manage/info";
        // POST /manage/info
        public static string ManageInfoPostEndpoint => $"{BaseUrl}/manage/info";

        #endregion
        #region Movie
        // GET /api/v1/movies/carousel-movies/{id}
        public static string CarouselMoviesEndpoint => $"{BaseUrl}/api/v1/movies/carousel-movies/";
        // GET /api/v1/movies/wishlist-movies/{id}
        public static string WishlistMoviesAllEndpoint => $"{BaseUrl}/api/v1/movies/wishlist-movies/";
        // GET /api/v1/movies/wishlist-movies/{id}/wishlisted-movies/{movieId}
        public static string WishlistMoviesByIdFirstPartEndpoint => $"{BaseUrl}/api/v1/movies/carousel-movies/";
        public static string WishlistMoviesByIdLastPartEndpoint => $"/wishlisted-movies/";
        // GET /api/v1/movies/recommended-movies/{id}
        public static string RecommendedMoviesGetEndpoint => $"{BaseUrl}/api/v1/movies/recommended-movies/";
        // POST /api/v1/movies/recommended-movies/{id}
        public static string RecommendedMoviesPostEndpoint => $"{BaseUrl}/api/v1/movies/recommended-movies/";
        #endregion
        
        #region Question
        // GET /api/v1/questions/general-questions
        public static string GeneralQuestionsEndpoint => $"{BaseUrl}/api/v1/questions/general-questions";
        // GET /api/v1/questions/daily-questions
        public static string DailyQuestionsEndpoint => $"{BaseUrl}/api/v1/questions/daily-questions";
        // GET /api/v1/questions/famous-movies
        public static string FamousMoviesEndpoint => $"{BaseUrl}/api/v1/questions/famous-movies";
        // POST /api/v1/questions/general-answers/{id}
        public static string GeneralAnswersEndpoint => $"{BaseUrl}/api/v1/questions/general-answers/";
        // POST /api/v1/questions/daily-answers/{id}
        public static string DailyAnswersEndpoint => $"{BaseUrl}/api/v1/questions/daily-answers/";
        #endregion
        #region User
        // GET /api/v1/users
        public static string MyIdEndpoint => $"{BaseUrl}/api/v1/users/me";
        // POST /api/v1/users/logout
        public static string LogoutEndpoint => $"{BaseUrl}/api/v1/users/logout";
        // PATCH /api/v1/users/change-username
        public static string ChangeUsernameEndpoint => $"{BaseUrl}/api/v1/users/change-username";
        // PATCH /api/v1/users/change-password
        public static string ChangePasswordEndpoint => $"{BaseUrl}/api/v1/users/change-password";
        // POST /api/v1/users/forgot-username
        public static string ForgotUsernameEndpoint => $"{BaseUrl}/api/v1/users/forgot-password";
        // POST /api/v1/users/reset-password
        public static string ResetPasswordEndpoint => $"{BaseUrl}/api/v1/users/reset-password";
        // DELETE /api/v1/users/delete-account
        public static string DeleteAccountEndpoint => $"{BaseUrl}/api/v1/users/delete-account";
        #endregion
        #endregion

    }
}
