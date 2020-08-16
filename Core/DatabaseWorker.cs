using System;
using System.Data.Linq;
namespace ClearServer
{
    class DatabaseWorker
    {

        private static DatabaseWorker instance;

        public static DatabaseWorker GetInstance
        {
            get
            {
                if (instance == null)
                    instance = new DatabaseWorker();
                return instance;
            }
        }


        private DatabaseWorker()
        {
            string connectionStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\drdre\source\repos\ClearServer\Users.mdf;Integrated Security=True";
            using (DataContext db = new DataContext(connectionStr))
            {
                Table<User> users = db.GetTable<User>();
                foreach (var item in users)
                {
                    Console.WriteLine($"{item.login} {item.password}");
                }
            }
        }
    }
}
