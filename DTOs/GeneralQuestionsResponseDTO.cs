using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumeClient.DTOs
{
    class GeneralQuestionsResponseDTO
    {
        public List<ExtraQuestionDTO> ExtraQuestions { get; set; } = new();
        public List<ThemeQuestionDTO> ThemeQuestions { get; set; } = new();
    }
}
