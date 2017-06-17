using System;
using Microsoft.EntityFrameworkCore;
using ForumAPI.Models;
using ForumAPI.Utilities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace ForumAPI.Data
{
    /// <summary>
    /// Interface for getting and storing information from the database.
    /// </summary>
    public class ForumContext : DbContext, IForumContext
    {
        public ForumContext(DbContextOptions<ForumContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Thread>()
                .HasOne(p => p.Author)
                .WithMany(p => p.Threads)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Owner)
                .WithMany(p => p.Posts)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(p => p.Posts)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<TopicRelation> TopicRelations { get; set; }

        public bool AddUser(User user, string password)
        {
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

        public User GetUser(string username)
        {
            return Users.FirstOrDefault(t => t.Name.ToUpper() == username.ToUpper());
        }

        public IEnumerable<User> GetAllUsers()
        {
            return Users;
        }

        // A User's profile should never be removed, merely deactivated.
        // Only used in order to update a user.
        public User RemoveUser(User user)
        {
            Users.Remove(user);
            return user;
        }

        public bool UpdateUser(User user, string password, UserStatus requestPermission)
        {
            User profile = GetUser(user.ID);
            if (profile == null)
            {
                return false;
            }

            if (password != "")
            {
                DataHandler.PopulatePasswordData(profile, password);
            }

            profile.Name = user.Name;
            profile.Email = user.Email;
            profile.HasSignature = user.HasSignature;
            profile.Signature = user.Signature;

            return true;
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

        public bool AddTopicRelation(TopicRelation relation)
        {
            TopicRelations.Add(relation);
            return true;
        }
    }
}
