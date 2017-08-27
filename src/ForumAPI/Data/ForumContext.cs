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

            modelBuilder.Entity<TopicRelation>()
                .HasOne(p => p.Parent)
                .WithMany(p => p.SubTopics)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TopicRelation>()
                .HasOne(p => p.Child)
                .WithMany(p => p.Parents)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<TopicRelation> TopicRelations { get; set; }

        public User AddUser(UserSubmission user)
        {
            User profile = new User
            {
                Name = user.Username,
                Email = user.Email,
                Status = user.Status,
                HasSignature = user.HasSignature,
                Signature = user.Signature,
                Created = new DateTime(DateTime.Now.Ticks)
            };
            DataHandler.PopulatePasswordData(profile, user.Password);

            profile.Status = UserStatus.Active;

            Users.Add(profile);

            return profile;
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
        
        public User RemoveUser(User user)
        {
            // A User's profile should never be removed, merely deactivated.
            // Only used in order to update a user.
            Users.Remove(user);
            return user;
        }

        public bool UpdateUser(int id, UserSubmission user, UserStatus requestPermission)
        {
            User profile = GetUser(id);
            if (profile == null)
            {
                return false;
            }

            if (user.Password != null && user.Password != "")
            {
                DataHandler.PopulatePasswordData(profile, user.Password);
            }

            profile.Name = user.Username;
            profile.HasSignature = user.HasSignature;
            profile.Signature = user.Signature;

            if (requestPermission >= user.Status)
            {
                profile.Status = user.Status;
            }

            return true;
        }

        public bool AddPost(Post post)
        {
            post.HasSignature = GetUser(post.AuthorID).HasSignature;
            post.Signature = GetUser(post.AuthorID).Signature;

            post.Created = new DateTime(DateTime.Now.Ticks);

            Posts.Add(post);

            return true;
        }

        public Post GetPost(int id)
        {
            return Posts.Find(id);
        }

        public ICollection<Post> GetPosts(Thread thread)
        {
            return Posts.Where(p => p.Owner == thread)
                .Include(p => p.Author).ToList();
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
            thread.Created = new DateTime(DateTime.Now.Ticks);
            thread.Locked = false;

            Threads.Add(thread);

            return true;
        }

        public Thread GetThread(int id)
        {
            return Threads.Include(t => t.Author)
                .FirstOrDefault(t => t.ID == id);
        }
        
        public ICollection<Thread> GetThreads(Topic topic)
        {
            return Threads.Where(t => t.Owner == topic)
                .Include(p => p.Author).ToList();
        }

        public Thread RemoveThread(Thread thread)
        {
            // Note: deletes all posts in thread
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

        public ICollection<TopicRelation> GetSubtopics(Topic topic)
        {
            ICollection<TopicRelation> subTopics =
                TopicRelations.Include(t => t.Child)
                              .Where(t => t.Parent.ID == topic.ID).ToList();
            return subTopics;
        }

        public bool AddTopicRelation(TopicRelation relation)
        {
            TopicRelations.Add(relation);
            return true;
        }
    }
}
