using System;
using System.Data.Linq;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace ClearServer
{
    class DatabaseWorker
    {

        private Table<User> users;
        string connectionStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\drdre\source\repos\ClearServer\Users.mdf;Integrated Security=True";

        public void UserAuth(TcpClient client,string[] logPass)
        {
            using (DataContext db = new DataContext(connectionStr))
            {
                users = db.GetTable<User>();
                Console.WriteLine("Starting user auth check");
                var user = users.SingleOrDefault(t => t.login == logPass[0] && t.password == logPass[1]);
                if (user != null)
                {

                }
                else
                {
                }
            }            

        }
    }
}
