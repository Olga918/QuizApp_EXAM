using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace QuizApp_EXAM
{
    public class Quiz
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        
        [JsonPropertyName("Category")]
        public string Category { get; set; }
        
        [JsonPropertyName("Questions")]
        public List<Question> Questions { get; set; } = new List<Question>();
        
        [JsonPropertyName("Results")]
        public List<Result> Results { get; set; } = new List<Result>();
        
        public Quiz()
        {
            if (string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Category))
            {
                Name = Category;
            }
        }
    }
}
