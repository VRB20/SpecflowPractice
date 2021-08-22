using CoreFramework.Services;
using OpenQA.Selenium;

namespace CoreFramework.Infrastructure.Extensions
{
    public static class ObjectConstructionExtension
    {
        public static By ConstructElement(this string identifier)
        {
            By by = null;
            (string locator, string path) = ObjectRepoService.ObjRepoBatchConstruction(identifier);
            switch (locator.ToLower())
            {
                case "xpath":
                    by = By.XPath(path);
                    break;
                case "id":
                    by = By.Id(path);
                    break;
                case "linktext":
                    by = By.LinkText(path);
                    break;
                case "partiallinktext":
                    by = By.PartialLinkText(path);
                    break;
                case "classname":
                    by = By.ClassName(path);
                    break;
                case "tagname":
                    by = By.TagName(path);
                    break;
                case "cssselector":
                    by = By.CssSelector(path);
                    break;
                default:
                    by = By.XPath(path);
                    break;
            }
            return by;
        }
    }
}
