using CoreFramework.Infrastructure;
using CoreFramework.Infrastructure.Extensions;
using CoreFramework.Utils;
using FunctionalTests.Pages;
using NUnit.Framework;
using System.Reflection;

namespace FunctionalTests.Nunit_Tests
{
    public class BaseTest : CoreBaseTest
    {
        protected HomePage home;
        [SetUp]
        public void Setup()
        {
            home = new HomePage(driver);
            home.goToPage(JsonConfigProvider.Environment.ApplicationUrl);
            TestDataExtension.TestDataBatchConstruction(Assembly.GetExecutingAssembly());
            ObjRepo.LoadObjectRepository(); 
        }
    }
}
