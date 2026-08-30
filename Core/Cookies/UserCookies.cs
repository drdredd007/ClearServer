using System;
using System.Security.Cryptography;

namespace ClearServer.Core.Cookies
{
    public class UserCookies
    {
        private const int TokenSize = 32;

        public string AuthCookie { get; }

        public UserCookies()
        {
            byte[] token = new byte[TokenSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(token);
            }
            string urlSafeToken = Convert.ToBase64String(token)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
            AuthCookie = "User=" + urlSafeToken;
        }
    }
}
