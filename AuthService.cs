using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp_EXAM
{
    public class AuthService
    {
        private List<User> users;

        public AuthService()
        {
            users = Database.LoadUsers();
        }

        public User Login(string login, string password)
        {
            return users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public User Register(string login, string password, DateTime birthDate)
        {
            if (users.Any(u => u.Login == login))
            {
                Console.WriteLine("Такой логин уже существует!");
                return null;
            }

            var user = new User { Login = login, Password = password, BirthDate = birthDate };
            users.Add(user);
            Database.SaveUsers(users);
            return user;
        }

        public List<User> GetAllUsers()
        {
            return users;
        }
    }
}
