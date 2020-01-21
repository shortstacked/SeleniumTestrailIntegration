using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TestRail;
using TestRail.Types;

namespace NUnitTestProject1
{
    public class Tests
    {
        public static IWebDriver WebDriver;
        public static TestRailClient Client;
        private static string _url = "";
        private static string _user = "";
        private static string _password = "";
        private ulong _projectId = 2;
        private ulong _runId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Client = new TestRailClient(_url, _user, _password);
            var commandResult = Client.AddRun(_projectId, 2, "Selenium Test Run " + DateTime.UtcNow.Ticks, "Selenium Test Run example", 1);
            Console.WriteLine(commandResult.Value);
            _runId = commandResult.Value;
        }

        [SetUp]
        public void Setup()
        {
            WebDriver = new ChromeDriver();
        }

        [Test(Description = "C3")]
        public void CanNavigateToGoogle()
        {
            WebDriver.Navigate().GoToUrl("http://www.google.com");
            var title = WebDriver.Title;
            Assert.AreEqual("Google", title);
        }

        [TearDown]
        public void TearDown()
        {
            var id = TestContext.CurrentContext.Test.Properties.Get("Description").ToString().Replace("C","");
            var result = TestContext.CurrentContext.Result.Outcome.Status;
            var testrailStatus = result switch
            {
                TestStatus.Failed => ResultStatus.Failed,
                TestStatus.Passed => ResultStatus.Passed,
                _ => ResultStatus.Retest
            };

            var resultForCase = Client.AddResultForCase(_runId, ulong.Parse(id), testrailStatus);
            Console.WriteLine(resultForCase.WasSuccessful);
            WebDriver.Quit();
        }

    }
}