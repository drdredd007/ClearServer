using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClearServerCore.Core.Database
{

    public class User
    {
        [Key]
        public int uid { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public DateTime dateOfBirth { get; set; }
        public DateTime registerDate { get; set; }
        public string skills { get; set; }
        public string cookie { get; set; }
        public string picture { get; set; }

        public User()
        {
            this.registerDate = DateTime.UtcNow;
        }

    }


    public class Message
    {
        [Key]
        public int mid { get; set; }
        public int uid_From { get; set; }
        public int uid_To { get; set; }        
        public string message { get; set; }
        public DateTime timeStamp { get; set; }

        public Message()
        {
            this.timeStamp = DateTime.UtcNow;
        }
    }
}
