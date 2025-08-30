using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace QuizApp_EXAM
{
    public class Question
    {
        [JsonPropertyName("Text")]
        public string Text { get; set; }
        [JsonPropertyName("Options")]
        public List<string> Options { get; set; } = new List<string>();
        [JsonPropertyName("CorrectAnswers")]
        public List<int> CorrectAnswers { get; set; } = new List<int>();
        [JsonPropertyName("Category")]
        public string Category { get; set; }
    }
}
