using CoreFramework.Infrastructure.Extensions;
using CoreFramework.Utils;
using FunctionalTests.Pages;
using NUnit.Framework;
using System.Threading;

namespace FunctionalTests.Nunit_Tests
{
    public class HomePageTests : BaseTest
    {
        [Test]
        public void LaunchOfficeWorks()
        {
            home.goToPage(JsonConfigProvider.Environment.ApplicationUrl);
            home.SearchItem("TC001".GetTestData("Product").Trim());
        }
        
    }
}
