using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Utilities
{
    /// <summary>
    /// Defines strings to return when specifying what went wrong to the user.
    /// </summary>
    public static class Errors
    {
        public static readonly string NoSuchElement = "ItemNotFound";
        public static readonly string InvalidEmail = "InvalidEmail";
        public static readonly string MissingFields = "ItemNotComplete";
        public static readonly string SessionTimeout = "LoginTimeout";
        public static readonly string SessionNotFound = "LoginTimeout";
        public static readonly string AlreadyExists = "UserAlreadyExists";
        public static readonly string WeakPassword = "PasswordTooShort";
    }
}
