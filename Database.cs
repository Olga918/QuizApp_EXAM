using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace QuizApp_EXAM
{
    public static class Database
    {
        private static string usersFile = "user.json";
        private static string quizzesFile = "questions.json";

        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static string FindFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                return fileName;
            }
            
            string currentDir = Directory.GetCurrentDirectory();
            string projectRoot = currentDir;
            
            while (!string.IsNullOrEmpty(projectRoot))
            {
                if (File.Exists(Path.Combine(projectRoot, "QuizApp_EXAM.csproj")))
                {
                    break;
                }
                projectRoot = Directory.GetParent(projectRoot)?.FullName;
            }
            
            if (!string.IsNullOrEmpty(projectRoot))
            {
                string fileInProjectRoot = Path.Combine(projectRoot, fileName);
                if (File.Exists(fileInProjectRoot))
                {
                    return fileInProjectRoot;
                }
            }
            
            string parentDir = Directory.GetParent(currentDir)?.FullName;
            if (!string.IsNullOrEmpty(parentDir))
            {
                string fileInParent = Path.Combine(parentDir, fileName);
                if (File.Exists(fileInParent))
                {
                    return fileInParent;
                }
            }
            
            return fileName;
        }

        public static List<User> LoadUsers()
        {
            try
            {
                string filePath = FindFile(usersFile);
                
                if (!File.Exists(filePath)) 
                {
                    var defaultUsers = new List<User>
                    {
                        new User { Login = "admin", Password = "admin123", BirthDate = new DateTime(1990, 1, 1), Results = new List<Result>() },
                        new User { Login = "TestUser", Password = "1234", BirthDate = new DateTime(2000, 1, 1), Results = new List<Result>() }
                    };
                    SaveUsers(defaultUsers);
                    return defaultUsers;
                }
                
                string jsonContent = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<User>>(jsonContent, jsonOptions) ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки пользователей: {ex.Message}");
                return new List<User>();
            }
        }

        public static void SaveUsers(List<User> users)
        {
            try
            {
                string jsonContent = JsonSerializer.Serialize(users, jsonOptions);
                File.WriteAllText(usersFile, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения пользователей: {ex.Message}");
            }
        }

        public static List<Quiz> LoadQuizzes()
        {
            try
            {
                string filePath = FindFile(quizzesFile);
                
                if (!File.Exists(filePath)) 
                {
                    return new List<Quiz>();
                }
                
                string jsonContent = File.ReadAllText(filePath);
                var quizzes = JsonSerializer.Deserialize<List<Quiz>>(jsonContent, jsonOptions) ?? new List<Quiz>();
                return quizzes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки викторин: {ex.Message}");
                return new List<Quiz>();
            }
        }

        public static void SaveQuizzes(List<Quiz> quizzes)
        {
            try
            {
                string jsonContent = JsonSerializer.Serialize(quizzes, jsonOptions);
                File.WriteAllText(quizzesFile, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения викторин: {ex.Message}");
            }
        }
    }
}
