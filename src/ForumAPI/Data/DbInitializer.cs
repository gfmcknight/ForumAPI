using ForumAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Data
{
    /// <summary>
    /// Class that defines the databse initialization behanvior, populating it
    /// with the administrator and root board.
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Creates the database, adds the admin and root board.
        /// </summary>
        /// <param name="context">The database to populate.</param>
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

            // This password only exists momentarily when the system is being created.
            var master = new UserSubmission { Username = "Graham",
                                    Email = "graham.mcknight2@gmail.com",
                                    HasSignature = false,
                                    Password = "DefaultPassword123" };

            context.AddTopic(mainBoard);

            int id = context.AddUser(master).ID;
            master.Status = UserStatus.Administrator;
            context.UpdateUser(id, master, UserStatus.Administrator);

            context.SaveChanges();
        }
    }
}
