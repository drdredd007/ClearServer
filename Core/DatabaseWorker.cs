using ClearServer.Core.UserController;
using System;
using System.Data.Linq;
using System.Linq;

namespace ClearServer
{
    class DatabaseWorker
    {

        private readonly Table<User> users = null;
        private readonly DataContext DataBase = null;
        private const string connectionStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\drdre\source\repos\ClearServer\Users.mdf;Integrated Security=True";

        public DatabaseWorker()
        {
            DataBase = new DataContext(connectionStr);
            users = DataBase.GetTable<User>();
        }

        public User UserAuth(User User)
        {
            try
            {
                var user = users.SingleOrDefault(t => t.login.ToLower() == User.login.ToLower() && t.password == User.password);
                if (user != null)
                    return user;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public void UserRegister(User user)
        {
            try
            {
                users.InsertOnSubmit(user);
                DataBase.SubmitChanges();
                Console.WriteLine($"User{user.name} with id {user.uid} added");
                foreach (var item in users)
                {
                    Console.WriteLine(item.login + "\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        public bool LoginValidate(string login)
        {
            if (users.Any(x => x.login.ToLower() == login.ToLower()))
            {
                Console.WriteLine("Login already exists");
                return false;
            }
            return true;
        }
        public void UserUpdate(User user)
        {
            var UserToUpdate = users.FirstOrDefault(x => x.uid == user.uid);
            UserToUpdate = user;
            DataBase.SubmitChanges();
            Console.WriteLine($"User {UserToUpdate.name} with id {UserToUpdate.uid} updated");
            foreach (var item in users)
            {
                Console.WriteLine(item.login + "\n");
            }
        }
        public User CookieValidate(string CookieInput)
        {
            User user = null;
            try
            {
                user = users.SingleOrDefault(x => x.cookie == CookieInput);
            }
            catch
            {
                return null;
            }
            if (user != null) return user;
            else return null;
        }
        public User FindUser(string login)
        {
            User user = null;
            try
            {
                user = users.Single(x => x.login.ToLower() == login.ToLower());
                if (user != null)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
