using CoreFramework.Utils;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace FunctionalTests.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver driver) : base(driver) {}

        private const string searchBoxXpath = "//input[contains(@class,'SearchInput__Input-sc')]";
        [FindsBy(How = How.XPath, Using = searchBoxXpath)]
        private readonly IWebElement searchBox;

        [FindsBy(How = How.XPath, Using = "//button[@title='Search']")]
        private readonly IWebElement searchButton;



        public void goToPage(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        public void SearchItem(string searchString)
        {
            searchBox.Clear();
            searchBox.SendKeys(searchString);
            searchButton.Click();
        }

    }
}
