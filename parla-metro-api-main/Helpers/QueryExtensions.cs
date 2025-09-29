using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace parla_metro_api_main.Helpers
{
    public static class QueryExtensions
    {
        public static string ToQueryString<T>(this T obj)
        {
            if (obj == null) return string.Empty;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var queryParams = new List<string>();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj);
                if (value != null)
                {
                    var encodedValue = HttpUtility.UrlEncode(value.ToString());
                    queryParams.Add($"{prop.Name}={encodedValue}");
                }
            }

            return string.Join("&", queryParams);
        }
    }
}