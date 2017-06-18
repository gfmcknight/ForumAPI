using ForumAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumAPI.Data
{
    public interface IForumContext
    {
        bool AddUser(UserSubmission user);
        User GetUser(int id);
        User GetUser(string username);
        IEnumerable<User> GetAllUsers();
        User RemoveUser(User user);
        bool UpdateUser(int id, UserSubmission user, UserStatus requestPermission);
        bool AddPost(Post post);
        Post GetPost(int id);
        Post RemovePost(Post post);
        bool UpdatePost(Post post);
        bool AddThread(Thread thread);
        Thread GetThread(int id);
        Thread RemoveThread(Thread thread);
        bool AddTopic(Topic topic);
        Topic GetTopic(int id);
        Topic RemoveTopic(Topic topic);
        bool AddTopicRelation(TopicRelation relation);
        int SaveChanges();
    }
}
