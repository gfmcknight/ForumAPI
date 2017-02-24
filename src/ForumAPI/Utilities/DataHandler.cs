using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
            if (email == null)
            {
                return false;
            }

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


        private static readonly string SaltCharacters =
            "ABCDEFGHIJKLMOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*(){}[];:.<>?/";


        public static string GenerateSalt(int length)
        {
            StringBuilder salt = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                salt.Append(SaltCharacters[random.Next(SaltCharacters.Length)]);
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

        public static void PopulatePasswordData(Models.User user, string password)
        {
            user.PasswordProtocolVersion = 1;
            user.Salt = GenerateSalt(8);

            SHA256 sha = SHA256.Create();
            user.SHA256Password = CreateString(
                sha.ComputeHash(DataHandler.CreateByteArray(password + user.Salt)));
        }

        private static readonly int BytesPerMeg = 1000000;
        private static readonly int MaxPictureSize = 22 * BytesPerMeg;

        public static bool ValidatePictureSize(byte[] picture)
        {
            return picture.Length < MaxPictureSize;
        }

        public static bool VerifyPassword(string password)
        {
            return password.Length > 8;
        }
    }
}
