using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ClearServer.Core.Cookies
{
    public static class UserCookies
    {
        private const string Seed = "UserAuth";

        public static Cookie AuthCookie(string login, string password)
        {
            
            var part1 = HashCookieParts(Seed);
            var part2 = HashCookieParts(login);
            var part3 = HashCookieParts(password);
            var tempCookie = new Cookie()
            {
                Name =  "User",
                Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(part2 + part1 + part3)),
                Expires = DateTime.Now.AddDays(2),
                HttpOnly = true,
                Secure = true
            };
            return tempCookie;
        }

        private static string HashCookieParts(string Part)
        {
            return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(Part)));
        }
    }
}