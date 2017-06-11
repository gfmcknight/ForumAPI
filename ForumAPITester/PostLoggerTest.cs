using ForumAPI.Controllers;
using ForumAPI.Models;
using ForumAPITester.Dummies;
using System;
using Xunit;
using Xunit.Abstractions;

namespace ForumAPITester
{
    public class PostLoggerTest
    {
        private readonly ITestOutputHelper output;
        private ThreadsController controller;

        public PostLoggerTest(ITestOutputHelper output)
        {
            this.output = output;
            ActionLoggerDummy dummy = new ActionLoggerDummy(output);
            controller = new ThreadsController(dummy, dummy);
        }

        [Fact]
        public void TestGet()
        {
            controller.Get(1);
            controller.Get(2);
            controller.GetPosts(1);
            controller.GetPosts(2);
        }

        [Fact]
        public void TestCreate()
        {
            controller.Create("session", ActionLoggerDummy.dummyThread);
        }
    }
}
