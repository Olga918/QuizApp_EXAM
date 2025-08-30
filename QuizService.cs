using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace QuizApp_EXAM
{
    public class QuizService
    {
        private List<Quiz> quizzes;

        private static readonly List<string> FunnyComments = new List<string>
        {
            "🤦‍♂️ Ой-ой! Может, стоит сходить прогуляться на свежем воздухе?",
            "😅 Вы что, совсем? Даже мой кот знает лучше!",
            "🤪 Ха-ха! Это было... интересно, скажем так!",
            "😵‍💫 Упс! Кажется, вы перепутали викторину с гаданием!",
            "🤡 Клоун дня! Но ничего страшного, все ошибаются!",
            "😴 Бррр... Может, стоит выпить кофе и попробовать снова?",
            "🤔 Хм... Интересная логика! Но неправильная!",
            "😱 Ай-яй-яй! Это даже не близко к правильному ответу!",
            "🤯 Взрыв мозга! Но в неправильную сторону!",
            "😵 Головокружение от неправильности!",
            "🤠 Ковбой, вы потеряли свой ум где-то на Диком Западе!",
            "🧠 Мозг в отпуске? Понимаю, бывает!",
            "🎭 Театральная постановка 'Как не надо отвечать'!",
            "🎪 Цирк приехал! И вы - главный клоун!",
            "🚀 Вы улетели в космос, но не туда!",
            "🌊 Утонули в море неправильности!",
            "🔥 Горите ярко, но не тем, что нужно!",
            "❄️ Замерзли от неправильности!",
            "⚡ Удар молнии по неправильности!",
            "💫 Звездный час... но не ваш!"
        };

        public QuizService()
        {
            quizzes = Database.LoadQuizzes();
            
            if (quizzes == null)
            {
                quizzes = new List<Quiz>();
            }
            
            foreach (var quiz in quizzes)
            {
                if (quiz != null && string.IsNullOrEmpty(quiz.Name) && !string.IsNullOrEmpty(quiz.Category))
                {
                    quiz.Name = quiz.Category;
                }
            }
        }

        public List<Quiz> GetQuizzes()
        {
            return quizzes;
        }

        public void StartQuiz(User user, string category, int timeLimit = 0)
        {
            Quiz quiz;
            string quizCategory;

            if (category.ToLower() == "смешанная" || category.ToLower() == "mixed")
            {
                quiz = CreateMixedQuiz();
                quizCategory = "Смешанная";
            }
            else
            {
                quiz = quizzes.FirstOrDefault(q => q.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
                quizCategory = category;
            }

            if (quiz == null || quiz.Questions.Count == 0)
            {
                Console.WriteLine("❌ Такой викторины нет или в ней нет вопросов!");
                return;
            }

            Console.WriteLine($"\n🎯 Начинаем викторину: {quizCategory}");
            Console.WriteLine($"📝 Всего вопросов: {Math.Min(20, quiz.Questions.Count)}");
            
            if (timeLimit > 0)
            {
                Console.WriteLine($"⏱️ Ограничение времени: {timeLimit} секунд на ответ");
            }
            else
            {
                Console.WriteLine("♾️ Без ограничения времени");
            }
            
            Console.WriteLine("Нажмите Enter для начала...");
            Console.ReadLine();

            int score = 0;
            int totalQuestions = Math.Min(20, quiz.Questions.Count);
            var randomQuestions = quiz.Questions.OrderBy(x => Guid.NewGuid()).Take(totalQuestions).ToList();

            for (int i = 0; i < randomQuestions.Count; i++)
            {
                var q = randomQuestions[i];
                Console.WriteLine($"\n❓ Вопрос {i + 1} из {totalQuestions}:");
                Console.WriteLine($"📚 Категория: {q.Category}");
                Console.WriteLine($"💭 {q.Text}");
                
                for (int j = 0; j < q.Options.Count; j++)
                    Console.WriteLine($"{j + 1}. {q.Options[j]}");

                if (q.CorrectAnswers.Count > 1)
                {
                    Console.WriteLine("🔘 Выберите варианты ответа (введите номера через запятую):");
                }
                else
                {
                    Console.WriteLine("🔘 Выберите вариант ответа (введите номер):");
                }

                Console.Write("Ваш ответ: ");
                string input = "";
                
                if (timeLimit > 0)
                {
                    input = ReadAnswerWithTimer(timeLimit);
                }
                else
                {
                    input = Console.ReadLine();
                }
                
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("❌ Ответ не введен. Вопрос не засчитан.");
                    Console.WriteLine($"💡 Правильный ответ: {GetCorrectAnswersText(q)}");
                    Console.WriteLine($"😅 {GetRandomFunnyComment()}");
                    continue;
                }

                var answers = input.Split(',')
                                   .Where(s => int.TryParse(s.Trim(), out _))
                                   .Select(x => int.Parse(x.Trim()) - 1)
                                   .ToList();

                if (answers.Count == 0)
                {
                    Console.WriteLine("❌ Неверный формат ответа. Вопрос не засчитан.");
                    Console.WriteLine($"💡 Правильный ответ: {GetCorrectAnswersText(q)}");
                    Console.WriteLine($"😅 {GetRandomFunnyComment()}");
                    continue;
                }

                if (answers.Any(a => a < 0 || a >= q.Options.Count))
                {
                    Console.WriteLine("❌ Один или несколько ответов выходят за пределы допустимого диапазона. Вопрос не засчитан.");
                    Console.WriteLine($"💡 Правильный ответ: {GetCorrectAnswersText(q)}");
                    Console.WriteLine($"😅 {GetRandomFunnyComment()}");
                    continue;
                }

                if (answers.Count == q.CorrectAnswers.Count &&
                    !q.CorrectAnswers.Except(answers).Any() &&
                    !answers.Except(q.CorrectAnswers).Any())
                {
                    score++;
                    Console.WriteLine("✅ Правильно! Отличная работа!");
                }
                else
                {
                    Console.WriteLine("❌ Неправильно!");
                    Console.WriteLine($"💡 Правильный ответ: {GetCorrectAnswersText(q)}");
                    Console.WriteLine($"😅 {GetRandomFunnyComment()}");
                }
                
                Console.WriteLine(new string('─', 50));
            }

            var result = new Result
            {
                Category = quizCategory,
                Score = score,
                TotalQuestions = totalQuestions,
                Date = DateTime.Now
            };

            user.Results.Add(result);
            Database.SaveUsers(Database.LoadUsers());

            Console.WriteLine($"\n🎉 Викторина завершена!");
            Console.WriteLine($"📊 Ваш результат: {score} из {totalQuestions} ({(double)score / totalQuestions * 100:F1}%)");
            
            ShowUserPosition(user, quizCategory, score);
        }

        private Quiz CreateMixedQuiz()
        {
            var mixedQuiz = new Quiz { Name = "Смешанная", Questions = new List<Question>(), Results = new List<Result>() };
            
            foreach (var quiz in quizzes)
            {
                if (quiz.Questions != null)
                {
                    mixedQuiz.Questions.AddRange(quiz.Questions);
                }
            }
            
            var randomQuestions = mixedQuiz.Questions.OrderBy(x => Guid.NewGuid()).Take(20).ToList();
            mixedQuiz.Questions = randomQuestions;
            
            return mixedQuiz;
        }

        private static void ShowUserPosition(User user, string category, int score)
        {
            var allUsers = Database.LoadUsers();
            var categoryResults = new List<Result>();
            
            foreach (var u in allUsers)
            {
                categoryResults.AddRange(u.Results.Where(r => r.Category == category));
            }
            
            if (categoryResults.Count == 0) return;
            
            var sortedResults = categoryResults.OrderByDescending(r => r.Score).ToList();
            var userResult = user.Results.FirstOrDefault(r => r.Category == category && r.Score == score);
            
            if (userResult != null)
            {
                var userPosition = sortedResults.FindIndex(r => r == userResult) + 1;
                Console.WriteLine($"🏆 Ваша позиция в топе: {userPosition} из {sortedResults.Count}");
                
                if (userPosition == 1)
                {
                    Console.WriteLine("🥇 Поздравляем! Вы на первом месте!");
                }
                else if (userPosition <= 3)
                {
                    Console.WriteLine("🎉 Отличный результат! Вы в тройке лидеров!");
                }
                else if (userPosition <= 10)
                {
                    Console.WriteLine("👍 Хороший результат! Вы в топ-10!");
                }
            }
        }

        public void ShowTop(string category)
        {
            var quiz = quizzes.FirstOrDefault(q => q.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
            if (quiz == null)
            {
                Console.WriteLine("❌ Категория не найдена!");
                return;
            }

            var allUsers = Database.LoadUsers();
            var categoryResults = new List<Result>();
            
            foreach (var user in allUsers)
            {
                categoryResults.AddRange(user.Results.Where(r => r.Category == category));
            }
            
            if (categoryResults.Count == 0)
            {
                Console.WriteLine("📊 Пока нет результатов для этой категории.");
                return;
            }

            var topResults = categoryResults
                .OrderByDescending(r => r.Score)
                .ThenBy(r => r.Date)
                .Take(20)
                .ToList();

            Console.WriteLine($"\n🏆 ТОП-20 по категории '{category}':");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"{"Место",-6} {"Логин",-15} {"Баллы",-8} {"Дата",-20}");
            Console.WriteLine(new string('=', 60));

            for (int i = 0; i < topResults.Count; i++)
            {
                var result = topResults[i];
                var user = allUsers.FirstOrDefault(u => u.Results.Contains(result));
                string login = user?.Login ?? "Неизвестно";
                
                string medal = i switch
                {
                    0 => "🥇",
                    1 => "🥈", 
                    2 => "🥉",
                    _ => $" {i + 1}."
                };
                
                Console.WriteLine($"{medal,-6} {login,-15} {result.Score,-8} {result.Date:dd.MM.yyyy HH:mm}");
            }
            
            Console.WriteLine(new string('=', 60));
        }

        public void ShowAvailableCategories()
        {
            Console.WriteLine("\n📚 Доступные категории викторин:");
            Console.WriteLine(new string('=', 40));
            
            if (quizzes == null || quizzes.Count == 0)
            {
                Console.WriteLine("❌ Категории не найдены. База данных пуста.");
                Console.WriteLine("Проверьте файл questions.json");
                return;
            }
            
            foreach (var quiz in quizzes)
            {
                if (quiz != null && !string.IsNullOrEmpty(quiz.Name))
                {
                    int questionCount = quiz.Questions?.Count ?? 0;
                    Console.WriteLine($"• {quiz.Name} ({questionCount} вопросов)");
                }
            }
            Console.WriteLine("• Смешанная (вопросы из всех категорий)");
            Console.WriteLine(new string('=', 40));
        }

        private static string GetRandomFunnyComment()
        {
            Random random = new Random();
            return FunnyComments[random.Next(FunnyComments.Count)];
        }

        private string GetCorrectAnswersText(Question question)
        {
            var correctOptions = question.CorrectAnswers.Select(i => question.Options[i]).ToList();
            return string.Join(", ", correctOptions);
        }

        private string ReadAnswerWithTimer(int timeLimit)
        {
            var input = new System.Text.StringBuilder();
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(timeLimit);
            
            Console.CursorVisible = true;
            
            while (DateTime.Now < endTime)
            {
                var remainingTime = (int)(endTime - DateTime.Now).TotalSeconds;
                
                // Очищаем строку и показываем оставшееся время
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"Ваш ответ: {input} | ⏱️ Осталось: {remainingTime} сек");
                
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        return input.ToString();
                    }
                    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input.Length--;
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        input.Append(key.KeyChar);
                    }
                }
                
                Thread.Sleep(100); // Небольшая задержка
            }
            
            Console.WriteLine();
            Console.WriteLine("⏰ Время истекло! Ответ не принят.");
            return "";
        }
    }
}
