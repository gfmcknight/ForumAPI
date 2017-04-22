using ForumAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Services
{
    public interface ILoginSessionService
    {
        string AddLogin(User user);
        void ClearTimedOut();
        User GetUser(string token, out string error);
        void Logout(string token);
        void UpdateLoginAction(string token);
    }
}
