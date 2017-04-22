using ForumAPI.Data;
using ForumAPI.Services;
using System;
using System.Collections.Generic;
using System.Text;
using ForumAPI.Models;
using Xunit;
using System.Diagnostics;

namespace ForumAPITester.Dummies
{
    class ActionLoggerDummy : IForumContext, ILoginSessionService
    {
        string ILoginSessionService.AddLogin(User user)
        {
            Debug.WriteLine("ILoginSessionService.AddLogin(User) called.");
            Debug.WriteLine(user);
            return "AAAAAA";
        }

        bool IForumContext.AddPost(Post post)
        {
            Debug.WriteLine("IForumContext.AddPost(Post) called.");
            Debug.WriteLine(post);
            return true;
        }

        bool IForumContext.AddThread(Thread thread)
        {
            Debug.WriteLine("IForumContext.AddThread(Thread) called.");
            Debug.WriteLine(thread);
            return true;
        }

        bool IForumContext.AddTopic(Topic topic)
        {
            Debug.WriteLine("IForumContext.AddTopic(Topic) called.");
            Debug.WriteLine(topic);
            return true;
        }

        bool IForumContext.AddTopicRelation(TopicRelation relation)
        {
            Debug.WriteLine("IForumContext.AddTopicRelation(TopicRelation) called.");
            Debug.WriteLine(relation);
            return true;
        }

        bool IForumContext.AddUser(User user, string password)
        {
            Debug.WriteLine("IForumContext.AddUser(User) called.");
            Debug.WriteLine(user);
            return true;
        }

        void ILoginSessionService.ClearTimedOut()
        {
            Debug.WriteLine("ILoginSessionService.ClearTimedOut() called.");
        }

        Post IForumContext.GetPost(int id)
        {
            throw new NotImplementedException();
        }

        Thread IForumContext.GetThread(int id)
        {
            throw new NotImplementedException();
        }

        Topic IForumContext.GetTopic(int id)
        {
            throw new NotImplementedException();
        }

        User IForumContext.GetUser(int id)
        {
            throw new NotImplementedException();
        }

        User IForumContext.GetUser(string username)
        {
            throw new NotImplementedException();
        }

        User ILoginSessionService.GetUser(string token, out string error)
        {
            throw new NotImplementedException();
        }

        void ILoginSessionService.Logout(string token)
        {
            throw new NotImplementedException();
        }

        Post IForumContext.RemovePost(Post post)
        {
            throw new NotImplementedException();
        }

        Thread IForumContext.RemoveThread(Thread thread)
        {
            throw new NotImplementedException();
        }

        Topic IForumContext.RemoveTopic(Topic topic)
        {
            throw new NotImplementedException();
        }

        User IForumContext.RemoveUser(User user)
        {
            throw new NotImplementedException();
        }

        void ILoginSessionService.UpdateLoginAction(string token)
        {
            throw new NotImplementedException();
        }

        bool IForumContext.UpdatePost(Post post)
        {
            throw new NotImplementedException();
        }

        bool IForumContext.UpdateUser(User user, string password, UserStatus requestPermission)
        {
            throw new NotImplementedException();
        }
    }
}
