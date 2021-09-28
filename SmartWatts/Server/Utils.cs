using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace SmartWatts.Server
{
    public static class Utils
    {
        public static string HashPassword(string password)
        {
            // this is temporary, add salt. determine if this should be done in client, or server, or both.
            SHA1CryptoServiceProvider sha1 = new();

            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            byte[] encryptedBytes = sha1.ComputeHash(passwordBytes);

            return Convert.ToBase64String(encryptedBytes);
        }
    }
}
