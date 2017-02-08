using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Utilities
{
    public static class DataVerifier
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
    }
}
