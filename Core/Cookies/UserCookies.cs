using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClearServer.Core.Cookies
{
    public class UserCookies
    {
        private const string Seed = "UserAuth";
        private readonly string login;
        private readonly string password;

        public UserCookies(string Login, string Password)
        {
            login = Login;
            password = Password;
        }
        public string AuthCookie { get
            {
               var part1 = HashCookieParts(Seed);
               var part2 = HashCookieParts(login);
               var part3 = HashCookieParts(password);
                return "User="+part2+part1+part3;
            }
        }

        private static string HashCookieParts(string Part)
        {
            return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(Part)));
        }
    }
}
