using CoreFramework.Infrastructure;
using CoreFramework.Infrastructure.Extensions;
using CoreFramework.Utils;
using FunctionalTests.Pages;
using System.Reflection;
using TechTalk.SpecFlow;

namespace FunctionalTests.Infrastructure
{
    [Binding]
    public class ScenarioHooks
    {
        public TestContext testContext;
        public HomePage home;
        public ScenarioHooks(TestContext context)
        {
            testContext = context;
            home = testContext.GetPageObjectManager().GetHomePage();
        }
        [BeforeTestRun(Order = 0)]
        public static void BeforeTestRun()
        {
            TestDataExtension.TestDataBatchConstruction(Assembly.GetExecutingAssembly());
            ObjRepo.LoadObjectRepository();
        }

        [BeforeScenario]
        public void setup()
        {
            home.goToPage(JsonConfigProvider.Environment.ApplicationUrl);
        }

        [AfterScenario]
        public void teardown()
        {
            testContext.GetWebDriverFactory().CloseDriver();
        }
    }
}
