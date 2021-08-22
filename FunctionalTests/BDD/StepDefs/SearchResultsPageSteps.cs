using CoreFramework.Infrastructure.Extensions;
using FluentAssertions;
using FunctionalTests.Infrastructure;
using FunctionalTests.Pages;
using TechTalk.SpecFlow;

namespace FunctionalTests.BDD.StepDefs
{
    [Binding]
    public class SearchResultsPageSteps
    {
        public TestContext testContext;
        private SearchResultsPage searchResultsPage;
        private readonly string tcName;
        private string productDetails;
        public SearchResultsPageSteps(TestContext context)
        {
            testContext = context;
            searchResultsPage = testContext.GetPageObjectManager().GetSearchResultsPage();
            tcName = testContext.sharedData.TestCaseName;
        }
        [Then(@"I look for the required model")]
        public void ThenILookForTheRequiredModel()
        {
            productDetails = searchResultsPage.GetDesiredProductDetails(tcName.GetTestData("Model").Trim());
        }

        [Then(@"I found it is unavailable online")]
        public void ThenIFoundItIsUnavailableOnline()
        {
            productDetails.Should().Contain("Unavailable Online"); ;
        }
    }
}
