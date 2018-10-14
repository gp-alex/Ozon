using System;
using System.Globalization;

namespace Infrastructure.Utils
{
    public static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int WeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.WeekOfYear() - first.WeekOfYear() + 1;
        }

        public static int WeekOfYear(this DateTime dt)
        {
            return _gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static int WeeksInMonth(int year, int month)
        {
            return new DateTime(year, month, 1)
                .AddDays(
                    DateTime.DaysInMonth(year, month) - 1
                )
                .WeekOfMonth();
        }

        public static int FirstDayOfWeek(int year, int month, int week)
        {
            var dt = new DateTime(year, month, 1);
            for (int i = 0; i < DateTime.DaysInMonth(year, month); ++i)
            {
                if (dt.WeekOfMonth() == week)
                {
                    return i + 1;
                }

                dt = dt.AddDays(1);
            }

            return -1;
        }

        public static int LastBusinessDayOfWeek(int year, int month, int week)
        {
            var dt = new DateTime(year, month, FirstDayOfWeek(year, month, week));
            for (int i = FirstDayOfWeek(year, month, week);
                i <= DateTime.DaysInMonth(year, month);
                ++i
            )
            {
                if (dt.WeekOfMonth() != week || dt.DayOfWeek == DayOfWeek.Saturday)
                {
                    return i - 1;
                }

                dt = dt.AddDays(1);
            }

            return DateTime.DaysInMonth(year, month);
        }

        public static DateTime LastDtOfWeek(DateTime x)
        {
            int week = x.WeekOfMonth();
            do
            {
                x = x.AddDays(1);
            } while (week == x.WeekOfMonth());

            return x.AddDays(-1);
        }

        public static DateTime LastBusinessDtOfWeek(DateTime x)
        {
            var dt = LastDtOfWeek(x);
            if (dt.DayOfWeek == DayOfWeek.Sunday)
            {
                dt = dt.AddDays(-1);
            }
            if (dt.DayOfWeek == DayOfWeek.Saturday)
            {
                dt = dt.AddDays(-1);
            }

            return dt;
        }
    }
}
