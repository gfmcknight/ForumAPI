using ForumAPI.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

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

        /// <summary>
        /// Determines whether a string is correctly formatted to contain an
        /// email address.
        /// </summary>
        /// <param name="email">The string to parse.</param>
        /// <returns>True if the email is correctly formatted.</returns>
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

        /// <summary>
        /// Generates a random string of characters to transform a password.
        /// </summary>
        /// <param name="length">The number of characters to generate.</param>
        /// <returns>The random characters.</returns>
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

        /// <summary>
        /// Translates a string into an array of bytes.
        /// </summary>
        /// <param name="input">The string to translate.</param>
        /// <returns>The created array.</returns>
        public static byte[] CreateByteArray(string input)
        {
            return Encoding.ASCII.GetBytes(input);
        }

        /// <summary>
        /// Translates an array of bytes into a string.
        /// </summary>
        /// <param name="input">The array of bytes to translate.</param>
        /// <returns>The created string.</returns>
        public static string CreateString(byte[] input)
        {
            return Encoding.ASCII.GetString(input);
        }

        /// <summary>
        /// Takes a user model and inserts a hashed password, as well as
        /// password-related data.
        /// </summary>
        /// <param name="user">The user to populate with password info.</param>
        /// <param name="password">The user-entered password.</param>

        public static readonly int CurrentPasswordProtocolVersion = 2;

        public static void PopulatePasswordData(User user, string password)
        {
            user.PasswordProtocolVersion = CurrentPasswordProtocolVersion;
            user.Salt = GenerateSalt(8);

            user.SHA256Password = getHash(password + user.Salt, user.PasswordProtocolVersion);
        }

        /// <summary>
        /// Determines whether the password entered for a user is the same
        /// as the one the user model was populated with.
        /// </summary>
        /// <param name="rawPassword">The user-entered password.</param>
        /// <param name="user">The user to check for password correctness.</param>
        /// <returns>True if the password matches, false otherwise.</returns>
        public static bool IsCorrectPassword(string rawPassword, User user)
        {
            return (getHash(rawPassword + user.Salt, user.PasswordProtocolVersion) ==
                user.SHA256Password);
        }

        /// <summary>
        /// Computes the hash using SHA-256 for a given password/salt
        /// combination.
        /// </summary>
        /// <param name="input">The password/hash combination.</param>
        /// <param name="protocol">Used to support staging and updates
        /// of the API password protocol.</param>
        /// <returns>The SHA-256 hash of the password.</returns>
        private static string getHash(string input, int protocol)
        {
            SHA256 sha = SHA256.Create();
            switch (protocol) {

                case 1:
                    return CreateString(sha.ComputeHash(CreateByteArray(input)));
                case 2:
                    return Convert.ToBase64String(sha.ComputeHash(CreateByteArray(input)));
                default:
                    return "";
            }
        }

        /// <summary>
        /// Checks the criteria for a valid password. Currently, only password
        /// length.
        /// </summary>
        /// <param name="password">The suggested user password/</param>
        /// <returns>True if the password meets all criteria</returns>
        public static bool VerifyPassword(string password)
        {
            return password.Length > 8;
        }

        private static readonly JWTHeader header = new JWTHeader
        {
            Type = "jwt",
            Algorithm = "HMAC"
        };

        /// <summary>
        /// Creates a JWT Token for the given user id using the JWTHeader and JWTBody
        /// classes.
        /// </summary>
        /// <param name="id">The id of the user the </param>
        /// <returns>The token </returns>
        public static string EncodeJWT(int id, UserStatus status)
        {
            JWTBody body = new JWTBody
            {
                Issuer = AuthorizationInfo.Issuer,
                Audience = AuthorizationInfo.Audience,
                UserID = id,
                Status = status,
                Expires = DateTime.Now + AuthorizationInfo.TokenExpiryTime
            };

            string payload = JsonConvert.SerializeObject(body);

            string basis = Convert.ToBase64String(
                CreateByteArray(JsonConvert.SerializeObject(header))) + "." +
                Convert.ToBase64String(CreateByteArray(payload));

            HMACMD5 encoder = new HMACMD5(
                CreateByteArray(AuthorizationInfo.SecretKey));

            string signature = Convert.ToBase64String(
                encoder.ComputeHash(CreateByteArray(basis)));

            return basis + "." + signature;

        }

        /// <summary>
        /// Validates a JDW Token, and determines the user
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <returns>False if token is invalid.</returns>
        public static bool DecodeJWT(string token, out int id, out UserStatus status)
        {
            id = 0;
            status = UserStatus.Banned;
            string[] parts = token.Split(".".ToArray());

            if (parts.Length != 3)
            {
                return false;
            }

            HMACMD5 encoder = new HMACMD5(
                CreateByteArray(AuthorizationInfo.SecretKey));

            string signature = Convert.ToBase64String(encoder.ComputeHash(
                CreateByteArray(parts[0] + "." + parts[1])));

            if (signature != parts[2])
            {
                return false;
            }

            JWTHeader tokenHeader = JsonConvert.DeserializeObject<JWTHeader>(
                Encoding.ASCII.GetString(Convert.FromBase64String(parts[0])));
            if (header == null || !header.Equals(tokenHeader))
            {
                return false;
            }

            JWTBody tokenBody = JsonConvert.DeserializeObject<JWTBody>(
                Encoding.ASCII.GetString(Convert.FromBase64String(parts[1])));

            if (tokenBody == null || 
                tokenBody.Issuer != AuthorizationInfo.Issuer ||
                tokenBody.Audience != AuthorizationInfo.Audience ||
                tokenBody.Expires < DateTime.Now)
            {
                return false;
            }

            id = tokenBody.UserID;
            status = tokenBody.Status;
            return true;
        }
    }
}
