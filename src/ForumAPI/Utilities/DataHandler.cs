using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumAPI.Utilities
{
    /// <summary>
    /// A helper class which helps with data validation and password hashing.
    /// </summary>
    public static class DataHandler
    {
        private static readonly string[] acceptedDomains =
        {
            "gmail.com", "hotmail.com", "yahoo.com", "msn.com", "live.com"
        };

        public static bool IsValidEmail(string email)
        {
            string[] parsedEmail = email.Split('@');

            if (parsedEmail.Length != 2)
            {
                return false;
            } else if (!acceptedDomains.Contains(parsedEmail[1]))
            {
                return false;
            }

            return true;
        }


        private static readonly string saltCharacters =
            "ABCDEFGHIJKLMOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*(){}[];:.<>?/";


        public static string GenerateSalt(int length)
        {
            StringBuilder salt = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                salt.Append(saltCharacters[random.Next(saltCharacters.Length)]);
            }

            return salt.ToString();
        }

        public static byte[] CreateByteArray(string input)
        {
            return Encoding.ASCII.GetBytes(input);
        }

        public static string CreateString(byte[] input)
        {
            return Encoding.ASCII.GetString(input);
        }
    }
}
