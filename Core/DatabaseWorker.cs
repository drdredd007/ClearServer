using ClearServer.Core.UserController;
using ClearServerCore.Core.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Sql;
using System.Linq;

namespace ClearServer
{
    class DatabaseWorker
    {
        private readonly DatabaseContext db = null;
        private static DatabaseWorker _instance = null;

        public static DatabaseWorker GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseWorker();
            }
            return _instance;
        }


        private DatabaseWorker()
        {
            db = new DatabaseContext();
            Console.WriteLine($"Database intialized \n Users in base {db.Users.Count()}");
        }

        public User UserAuth(User User)
        {
            try
            {
                var user = db.Users.First(t => t.login.ToLower() == User.login.ToLower() && t.password == User.password);
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
                db.Users.Add(user);
                db.SaveChanges();
                Console.WriteLine($"User{user.name} with id {user.uid} added");
                foreach (var item in db.Users)
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
            if (db.Users.Any(x => x.login.ToLower() == login.ToLower()))
            {
                Console.WriteLine("Login already exists");
                return false;
            }
            return true;
        }
        public void UserUpdate(User user)
        {
            var userToUpdate = db.Users.FirstOrDefault(x => x.uid == user.uid);
            userToUpdate = user;
            db.Users.Update(userToUpdate);
            db.SaveChanges();
            Console.WriteLine($"User {userToUpdate.name} with id {userToUpdate.uid} updated");
        }
        public void Testing()
        {
        }
        public User CookieValidate(string CookieInput)
        {
            User user = null;
            try
            {
                user = db.Users.SingleOrDefault(x => x.cookie == CookieInput);
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
                user = db.Users.Single(x => x.login.ToLower() == login.ToLower());
                if (user != null)
                {
                    Console.WriteLine($"User: {user.login} finded");
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }

    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB; Database=MainDB; Trusted_Connection=true");
        }
    }
}
