using CoreFramework.Models;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace CoreFramework.Utils
{
    public class JsonConfigProvider
    {
        private const string WebDriverConfigSectionName = "webdriver";
        private const string EnvironmentConfigSectionName = "environment";
        private const string FilePath = @"..\..\..\TestSettings.json";
        private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FilePath);

        public static WebDriverConfiguration WebDriver =>
            Load<WebDriverConfiguration>(WebDriverConfigSectionName);

        public static EnvironmentConfiguration Environment =>
            Load<EnvironmentConfiguration>(EnvironmentConfigSectionName);

        private static T Load<T>(string sectionName) =>
            JObject.Parse(File.ReadAllText(SettingsPath)).SelectToken(sectionName).ToObject<T>();
    }
}
