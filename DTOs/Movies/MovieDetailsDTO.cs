using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeClient.DTOs.Movies
{
    public class MovieDetailsDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public float VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public string? Status { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public float Runtime { get; set; }
        public bool Adult { get; set; }
        public string? BackdropPath { get; set; }
        public long Budget { get; set; }
        public string? Homepage { get; set; }
        public string? ImdbId { get; set; }
        public string? OriginalLanguage { get; set; }
        public string? OriginalTitle { get; set; }
        public string? Overview { get; set; }
        public float Popularity { get; set; }
        public string PosterPath { get; set; }
        public string? Tagline { get; set; }

        public List<string> Genres { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> ProductionCompanies { get; set; }
        public List<string> ProductionCountries { get; set; }
        public List<string> SpokenLanguages { get; set; }
    }
}
