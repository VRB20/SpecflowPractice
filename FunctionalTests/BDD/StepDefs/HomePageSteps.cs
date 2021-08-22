using CoreFramework.Infrastructure.Extensions;
using FunctionalTests.Infrastructure;
using FunctionalTests.Pages;
using TechTalk.SpecFlow;

namespace FunctionalTests.BDD.StepDefs
{
    [Binding]
    public class HomePageSteps
    {
        public TestContext testContext;
        public HomePage home;

        public HomePageSteps(TestContext context)
        {
            testContext = context;
            home = testContext.GetPageObjectManager().GetHomePage();
        }
        [Given(@"I am on the officeworks homepage '(.*)'")]
        public void GivenIAmOnTheOfficeworksHomepage(string testcaseName)
        {
            testContext.sharedData.TestCaseName = testcaseName;
        }

        [When(@"I search for the product")]
        public void WhenISearchForTheProduct()
        {
            home.SearchItem(testContext.sharedData.TestCaseName.GetTestData("Product"));
        }

    }
}
