using CoreFramework.Enums;

namespace CoreFramework.Models
{
    public class WebDriverConfiguration
    {
        public BrowserName BrowserName { get; set; }
        public string ScreenshotsPath { get; set; }
        public int DefaultTimeout { get; set; }
    }
}
