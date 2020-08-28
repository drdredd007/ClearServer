using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClearServer.Core.Security
{
    class PasswordHasher
    {
        public PasswordHasher()
        {

        }

        public string PasswordHash(string password)
        {
            using (MD5 hasher = MD5.Create())
            {
                byte[] hash = Encoding.UTF8.GetBytes(password);
                for (int i = 0; i < 5; i++)
                {
                    hash = hasher.ComputeHash(hash);
                }
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    stringBuilder.Append(hash[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }

        }
    }
}
