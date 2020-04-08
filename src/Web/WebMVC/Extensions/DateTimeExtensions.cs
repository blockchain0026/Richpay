using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToFullString(this DateTime dateTime)
        {
            return dateTime.ToString(GetFormatFullString());
        }

        public static string GetFormatFullString()
        {
            return "yyyy/MM/dd HH:mm:ss";
        }
    }
}
