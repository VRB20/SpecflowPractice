using CoreFramework.Helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace CoreFramework.Utils
{
    public class CoreBaseTest
    {
        protected IWebDriver driver;
        private WebDriverFactory webDriverFactory = new WebDriverFactory();
        [SetUp]
        public void SetupBeforeEverySingleTest()
        {
            driver = webDriverFactory.GetWebDriver();
        }

        [TearDown]
        public void CleanUpAfterEverySingleTest()
        {
            webDriverFactory.CloseDriver();
        }
    }
}
