using ForumAPI.Controllers;
using ForumAPITester.Dummies;
using System;
using Xunit;
using Xunit.Abstractions;

namespace ForumAPITester
{
    public class BoardLoggerTest
    {
        private readonly ITestOutputHelper output;
        private BoardController controller;

        public BoardLoggerTest(ITestOutputHelper output)
        {
            this.output = output;
            ActionLoggerDummy dummy = new ActionLoggerDummy(output);
            controller = new BoardController(dummy, dummy);
        }

        [Fact]
        public void TestGetRoot()
        {
            controller.GetRoot();
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
            controller.Post("Hello", ActionLoggerDummy.dummyTopic);
        }

        [Fact]
        public void TestDelete()
        {
            controller.Delete(1, "Hello");
        }
    }
}
