using CoreFramework.Enums;
using CoreFramework.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace CoreFramework.UnitTests
{
    public class CheckJSONConfigProvider
    {
        [Test]
        public void ChkBrowserTypeIsChrome()
        {
            JsonConfigProvider.WebDriver.BrowserName.Should().Be(BrowserName.Chrome);
        }

        [Test]
        public void ChkApplicationURLIsNotNull()
        {
            JsonConfigProvider.Environment.ApplicationUrl.Should().NotBeNull();
        }
    }
}
