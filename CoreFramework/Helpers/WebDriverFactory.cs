using CoreFramework.Enums;
using CoreFramework.Extensions;
using CoreFramework.Models;
using CoreFramework.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace CoreFramework.Helpers
{
    public class WebDriverFactory
    {
        private IWebDriver GlobalDriver;
        private readonly string driverFolder = typeof(WebDriverFactory).Assembly.GetFolder(@"Drivers").FullName.ToString();
        private WebDriverConfiguration driverConfig;

        private void GetDriverConfig()
        {
            driverConfig = JsonConfigProvider.WebDriver;
        }
        private void SetWebDriver()
        {
            switch (driverConfig.BrowserName)
            {
                case BrowserName.Chrome:
                    GlobalDriver = new ChromeDriver(driverFolder);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(JsonConfigProvider.WebDriver.BrowserName),
                        JsonConfigProvider.WebDriver.BrowserName, null);
            }
        }
        private void SetDriverNorms()
        {
            GlobalDriver.Manage().Window.Maximize();
            GlobalDriver.Manage().Timeouts().ImplicitWait
                    = TimeSpan.FromSeconds(JsonConfigProvider.WebDriver.DefaultTimeout);
        }
        public IWebDriver GetWebDriver()
        {
            if (GlobalDriver == null)
            {
                GetDriverConfig();
                SetWebDriver();
                SetDriverNorms();
            }
            return GlobalDriver;
        }

        public void CloseDriver()
        {
            GlobalDriver.Close();
            GlobalDriver.Quit();
        }
    }
}
