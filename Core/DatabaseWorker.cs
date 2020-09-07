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
        private DbSet<User> _users = null;
        private DbSet<Message> _messages = null;
        public DatabaseWorker()
        {
            db = new DatabaseContext();
            _users = db.Users;
            _messages = db.Messages;
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
                db.Users.Update(user);
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
            if (_users.Any(x => x.login.ToLower() == login.ToLower()))
            {
                Console.WriteLine("Login already exists");
                return false;
            }
            return true;
        }
        public void UserUpdate(User user)
        {
            var userToUpdate = _users.FirstOrDefault(x => x.uid == user.uid);
            userToUpdate = user;
            db.SaveChanges();
            Console.WriteLine($"User {userToUpdate.name} with id {userToUpdate.uid} updated");
            foreach (var item in _users)
            {
                Console.WriteLine(item.login + "\n");
            }
        }
        public void Testing()
        {
            User test1 = new User { name = "Test1", login = "tes1" };
            User test2 = new User { name = "Test2", login = "test" };
            _users.Add(test1);
            _users.Add(test2);
            db.SaveChanges();
            foreach (var item in _users.ToList())
            {
                Console.WriteLine(item.login + "\n");
            }
        }
        public User CookieValidate(string CookieInput)
        {
            User user = null;
            try
            {
                user = _users.SingleOrDefault(x => x.cookie == CookieInput);
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
                user = _users.Single(x => x.login.ToLower() == login.ToLower());
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
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB; Database=TestingBase; Trusted_Connection=true");
        }
    }
}
