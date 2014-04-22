using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Utility
{
    public class DateTimeUtility
    {
        public static DateTime TimePart(DateTime dateTime)
        {
            //dt.ToString("HH:mm"); // 07:00 // 24 hour clock // hour is always 2 digits
            //dt.ToString("hh:mm tt"); // 07:00 AM // 12 hour clock // hour is always 2 digits
            //dt.ToString("H:mm"); // 7:00 // 24 hour clock
            //dt.ToString("h:mm tt"); // 7:00 AM // 12 hour clock

            return Convert.ToDateTime(dateTime.ToString("HH:mm"));
        }

        public static DateTime GetDateTime(DateTime dt, string time)
        {
            return DateTime.Now;
        }
    }
}