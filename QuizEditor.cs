using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp_EXAM
{
    public class QuizEditor
    {
        private List<Quiz> quizzes;

        public QuizEditor()
        {
            quizzes = Database.LoadQuizzes();
        }

        public void OpenEditor()
        {
            Console.WriteLine("✅ Доступ к редактору разрешен!");
            
            while (true)
            {
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("РЕДАКТОР ВИКТОРИН - ПАНЕЛЬ АДМИНИСТРАТОРА");
                Console.WriteLine(new string('=', 60));
                Console.WriteLine("1. 📚 Добавить новую категорию");
                Console.WriteLine("2. ❓ Добавить вопрос в категорию");
                Console.WriteLine("3. 📋 Показать все категории и вопросы");
                Console.WriteLine("4. ✏️  Редактировать существующий вопрос");
                Console.WriteLine("5. 🗑️  Удалить вопрос");
                Console.WriteLine("6. 🗑️  Удалить категорию");
                Console.WriteLine("7. 📊 Статистика викторин");
                Console.WriteLine("8. 💾 Сохранить изменения");
                Console.WriteLine("9. 🔙 Выйти из редактора");
                Console.WriteLine(new string('=', 60));
                Console.Write("Выберите действие: ");
                
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddCategory();
                            break;
                        case "2":
                            AddQuestion();
                            break;
                        case "3":
                            ShowCategories();
                            break;
                        case "4":
                            EditQuestion();
                            break;
                        case "5":
                            DeleteQuestion();
                            break;
                        case "6":
                            DeleteCategory();
                            break;
                        case "7":
                            ShowStatistics();
                            break;
                        case "8":
                            SaveChanges();
                            break;
                        case "9":
                            Console.WriteLine("Выход из редактора...");
                            return;
                        default:
                            Console.WriteLine("❌ Неверный выбор. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Произошла ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
            }
        }

        private void AddCategory()
        {
            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine("ДОБАВЛЕНИЕ НОВОЙ КАТЕГОРИИ");
            Console.WriteLine(new string('-', 40));
            
            Console.Write("Введите название новой категории: ");
            string name = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("❌ Название категории не может быть пустым!");
                return;
            }
            
            if (quizzes.Any(q => q.Category.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("❌ Такая категория уже существует!");
                return;
            }
            
            var newQuiz = new Quiz 
            { 
                Category = name, 
                Questions = new List<Question>(), 
                Results = new List<Result>() 
            };
            
            quizzes.Add(newQuiz);
            Console.WriteLine($"✅ Категория '{name}' успешно добавлена!");
        }

        private void AddQuestion()
        {
            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine("ДОБАВЛЕНИЕ НОВОГО ВОПРОСА");
            Console.WriteLine(new string('-', 40));
            
            if (quizzes.Count == 0)
            {
                Console.WriteLine("❌ Сначала создайте хотя бы одну категорию!");
                return;
            }

            ShowCategories();
            
            Console.Write("\nВведите название категории: ");
            string category = Console.ReadLine();
            var quiz = quizzes.FirstOrDefault(q => q.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            
            if (quiz == null)
            {
                Console.WriteLine("❌ Категория не найдена!");
                return;
            }

            Question question = new Question();
            
            Console.Write("Введите текст вопроса: ");
            question.Text = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(question.Text))
            {
                Console.WriteLine("❌ Текст вопроса не может быть пустым!");
                return;
            }

            Console.Write("Сколько вариантов ответа? (минимум 2): ");
            if (!int.TryParse(Console.ReadLine(), out int optionsCount) || optionsCount < 2)
            {
                Console.WriteLine("❌ Количество вариантов должно быть не менее 2!");
                return;
            }

            for (int i = 0; i < optionsCount; i++)
            {
                Console.Write($"Вариант {i + 1}: ");
                string option = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(option))
                {
                    Console.WriteLine("❌ Вариант ответа не может быть пустым!");
                    i--; // Повторяем ввод
                    continue;
                }
                question.Options.Add(option);
            }

            Console.Write("Введите номера правильных ответов через запятую: ");
            string correctAnswersInput = Console.ReadLine();
            
            try
            {
                question.CorrectAnswers = correctAnswersInput
                    .Split(',')
                    .Select(x => int.Parse(x.Trim()) - 1)
                    .ToList();

                // Проверяем корректность индексов
                if (question.CorrectAnswers.Any(a => a < 0 || a >= question.Options.Count))
                {
                    Console.WriteLine("❌ Один или несколько индексов правильных ответов выходят за пределы диапазона!");
                    return;
                }

                if (question.CorrectAnswers.Count == 0)
                {
                    Console.WriteLine("❌ Должен быть указан хотя бы один правильный ответ!");
                    return;
                }
            }
            catch
            {
                Console.WriteLine("❌ Неверный формат правильных ответов!");
                return;
            }

            quiz.Questions.Add(question);
            Console.WriteLine($"✅ Вопрос успешно добавлен в категорию '{category}'!");
        }

        private void ShowCategories()
        {
            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine("СУЩЕСТВУЮЩИЕ КАТЕГОРИИ И ВОПРОСЫ");
            Console.WriteLine(new string('-', 40));
            
            if (quizzes.Count == 0)
            {
                Console.WriteLine("Категории не найдены.");
                return;
            }

            foreach (var quiz in quizzes)
            {
                Console.WriteLine($"\n📚 {quiz.Category} ({quiz.Questions.Count} вопросов)");
                
                if (quiz.Questions.Count > 0)
                {
                    for (int i = 0; i < quiz.Questions.Count; i++)
                    {
                        var q = quiz.Questions[i];
                        Console.WriteLine($"   {i + 1}. {q.Text}");
                        Console.WriteLine($"      Варианты: {string.Join(", ", q.Options)}");
                        Console.WriteLine($"      Правильные: {string.Join(", ", q.CorrectAnswers.Select(a => a + 1))}");
                    }
                }
            }
        }

        private void EditQuestion()
        {
            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine("РЕДАКТИРОВАНИЕ ВОПРОСА");
            Console.WriteLine(new string('-', 40));
            
            if (quizzes.Count == 0)
            {
                Console.WriteLine("❌ Нет доступных категорий для редактирования!");
                return;
            }

            ShowCategories();
            
            Console.Write("\nВведите название категории: ");
            string category = Console.ReadLine();
            var quiz = quizzes.FirstOrDefault(q => q.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            
            if (quiz == null || quiz.Questions.Count == 0)
            {
                Console.WriteLine("❌ Категория не найдена или в ней нет вопросов!");
                return;
            }

            Console.Write("Введите номер вопроса для редактирования: ");
            if (!int.TryParse(Console.ReadLine(), out int questionIndex) || questionIndex < 1 || questionIndex > quiz.Questions.Count)
            {
                Console.WriteLine("❌ Неверный номер вопроса!");
                return;
            }

            var question = quiz.Questions[questionIndex - 1];
            Console.WriteLine($"\nРедактируем вопрос: {question.Text}");
            
            Console.WriteLine("1. Изменить текст вопроса");
            Console.WriteLine("2. Изменить варианты ответов");
            Console.WriteLine("3. Изменить правильные ответы");
            Console.Write("Выберите что изменить: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    Console.Write("Новый текст вопроса: ");
                    question.Text = Console.ReadLine();
                    break;
                case "2":
                    EditQuestionOptions(question);
                    break;
                case "3":
                    EditCorrectAnswers(question);
                    break;
                default:
                    Console.WriteLine("❌ Неверный выбор!");
                    return;
            }
            
            Console.WriteLine("✅ Вопрос успешно отредактирован!");
        }

        private void EditQuestionOptions(Question question)
        {
            Console.WriteLine("Текущие варианты ответов:");
            for (int i = 0; i < question.Options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {question.Options[i]}");
            }
            
            Console.Write("Введите новые варианты ответов через запятую: ");
            string newOptions = Console.ReadLine();
            var options = newOptions.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            
            if (options.Count < 2)
            {
                Console.WriteLine("❌ Должно быть не менее 2 вариантов ответа!");
                return;
            }
            
            question.Options = options;
            // Сбрасываем правильные ответы, так как количество вариантов изменилось
            question.CorrectAnswers.Clear();
        }

        private void EditCorrectAnswers(Question question)
        {
            Console.WriteLine("Текущие варианты ответов:");
            for (int i = 0; i < question.Options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {question.Options[i]}");
            }
            
            Console.Write("Введите номера правильных ответов через запятую: ");
            string correctAnswersInput = Console.ReadLine();
            
            try
            {
                var newCorrectAnswers = correctAnswersInput
                    .Split(',')
                    .Select(x => int.Parse(x.Trim()) - 1)
                    .ToList();

                if (newCorrectAnswers.Any(a => a < 0 || a >= question.Options.Count))
                {
                    Console.WriteLine("❌ Один или несколько индексов выходят за пределы диапазона!");
                    return;
                }

                question.CorrectAnswers = newCorrectAnswers;
            }
            catch
            {
                Console.WriteLine("❌ Неверный формат правильных ответов!");
            }
        }

        private void DeleteQuestion()
        {
            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine("УДАЛЕНИЕ ВОПРОСА");
            Console.WriteLine(new string('-', 40));
            
            if (quizzes.Count == 0)
            {
                Console.WriteLine("❌ Нет доступных категорий!");
                return;
            }

            ShowCategories();
            
            Console.Write("\nВведите название категории: ");
            string category = Console.ReadLine();
            var quiz = quizzes.FirstOrDefault(q => q.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            
            if (quiz == null || quiz.Questions.Count == 0)
            {
                Console.WriteLine("❌ Категория не найдена или в ней нет вопросов!");
                return;
            }

            Console.Write("Введите номер вопроса для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int questionIndex) || questionIndex < 1 || questionIndex > quiz.Questions.Count)
            {
                Console.WriteLine("❌ Неверный номер вопроса!");
                return;
            }

            var question = quiz.Questions[questionIndex - 1];
            Console.WriteLine($"\nУдаляем вопрос: {question.Text}");
            Console.Write("Вы уверены? (да/нет): ");
            
            if (Console.ReadLine().ToLower().StartsWith("да"))
            {
                quiz.Questions.RemoveAt(questionIndex - 1);
                Console.WriteLine("✅ Вопрос успешно удален!");
            }
            else
            {
                Console.WriteLine("Удаление отменено.");
            }
        }

        private void DeleteCategory()
        {
            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine("УДАЛЕНИЕ КАТЕГОРИИ");
            Console.WriteLine(new string('-', 40));
            
            if (quizzes.Count == 0)
            {
                Console.WriteLine("❌ Нет доступных категорий!");
                return;
            }

            ShowCategories();
            
            Console.Write("\nВведите название категории для удаления: ");
            string category = Console.ReadLine();
            var quiz = quizzes.FirstOrDefault(q => q.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            
            if (quiz == null)
            {
                Console.WriteLine("❌ Категория не найдена!");
                return;
            }

            Console.WriteLine($"\nУдаляем категорию: {category}");
            Console.WriteLine($"В ней {quiz.Questions.Count} вопросов и {quiz.Results.Count} результатов");
            Console.Write("Вы уверены? (да/нет): ");
            
            if (Console.ReadLine().ToLower().StartsWith("да"))
            {
                quizzes.Remove(quiz);
                Console.WriteLine("✅ Категория успешно удалена!");
            }
            else
            {
                Console.WriteLine("Удаление отменено.");
            }
        }

        private void ShowStatistics()
        {
            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine("СТАТИСТИКА ВИКТОРИН");
            Console.WriteLine(new string('-', 40));
            
            if (quizzes.Count == 0)
            {
                Console.WriteLine("Нет доступных викторин.");
                return;
            }

            int totalQuestions = quizzes.Sum(q => q.Questions.Count);
            int totalResults = quizzes.Sum(q => q.Results.Count);
            
            Console.WriteLine($"Всего категорий: {quizzes.Count}");
            Console.WriteLine($"Всего вопросов: {totalQuestions}");
            Console.WriteLine($"Всего результатов: {totalResults}");
            
            Console.WriteLine("\nДетальная статистика по категориям:");
            foreach (var quiz in quizzes.OrderByDescending(q => q.Questions.Count))
            {
                Console.WriteLine($"• {quiz.Category}: {quiz.Questions.Count} вопросов, {quiz.Results.Count} результатов");
            }
        }

        private void SaveChanges()
        {
            try
            {
                Database.SaveQuizzes(quizzes);
                Console.WriteLine("✅ Все изменения успешно сохранены!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при сохранении: {ex.Message}");
            }
        }
    }
}
