using CoreFramework.Infrastructure.Extensions;
using CoreFramework.Utils;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;

namespace FunctionalTests.Pages
{
    public class SearchResultsPage : BasePage
    {
        public SearchResultsPage(IWebDriver driver) : base(driver) { }

        private const string productListXpath = "//*[contains(@data-ref,'product-tile-')]";
        [FindsBy(How = How.XPath, Using = productListXpath)]
        private readonly IList<IWebElement> productList;


        public string GetDesiredProductDetail(string modelNo)
        {
            var result = "Not Listed";
            foreach (var product in productList)
            {
                if (product.Text.Contains(modelNo))
                    result = product.Text;
            }
            return result;
        }

        public string GetDesiredProductDetails(string modelNo)
        {
            var result = "Not Listed";
            var productList = Driver.FindElements("productList".ConstructElement());
            foreach (var product in productList)
            {
                if (product.Text.Contains(modelNo))
                    result = product.Text;
            }
            return result;
        }

    }
}
