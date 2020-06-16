using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLvlServInfo.Auxiliary.Extensions
{
    public static class DateTimeExt
    {
        public static bool Between(this DateTime d, DateTime start, DateTime stop)
        {
            return (d >= start) && (d <= stop);
        }


        public static DateTime CutToSeconds(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }

        public static DateTime NowIfNull(this DateTime? dt)
        {
            return dt ?? DateTime.Now;
        }
    }
}
