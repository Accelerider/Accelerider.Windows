using Newtonsoft.Json;

namespace System
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };

        public static string ToJson(this object @object, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(@object, formatting, JsonSerializerSettings);
        }

        public static T ToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }
    }
}
