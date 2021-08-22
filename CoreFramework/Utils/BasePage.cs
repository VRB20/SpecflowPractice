using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumExtras.WaitHelpers;
using System;

namespace CoreFramework.Utils
{
    public class BasePage
    {
        protected readonly IWebDriver Driver;
        protected WebDriverWait wait;
        public BasePage(IWebDriver driver)
        {
            Driver = driver;
            PageFactory.InitElements(Driver, this);
            //TODO:- Can get this timeout parameter from settings file
            wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }

        protected void MouseHover(IWebElement elem)
        {
            Actions action = new Actions(Driver);
            action.MoveToElement(elem).Perform();
        }
        protected void MouseHoverAndClick(IWebElement elem)
        {
            Actions action = new Actions(Driver);
            action.MoveToElement(elem).Click().Build().Perform();
        }

        protected void WaitForElementToExistByXpath(string elemXpath)
        {
            wait.Until(ExpectedConditions.ElementExists(By.XPath(elemXpath)));
        }

        protected void WaitForElementToBeClickableByXpathAndClick(string elemXpath)
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(elemXpath)));
            Driver.FindElement(By.XPath(elemXpath)).Click();
        }

        protected void WaitForElementToBeClickableAndClick(IWebElement elem)
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(elem));
            elem.Click();
        }
    }
}
