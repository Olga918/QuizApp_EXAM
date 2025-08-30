using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp_EXAM
{
    public class Result
    {
        public string UserLogin { get; set; }
        public string QuizCategory { get; set; }
        public string Category { get; set; } // Добавляем для совместимости
        public int Score { get; set; }
        public int TotalQuestions { get; set; } // Добавляем общее количество вопросов
        public DateTime Date { get; set; }
    }
}
