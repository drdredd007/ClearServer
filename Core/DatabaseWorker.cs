using System;
using System.Data.Linq;
using System.Linq;

namespace ClearServer
{
    class DatabaseWorker
    {

        private Table<User> users;
        string connectionStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\drdre\source\repos\ClearServer\Users.mdf;Integrated Security=True";

        public User UserAuth(User User)
        {
            using (DataContext db = new DataContext(connectionStr))
            {
                users = db.GetTable<User>();
                try
                {
                    var user = users.SingleOrDefault(t => t.login == User.login && t.password == User.password);
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

        }

        public void UserRegister(User user)
        {
            using (DataContext db = new DataContext(connectionStr))
            {
                var table = db.GetTable<User>();
                table.InsertOnSubmit(user);
                db.SubmitChanges();
                Console.WriteLine($"User{user.name} with id {user.uid} added");
                foreach (var item in table)
                {
                    Console.WriteLine(item.login + "\n");
                }
            }
        }

        public bool LoginValidate(string login)
        {
            using (DataContext db = new DataContext(connectionStr))
            {
                users = db.GetTable<User>();
                if (users.Any(x => x.login == login))
                {
                    Console.WriteLine("Login already exists");
                    return false;
                }
                return true;
            }
        }
    }
}
