using CoreFramework.Helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace CoreFramework.Utils
{
    public class CoreBaseTest
    {
        protected IWebDriver driver;
        [SetUp]
        public void SetupBeforeEverySingleTest()
        {
            var driverConfig = JsonConfigProvider.WebDriver;
            driver = new WebDriverFactory().GetWebDriver(driverConfig);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait
                    = TimeSpan.FromSeconds(JsonConfigProvider.WebDriver.DefaultTimeout);
        }

        [TearDown]
        public void CleanUpAfterEverySingleTest()
        {
            driver.Quit();
        }
    }
}
