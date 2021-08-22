using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace CoreFramework.Extensions
{
    public static class SerializationExtensions
    {
        public static readonly JsonSerializerSettings CamelCaseSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        public static readonly JsonSerializerSettings CamelCaseSerializerSettingsExcludeNulls = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore
        };

        public static T As<T>(this string from) => from.DeserializeAs<T>();

        public static T DeserializeAs<T>(this string from, JsonSerializerSettings settings = null)
        {
            settings = settings ?? CamelCaseSerializerSettings;

            return JsonConvert.DeserializeObject<T>(from, settings);
        }

        public static T DeserializeAs<T>(this FileInfo fromFileInfo, JsonSerializerSettings settings = null) => fromFileInfo.Exists
            ? File.ReadAllText(fromFileInfo.FullName).DeserializeAs<T>(settings)
            : default;

        public static string ToJson<T>(this T data, JsonSerializerSettings settings = null) => JsonConvert.SerializeObject(data, Formatting.Indented, settings);
    }
}
