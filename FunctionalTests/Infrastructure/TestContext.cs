using CoreFramework.Helpers;
using FunctionalTests.Pages;

namespace FunctionalTests.Infrastructure
{
    public class TestContext
    {
        private PageObjectManager pageObjectManager;
        private WebDriverFactory webDriverFactory;
        public SharedData sharedData;
        public TestContext()
        {
            webDriverFactory = new WebDriverFactory();
            pageObjectManager = new PageObjectManager(webDriverFactory.GetWebDriver());
            sharedData = new SharedData();
        }

        public WebDriverFactory GetWebDriverFactory()
        {
            return webDriverFactory;
        }

        public PageObjectManager GetPageObjectManager()
        {
            return pageObjectManager;
        }
    }
}
