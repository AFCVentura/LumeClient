using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeClient.DTOs.Movies
{
    class RecommendedMovieStatusDTO
    {
        public int MovieId { get; set; }
        public bool Liked { get; set; }
        public bool Watched { get; set; }
    }
}
