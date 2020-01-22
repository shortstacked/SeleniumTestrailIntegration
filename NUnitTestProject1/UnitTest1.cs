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
        private static readonly string Url = Environment.GetEnvironmentVariable("TESTRAIL_URL");
        private static readonly string User = Environment.GetEnvironmentVariable("TESTRAIL_USER");
        private static readonly string Password = Environment.GetEnvironmentVariable("TESTRAIL_PASSWORD");
        private ulong _projectId = 2;
        private ulong _runId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Client = new TestRailClient(Url, User, Password);
            var commandResult = Client.AddRun(_projectId, 2, "Selenium Test Run " + DateTime.UtcNow.Ticks, "Selenium Test Run example", 1);
            Console.WriteLine(commandResult.Value);
            _runId = commandResult.Value;
        }

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--disable-dev-shm-usage");
            WebDriver = new ChromeDriver(options);

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