using ForumAPI.Controllers;
using ForumAPI.Models;
using ForumAPITester.Dummies;
using System;
using Xunit;
using Xunit.Abstractions;

namespace ForumAPITester
{
    public class UserLoggerTest
    {
        private readonly ITestOutputHelper output;
        private UsersController controller;

        public UserLoggerTest(ITestOutputHelper output)
        {
            this.output = output;
            ActionLoggerDummy dummy = new ActionLoggerDummy(output);
            controller = new UsersController(dummy, dummy);
        }

        [Fact]
        public void TestGet()
        {
            controller.Get(1);
            controller.Get(2);
            controller.Get(3);
        }

        [Fact]
        public void TestPost()
        {
            controller.Post(ActionLoggerDummy.dummyUser, ActionLoggerDummy.dummyUser.SHA256Password);
            controller.Post(new User
                {
                    Email = "my.email@gmail.com",
                    Name = "bigUserName",
                }, "Hello!");
            controller.Post(new User
            {
                Email = "my.email@gmail.com",
                Name = "bigUserName",
            }, "EHL$$5%k9!");
        }

        [Fact]
        public void TestPut()
        {
            controller.Put(ActionLoggerDummy.dummyUser, "", "token");

            controller.Put(new User
            {
                ID = 1,
                Name = "new name",
                Status = UserStatus.Active,
                HasSignature = true,
            }, "", "session");

            controller.Put(ActionLoggerDummy.dummyUser, "hello", "mysession");
        }
    }
}
