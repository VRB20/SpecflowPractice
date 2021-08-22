using FunctionalTests.Pages;
using OpenQA.Selenium;

namespace FunctionalTests.Infrastructure
{
    public class PageObjectManager
    {
        private IWebDriver driver;
        private HomePage homePage;
        private SearchResultsPage searchResultsPage;

        public PageObjectManager(IWebDriver driver)
        {
            this.driver = driver;
        }

        public HomePage GetHomePage()
        {
            return (homePage == null) ? homePage = new HomePage(driver) : homePage;
        }

        public SearchResultsPage GetSearchResultsPage()
        {
            return (searchResultsPage == null) ? searchResultsPage = new SearchResultsPage(driver) : searchResultsPage;
        }
    }
}
