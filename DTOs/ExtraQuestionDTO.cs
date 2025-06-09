using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LumeClient.DTOs
{
    class ExtraQuestionDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsGeneralProfile { get; set; }
        public bool IsMultipleChoice { get; set; }
        [JsonPropertyName("answers")]
        public List<ExtraAnswerDTO> ExtraAnswers { get; set; } = new();
    }
}
