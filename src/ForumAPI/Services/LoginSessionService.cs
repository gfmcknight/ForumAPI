using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumAPI.Models;
using ForumAPI.Utilities;

namespace ForumAPI.Services
{
    /// <summary>
    /// A service that keeps track of users that are logged in to the
    /// API.
    /// </summary>
    public class LoginSessionService
    {
        private static readonly long TimeoutTime = (long)10000000 * 3600 * 3;

        // TODO: Make this a concurrent dictionary
        private Dictionary<string, LoginSession> userLogins;

        public LoginSessionService()
        {
            userLogins = new Dictionary<string, LoginSession>();
        }

        /// <summary>
        /// Creates a login session that gets remembered instead of having to
        /// send login credentials with every action.
        /// </summary>
        /// <param name="user">The user being logged in</param>
        /// <returns>The token to be sent with requests that require logins
        /// in order to identify themselves.</returns>
        public string AddLogin(User user)
        {
            LoginSession login = new LoginSession
            {
                User = user,
                UserID = user.ID,
                LastAction = new DateTime(DateTime.Now.Ticks),
                Token = DataHandler.GenerateSalt(12)
            };

            userLogins.Add(login.Token, login);

            return login.Token;
        }

        /// <summary>
        /// Removes logins from the service which have been inactive for more
        /// than 3 hours.
        /// </summary>
        public void ClearTimedOut()
        {
            List<LoginSession> logins = userLogins.Values.ToList();
            foreach (LoginSession login in logins)
            {
                TimeSpan sinceLastAction = 
                    new TimeSpan(DateTime.Now.Ticks - login.LastAction.Ticks);
                if (sinceLastAction.Hours >= 3)
                {
                    userLogins.Remove(login.Token);
                }
            }
        }

        /// <summary>
        /// Gets the user associated with a certain token. If there has been
        /// more than 3 since the session was last active, the session will
        /// not be found.
        /// </summary>
        /// <param name="token">The string token associated with the login
        /// session.</param>
        /// <param name="error">The error message which will be sent if
        /// the login is not found, or the session has timed out.</param>
        /// <returns>The user if found, null otherwise.</returns>
        public User GetUser(string token, out string error)
        {
            error = "";
            LoginSession login;

            if (userLogins.TryGetValue(token, out login))
            {
                TimeSpan sinceLastAction = 
                    new TimeSpan(DateTime.Now.Ticks - login.LastAction.Ticks);
                if (sinceLastAction.Hours >= 3)
                {
                    error = Errors.SessionTimeout;
                    userLogins.Remove(login.Token);
                    return null;
                }
                else
                {
                    return login.User;
                }
            }
            else
            {
                error = Errors.SessionNotFound;
                return null;
            }
        }

        /// <summary>
        /// Removes a login session from the 
        /// </summary>
        /// <param name="token">The token associated with the login.</param>
        public void Logout(string token)
        {
            userLogins.Remove(token);
        }

        /// <summary>
        /// Updates a login with a more recent last action when a user performs
        /// an action using the session.
        /// </summary>
        /// <param name="token">The token associated with the login.</param>
        public void UpdateLoginAction(string token)
        {
            LoginSession login;
            if (userLogins.TryGetValue(token, out login))
            {
                login.LastAction = new DateTime(DateTime.Now.Ticks);
            }
        }
    }
}
