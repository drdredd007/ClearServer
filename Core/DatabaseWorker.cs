using ClearServer.Core.WebSockets;
using ClearServerCore.Core.Database;
using ClearServerCore.Core.WebSockets.ChatController;
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
        public event Action<ChatMessage> OnMessageReceived;
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
            Console.WriteLine($"Database initialized \n Users in base {db.Users.Count()}");
        }

        public User UserAuth(User user)
        {
            try
            {
                return db.Users.First(t => t.login.ToLower() == user.login.ToLower() && t.password == user.password);
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
        public void MessageProcess(ChatMessage message, User sender)
        {
            var receiver = FindUser(message.to_User);
            var msg = new Message()
            {
                message = message.message,
                uid_From = sender.uid,
                uid_To = receiver.uid

            };
            db.Messages.Add(msg);
            db.SaveChanges();
            message = msg.ToChatMessage();
            OnMessageReceived?.Invoke(message);
        }

        public User CookieValidate(string CookieInput)
        {
            try
            {
                return db.Users.SingleOrDefault(x => x.cookie == CookieInput);
            }
            catch
            {
                return null;
            }
        }
        public User FindUser(string login)
        {
            try
            {
                return db.Users.Single(x => x.login.ToLower() == login.ToLower());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public User FindUser(int uid)
        {
            try
            {
                return db.Users.Single(x => x.uid == uid);
            }
            catch
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
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB; Database=MainDB; Trusted_Connection=true");
        }
    }
}
