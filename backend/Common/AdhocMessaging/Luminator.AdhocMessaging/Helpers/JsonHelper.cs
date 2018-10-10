namespace Luminator.AdhocMessaging.Helpers
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class JsonHelper
    {
        public static List<string> InvalidJsonElements;

        public static List<T> DeserializeToList<T>(string jsonString)
        {
            InvalidJsonElements = null;
            var array = JArray.Parse(jsonString);
            var objectsList = new List<T>();
            foreach (var item in array)
                try
                {
                    objectsList.Add(item.ToObject<T>());
                }
                catch (Exception ex)
                {
                    InvalidJsonElements = InvalidJsonElements ?? new List<string>();
                    InvalidJsonElements.Add(item.ToString());
                }
            return objectsList;
        }

        public static T ConvertJsonToClass<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToJsonString(this object model)
        {
            if (model is string) throw new ArgumentException("model should not be a string");
            return JsonConvert.SerializeObject(model, Formatting.Indented,new JsonSerializerSettings()
                                                                              {
                                                                                  DateFormatString = "dd-MM-yyyy h:mm tt"
            });
        }
    }
}