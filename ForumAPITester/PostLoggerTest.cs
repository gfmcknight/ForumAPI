using ForumAPI.Controllers;
using ForumAPI.Models;
using ForumAPITester.Dummies;
using System;
using Xunit;
using Xunit.Abstractions;

namespace ForumAPITester
{
    class PostLoggerTest
    {
        private readonly ITestOutputHelper output;
        private UsersController controller;

        public PostLoggerTest(ITestOutputHelper output)
        {
            this.output = output;
            ActionLoggerDummy dummy = new ActionLoggerDummy(output);
            controller = new UsersController(dummy, dummy);
        }

        [Fact]
        public void TestGet()
        {

        }
    }
}
