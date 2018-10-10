namespace Luminator.AdhocMessaging.Helpers
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    public static class Helpers
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
                    // CorrectElements
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
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}