using Newtonsoft.Json;

namespace System
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

        public static string ToJson<T>(this T @object, Formatting formatting = Formatting.None)
        {
            var type = @object.GetType();

            return typeof(T) != type
                ? JsonConvert.SerializeObject(@object, type, formatting, JsonSerializerSettings)
                : JsonConvert.SerializeObject(@object, formatting, JsonSerializerSettings);
        }

        public static T ToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }
    }
}
