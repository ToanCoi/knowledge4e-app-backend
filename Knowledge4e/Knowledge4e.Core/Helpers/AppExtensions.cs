using System;
using System.Linq;

namespace Knowledge4e.Core.Helpers
{
    public static class AppExtensions
    {
        public static DateTime GetUnixDate(this DateTime dateTime, string value)
        {
            if (!string.IsNullOrEmpty(value) && long.TryParse(value, out long result))
            {  
                dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                return dateTime.AddMilliseconds(result);
            }

            return dateTime;
        }

        public static int GetValue(this int value, string key) 
        {
            if (!string.IsNullOrEmpty(key) && int.TryParse(key, out int result))
                return result;
            
            return value;
        }

        public static string CharFormat(this string value) => string.Concat(value.ToCharArray().Select((c, i) => { return i == 0 ? char.ToUpper(c) : c; }));
    }
}
