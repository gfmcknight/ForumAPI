﻿using System;
using Microsoft.EntityFrameworkCore;
using ForumAPI.Models;
using ForumAPI.Utilities;

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
        public DbSet<Topic> Topics { get; set; }

        public bool AddUser(User user, string password)
        {
            if (!DataHandler.IsValidEmail(user.Email))
            {
                return false;
            }

            foreach (User other in Users)
            {
                if (user.Name == other.Name || user.Email == other.Email)
                {
                    return false;
                }
            }

            user.Created = new DateTime(DateTime.Now.Ticks);
            DataHandler.PopulatePasswordData(user, password);

            user.Status = UserStatus.Active;

            Users.Add(user);

            return true;
        }

        public User GetUser(int id)
        {
            return Users.Find(id);
        }

        // A User's profile should never be removed, merely deactivated.
        // Only used in order to update a user.
        public User RemoveUser(User user)
        {
            Users.Remove(user);
            return user;
        }

        public bool UpdateUser(User user, string password)
        {
            User oldProfile = GetUser(user.ID);
            if (oldProfile == null)
            {
                return false;
            }

            user.Created = oldProfile.Created;
            user.Status = oldProfile.Status;
            if (password == "")
            {
                user.Salt = oldProfile.Salt;
                user.SHA256Password = oldProfile.SHA256Password;
                user.PasswordProtocolVersion = oldProfile.PasswordProtocolVersion;
            }
            else
            {
                DataHandler.PopulatePasswordData(user, password);
            }

            Remove(oldProfile);

            Users.Add(user);

            return true;
        }

        public void ChangeUserStatus(UserStatus status, int id)
        {
            User user = GetUser(id);
            if (user != null)
            {
                user.Status = status;
            }
            
        }

        public bool AddPost(Post post)
        {
            post.HasSignature = post.Author.HasSignature;
            post.Signature = post.Author.Signature;

            post.Created = new DateTime(DateTime.Now.Ticks);

            Posts.Add(post);


            return true;
        }

        public Post GetPost(int id)
        {
            return Posts.Find(id);
        }

        public Post RemovePost(Post post)
        {
            Posts.Remove(post);
            return post;
        }

        public bool UpdatePost(Post post)
        {
            Post oldPost = GetPost(post.ID);
            if (oldPost == null)
            {
                return false;
            }

            post.Created = oldPost.Created;

            return true;
        }

        public bool AddThread(Thread thread)
        {
            thread.Created = new DateTime();
            thread.Locked = false;

            Threads.Add(thread);

            return true;
        }

        public Thread GetThread(int id)
        {
            return Threads.Find(id);
        }

        // Note: deletes all posts in thread
        public Thread RemoveThread(Thread thread)
        {
            foreach (Post post in thread.Posts)
            {
                RemovePost(post);
            }

            Threads.Remove(thread);

            return thread;
        }

        public bool AddTopic(Topic topic)
        {
            Topics.Add(topic);
            return true;
        }

        public Topic GetTopic(int id)
        {
            return Topics.Find(id);
        }

        public Topic RemoveTopic(Topic topic)
        {
            Topics.Remove(topic);
            return topic;
        }
    }
}
