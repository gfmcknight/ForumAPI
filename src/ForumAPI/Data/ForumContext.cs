using System;
using Microsoft.EntityFrameworkCore;
using ForumAPI.Models;
using ForumAPI.Utilities;
using System.Security.Cryptography;

namespace ForumAPI.Data
{
    /// <summary>
    /// Interface for getting and storing information from the database.
    /// </summary>
    public class ForumContext : DbContext
    {
        public ForumContext(DbContextOptions<ForumContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Topic> Topics { get; set; }

        public bool addUser(User user, string password)
        {
            if (!DataHandler.IsValidEmail(user.Email))
            {
                return false;
            }
            
            foreach (User other in Users)
            {
                if (user.Name == other.Name)
                {
                    return false;
                }
            }

            user.Created = new DateTime(DateTime.Now.Ticks);
            user.PasswordProtocolVersion = 1;
            user.Salt = DataHandler.GenerateSalt(8);

            SHA256 sha = SHA256.Create();
            user.SHA256Password = DataHandler.CreateString(
                sha.ComputeHash(DataHandler.CreateByteArray(password + user.Salt)));

            user.Status = UserStatus.Active;

            return true;
        }
    }
}
