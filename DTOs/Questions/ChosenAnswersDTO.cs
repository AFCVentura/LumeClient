using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeClient.DTOs.Questions
{
    class ChosenAnswersDTO
    {
        public List<int> ExtraAnswerIds { get; set; } = new();
        public List<int> ThemeAnswerIds { get; set; } = new();
    }
}
