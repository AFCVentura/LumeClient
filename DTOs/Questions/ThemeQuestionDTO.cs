using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeClient.DTOs
{
    class ThemeQuestionDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsGeneralProfile { get; set; }
        public bool IsMultipleChoice { get; set; }
        public List<ThemeAnswerDTO> ThemeAnswers { get; set; } = new();
    }
}
