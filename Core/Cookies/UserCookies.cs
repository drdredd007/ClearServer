using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ClearServer.Core.Cookies
{
    public static class UserCookies
    {
        private const string Seed = "I1N6QVNmMTIhZGZzQHpEWGZxMkAkYXNkZmZAJCUhQERGQEAkUUFTREZBQTkwODEyM0FBQTEyM0FBZmYx";

        public static Cookie AuthCookie(string login, string password)
        {
            var parts = new string[] { password, Seed, login };
            var tempCookie = new Cookie()
            {
                Name =  "User",
                Value = HashCookieParts(parts),
                Expires = DateTime.Now.AddDays(2),
                HttpOnly = true,
                Secure = true
            };
            return tempCookie;
        }


        private static string HashCookieParts(string[] parts)
        {
            string hash = "";
            for (int i = 0; i < parts.Length; i++)
            {
                hash += Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(parts[i])));
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(hash));
        }
    }
}