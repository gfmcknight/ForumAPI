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
        public static readonly string NoSuchElement = "The item you are looking for was not found.";
        public static readonly string InvalidEmail = "The email entered is not a valid email.";
        public static readonly string MissingFields = "Please enter all fields in the form.";
        public static readonly string SessionTimeout = "Your login session has timed out.";
        public static readonly string SessionNotFound = "Your login session has timed out.";
        public static readonly string AlreadyExists = "A user with that username or email already exists.";
        public static readonly string WeakPassword = "Your password be at least 8 characters long.";
    }
}
