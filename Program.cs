namespace QuizApp_EXAM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine("=== ДОБРО ПОЖАЛОВАТЬ В ВИКТОРИНУ ===");
            Console.WriteLine("Проверьте свои знания в разных областях!\n");
            
            var auth = new AuthService();
            User currentUser = null;

            while (currentUser == null)
            {
                Console.WriteLine("1. Вход в систему");
                Console.WriteLine("2. Регистрация нового пользователя");
                Console.Write("\nВыберите действие: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    currentUser = LoginUser(auth);
                }
                else if (choice == "2")
                {
                    currentUser = RegisterUser(auth);
                }
                else
                {
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                }
            }

            Console.WriteLine($"\nДобро пожаловать, {currentUser.Login}! 🎉");
            
            var quizService = new QuizService();

            while (true)
            {
                ShowMainMenu();
                string cmd = Console.ReadLine();

                try
                {
                    switch (cmd)
                    {
                        case "1":
                            StartNewQuiz(currentUser);
                            break;
                        case "2":
                            ShowUserResults(currentUser);
                            break;
                        case "3":
                            ShowTopResults(quizService);
                            break;
                        case "4":
                            ManageSettings(auth, currentUser);
                            break;
                        case "5":
                            OpenQuizEditor();
                            break;
                        case "6":
                            Console.WriteLine("До свидания! 👋");
                            return;
                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        private static User LoginUser(AuthService auth)
        {
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();
            string password = ReadPassword("Введите пароль: ");
            
            var user = auth.Login(login, password);
            if (user == null)
            {
                Console.WriteLine("❌ Неверный логин или пароль!");
                return null;
            }
            
            Console.WriteLine("✅ Вход выполнен успешно!");
            return user;
        }

        private static User RegisterUser(AuthService auth)
        {
            Console.Write("Введите логин: ");
            string login = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(login))
            {
                Console.WriteLine("❌ Логин не может быть пустым!");
                return null;
            }

            string password = ReadPassword("Введите пароль: ");
            
            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("❌ Пароль не может быть пустым!");
                return null;
            }

            DateTime birthDate;
            while (true)
            {
                Console.Write("Введите дату рождения (гггг-мм-дд): ");
                if (DateTime.TryParse(Console.ReadLine(), out birthDate))
                {
                    if (birthDate > DateTime.Now)
                    {
                        Console.WriteLine("❌ Дата рождения не может быть в будущем!");
                        continue;
                    }
                    break;
                }
                Console.WriteLine("❌ Неверный формат даты. Используйте формат гггг-мм-дд");
            }

            var user = auth.Register(login, password, birthDate);
            if (user != null)
            {
                Console.WriteLine("✅ Регистрация выполнена успешно!");
            }
            return user;
        }

        private static string ReadPassword(string prompt = "Пароль: ")
        {
            var password = new System.Text.StringBuilder();
            Console.Write(prompt);
            
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Length--;
                    Console.Write("\b \b"); // Удаляем звездочку
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write("*"); // Показываем звездочку
                }
            }
            Console.WriteLine();
            return password.ToString();
        }

        private static void ShowMainMenu()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("ГЛАВНОЕ МЕНЮ ВИКТОРИНЫ");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("1. 🎯 Начать новую викторину");
            Console.WriteLine("2. 📊 Мои результаты");
            Console.WriteLine("3. 🏆 Топ-20 результатов");
            Console.WriteLine("4. ⚙️  Настройки профиля");
            Console.WriteLine("5. ✏️  Редактор викторин");
            Console.WriteLine("6. 🚪 Выход");
            Console.WriteLine(new string('=', 50));
            Console.Write("Выберите действие: ");
        }

        private static void StartNewQuiz(User user)
        {
            var quizService = new QuizService();
            var quizzes = quizService.GetQuizzes();
            
            if (quizzes.Count == 0)
            {
                Console.WriteLine("❌ Нет доступных викторин!");
                return;
            }

            Console.WriteLine("\n🎯 Доступные категории викторин:");
            Console.WriteLine(new string('=', 40));
            
            for (int i = 0; i < quizzes.Count; i++)
            {
                string categoryName = quizzes[i].Name ?? quizzes[i].Category ?? $"Категория {i + 1}";
                int questionCount = quizzes[i].Questions?.Count ?? 0;
                Console.WriteLine($"{i + 1}. {categoryName} ({questionCount} вопросов)");
            }
            Console.WriteLine($"{quizzes.Count + 1}. Смешанная (вопросы из всех категорий)");
            Console.WriteLine(new string('=', 40));

            Console.Write("\nВыберите категорию (введите номер): ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                string selectedCategory;
                if (choice == quizzes.Count + 1)
                {
                    selectedCategory = "Смешанная";
                }
                else if (choice > 0 && choice <= quizzes.Count)
                {
                    var selectedQuiz = quizzes[choice - 1];
                    selectedCategory = selectedQuiz.Name ?? selectedQuiz.Category ?? $"Категория {choice}";
                }
                else
                {
                    Console.WriteLine("❌ Неверный выбор!");
                    return;
                }

                quizService.StartQuiz(user, selectedCategory);
            }
            else
            {
                Console.WriteLine("❌ Неверный ввод!");
            }
        }

        private static void ShowUserResults(User user)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine($"РЕЗУЛЬТАТЫ ПОЛЬЗОВАТЕЛЯ {user.Login.ToUpper()}");
            Console.WriteLine(new string('=', 50));
            
            if (user.Results.Count == 0)
            {
                Console.WriteLine("У вас пока нет результатов. Пройдите викторину!");
                return;
            }

            var groupedResults = user.Results
                .GroupBy(r => r.QuizCategory)
                .OrderByDescending(g => g.Max(r => r.Score))
                .ToList();

            foreach (var group in groupedResults)
            {
                Console.WriteLine($"\n📚 {group.Key}:");
                var bestResult = group.OrderByDescending(r => r.Score).First();
                Console.WriteLine($"   Лучший результат: {bestResult.Score} баллов ({bestResult.Date:dd.MM.yyyy})");
                Console.WriteLine($"   Всего попыток: {group.Count()}");
            }
        }

        private static void ShowTopResults(QuizService quizService)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("ТОП-20 РЕЗУЛЬТАТОВ");
            Console.WriteLine(new string('=', 50));
            
            quizService.ShowAvailableCategories();
            
            Console.Write("\nВведите название категории: ");
            string category = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(category))
            {
                Console.WriteLine("❌ Категория не может быть пустой!");
                return;
            }

            quizService.ShowTop(category);
        }

        private static void ManageSettings(AuthService auth, User currentUser)
        {
            while (true)
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("НАСТРОЙКИ ПРОФИЛЯ");
                Console.WriteLine(new string('=', 50));
                Console.WriteLine("1. 🔒 Изменить пароль");
                Console.WriteLine("2. 📅 Изменить дату рождения");
                Console.WriteLine("3. 🔙 Вернуться в главное меню");
                Console.WriteLine(new string('=', 50));
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        ChangePassword(currentUser);
                        break;
                    case "2":
                        ChangeBirthDate(currentUser);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        private static void ChangePassword(User user)
        {
            string newPassword = ReadPassword("Введите новый пароль: ");
            
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine("❌ Пароль не может быть пустым!");
                return;
            }

            user.Password = newPassword;
            Database.SaveUsers(Database.LoadUsers());
            Console.WriteLine("✅ Пароль успешно изменен!");
        }

        private static void ChangeBirthDate(User user)
        {
            Console.Write("Введите новую дату рождения (гггг-мм-дд): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime newBirthDate))
            {
                if (newBirthDate > DateTime.Now)
                {
                    Console.WriteLine("❌ Дата рождения не может быть в будущем!");
                    return;
                }
                
                user.BirthDate = newBirthDate;
                Database.SaveUsers(Database.LoadUsers());
                Console.WriteLine("✅ Дата рождения успешно изменена!");
            }
            else
            {
                Console.WriteLine("❌ Неверный формат даты!");
            }
        }

        private static void OpenQuizEditor()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("РЕДАКТОР ВИКТОРИН");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Для доступа к редактору требуется авторизация администратора.");
            
            Console.Write("Введите логин администратора: ");
            string adminLogin = Console.ReadLine();
            string adminPassword = ReadPassword("Введите пароль администратора: ");
            
            // Отладочная информация
            Console.WriteLine($"Введенный логин: '{adminLogin}'");
            Console.WriteLine($"Введенный пароль: '{adminPassword}'");
            Console.WriteLine($"Ожидаемый логин: 'admin'");
            Console.WriteLine($"Ожидаемый пароль: 'admin123'");
            
            // Простая проверка на администратора
            if (adminLogin == "admin" && adminPassword == "admin123")
            {
                Console.WriteLine("✅ Доступ разрешен!");
                var editor = new QuizEditor();
                editor.OpenEditor();
            }
            else
            {
                Console.WriteLine("❌ Неверные данные администратора!");
                Console.WriteLine("Подсказка: логин 'admin', пароль 'admin123'");
            }
        }
    }
}
