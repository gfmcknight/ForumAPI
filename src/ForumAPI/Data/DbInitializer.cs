using ForumAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize (ForumContext  context)
        {
            context.Database.EnsureCreated();

            if (context.Topics.Any())
            {
                return;
            }

            var mainBoard = new Topic
            {
                Name = "Index",
                AllowsThreads = false,
                Description = "This is the root board"
            };

            var master = new UserSubmission { Username = "Graham",
                                    Email = "graham.mcknight2@gmail.com",
                                    HasSignature = false,
                                    Password = "DefaultPassword123" };

            context.AddTopic(mainBoard);
            // This password only exists momentarily when the system is being created.
            int id = context.AddUser(master).ID;
            master.Status = UserStatus.Administrator;
            context.UpdateUser(id, master, UserStatus.Administrator);

            context.SaveChanges();
        }
    }
}
