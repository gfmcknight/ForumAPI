﻿using ForumAPI.Data;
using ForumAPI.Services;
using ForumAPI.Models;
using Xunit.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ForumAPI.Utilities;

namespace ForumAPITester.Dummies
{
    class ActionLoggerDummy : IForumContext, ILoginSessionService
    {
        public static User dummyUser = new User
        {
            ID = 1,
            Name = "Name",
            Email = "Email",
            Status = UserStatus.Active,
            SHA256Password = "Password",
            Salt = "Salt",
            PasswordProtocolVersion = -1,
            HasSignature = false
        };

        public static Topic dummyTopic = new Topic
        {
            ID = 1,
            AllowsThreads = true,
            Name = "Name",
            Description = "Desciption",
        };

        public static Thread dummyThread = new Thread
        {
            ID = 1,
            AuthorID = 1,
            Author = dummyUser,
            Locked = false,
            OwnerID = 1,
            Owner = dummyTopic,
            Title = "Title"
        };

        public static Post dummyPost = new Post
        {
            ID = 1,
            OwnerID = 1,
            Owner = dummyThread,
            AuthorID = 1,
            Author = dummyUser,
            Text = "Text",
            HasSignature = false            
        };

        private ITestOutputHelper output;

        public ActionLoggerDummy(ITestOutputHelper output)
        {
            this.output = output;
        }

        string ILoginSessionService.AddLogin(User user)
        {
            output.WriteLine("ILoginSessionService.AddLogin(User) called.");
            output.WriteLine(JsonConvert.SerializeObject(user));
            return "AAAAAA";
        }

        bool IForumContext.AddPost(Post post)
        {
            output.WriteLine("IForumContext.AddPost(Post) called.");
            output.WriteLine(JsonConvert.SerializeObject(post));
            return true;
        }

        bool IForumContext.AddThread(Thread thread)
        {
            output.WriteLine("IForumContext.AddThread(Thread) called.");
            output.WriteLine(JsonConvert.SerializeObject(thread));
            return true;
        }

        bool IForumContext.AddTopic(Topic topic)
        {
            output.WriteLine("IForumContext.AddTopic(Topic) called.");
            output.WriteLine(JsonConvert.SerializeObject(topic));
            return true;
        }

        bool IForumContext.AddTopicRelation(TopicRelation relation)
        {
            output.WriteLine("IForumContext.AddTopicRelation(TopicRelation) called.");
            output.WriteLine(JsonConvert.SerializeObject(relation));
            return true;
        }

        bool IForumContext.AddUser(User user, string password)
        {
            output.WriteLine("IForumContext.AddUser(User) called.");
            output.WriteLine(JsonConvert.SerializeObject(user));
            output.WriteLine(password);
            return true;
        }

        void ILoginSessionService.ClearTimedOut()
        {
            output.WriteLine("ILoginSessionService.ClearTimedOut() called.");
        }

        IEnumerable<User> IForumContext.GetAllUsers()
        {
            output.WriteLine("IForumContext.GetAllUsers() called.");
            LinkedList<User> users = new LinkedList<User>();
            users.AddLast(dummyUser);
            return users;
        }

        Post IForumContext.GetPost(int id)
        {
            output.WriteLine("IForumContext.GetPost(int) called.");
            output.WriteLine(id.ToString());
            return dummyPost;
        }

        Thread IForumContext.GetThread(int id)
        {
            output.WriteLine("IForumContext.GetThread(int) called.");
            output.WriteLine(id.ToString());
            return dummyThread;
        }

        Topic IForumContext.GetTopic(int id)
        {
            output.WriteLine("IForumContext.GetTopic(int) called.");
            output.WriteLine(id.ToString());
            return dummyTopic;
        }

        User IForumContext.GetUser(int id)
        {
            output.WriteLine("IForumContext.GetUser(int) called.");
            output.WriteLine(id.ToString());
            return dummyUser;
        }

        User IForumContext.GetUser(string username)
        {
            output.WriteLine("IForumContext.GetUser(string) called.");
            output.WriteLine(username);
            return dummyUser;
        }

        User ILoginSessionService.GetUser(string token, out string error)
        {
            output.WriteLine("ILoginSessionService.GetUser(string, out string) called.");
            output.WriteLine(token + " " + "[error]");
            error = "";
            return dummyUser;
        }

        void ILoginSessionService.Logout(string token)
        {
            output.WriteLine("ILoginSessionService.Logout(string) called.");
            output.WriteLine(token);
        }

        Post IForumContext.RemovePost(Post post)
        {
            output.WriteLine("IForumContext.RemovePost(Post) called.");
            output.WriteLine(JsonConvert.SerializeObject(post));
            return post;
        }

        Thread IForumContext.RemoveThread(Thread thread)
        {
            output.WriteLine("IForumContext.RemoveThread(Thread) called.");
            output.WriteLine(JsonConvert.SerializeObject(thread));
            return thread;
        }

        Topic IForumContext.RemoveTopic(Topic topic)
        {
            output.WriteLine("IForumContext.RemoveTopic(Topic) called.");
            output.WriteLine(JsonConvert.SerializeObject(topic));
            return topic;
        }

        User IForumContext.RemoveUser(User user)
        {
            output.WriteLine("IForumContext.RemoveUser(User) called.");
            output.WriteLine(JsonConvert.SerializeObject(user));
            return user;
        }

        int IForumContext.SaveChanges()
        {
            output.WriteLine("IForumContext.SaveChanges() called.");
            return 0;
        }

        void ILoginSessionService.UpdateLoginAction(string token)
        {
            output.WriteLine("ILoginSessionService.UpdateLoginAction(string token) called.");
            output.WriteLine(token);
        }

        bool IForumContext.UpdatePost(Post post)
        {
            output.WriteLine("IForumContext.UpdatePost(Post) called.");
            output.WriteLine(JsonConvert.SerializeObject(post));
            return true;
        }

        bool IForumContext.UpdateUser(User user, string password, UserStatus requestPermission)
        {
            output.WriteLine("IForumContext.UpdateUser(User, string, UserStatus) called.");
            output.WriteLine(JsonConvert.SerializeObject(user));
            output.WriteLine(password + " " + requestPermission.ToString());
            return true;
        }
    }
}
