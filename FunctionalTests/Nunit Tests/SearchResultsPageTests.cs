using FunctionalTests.Pages;
using NUnit.Framework;
using System.Threading;
using FluentAssertions;
using CoreFramework.Infrastructure.Extensions;

namespace FunctionalTests.Nunit_Tests
{
    public class SearchResultsPageTests : BaseTest
    {
       [Test]
        public void Check2750ModelIsUnavailable()
        {
            home.SearchItem("TC001".GetTestData("Product").Trim());

            SearchResultsPage srchRslts = new SearchResultsPage(driver);
            srchRslts.GetDesiredProductDetails("TC001".GetTestData("Model").Trim())
                .Should().Contain("Unavailable Online");

            //srchRslts.GetDesiredProductDetails("2395").Should().Contain("Add to Cart");
        }
    }
}
