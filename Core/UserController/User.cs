using System.ComponentModel.DataAnnotations;

namespace ClearServer.Core.UserController
{
    public class User
    {
        [Key]
        public int uid { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string date { get; set; }
        public string skills { get; set; }
        public string cookie { get; set; }

    }
}