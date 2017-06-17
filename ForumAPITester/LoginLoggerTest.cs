using ForumAPI.Controllers;
using ForumAPI.Utilities;
using ForumAPITester.Dummies;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ForumAPITester
{
    public class LoginLoggerTest
    {
        private readonly ITestOutputHelper output;
        private LoginController controller;

        public LoginLoggerTest(ITestOutputHelper output)
        {
            this.output = output;
            ActionLoggerDummy dummy = new ActionLoggerDummy(output);
            controller = new LoginController(dummy, dummy);
        }

        [Fact]
        public void TestLogin()
        {
            controller.Login("Wrong", "Wrong");
            controller.Login(ActionLoggerDummy.dummyUser.Name, "Wrong");
            DataHandler.PopulatePasswordData(ActionLoggerDummy.dummyUser, "Password");
            output.WriteLine(controller.Login(ActionLoggerDummy.dummyUser.Name, "Password"));
        }
    }
}
