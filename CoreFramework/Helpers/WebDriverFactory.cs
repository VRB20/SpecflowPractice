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
        IWebDriver driver;
        public static readonly string driverFolder = typeof(WebDriverFactory).Assembly.GetFolder(@"Drivers").FullName.ToString();
        public IWebDriver GetWebDriver(WebDriverConfiguration driverConfig)
        {
            switch (driverConfig.BrowserName)
            {
                case BrowserName.Chrome:
                    driver = new ChromeDriver(driverFolder);
                    return driver;
                default:
                    throw new ArgumentOutOfRangeException(nameof(JsonConfigProvider.WebDriver.BrowserName),
                        JsonConfigProvider.WebDriver.BrowserName, null);
            }
        }
    }
}
