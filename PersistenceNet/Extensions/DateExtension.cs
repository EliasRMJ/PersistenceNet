namespace PersistenceNet.Extensions
{
    public static class DateExtension
    {
        public static string ToDatePTbr(this DateOnly date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static string ToTimePTbr(this DateTime date)
        {
            return date.ToString("HH:mm");
        }

        public static string ToDatePTbr(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static string ToDateTimePTbr(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm");
        }

        public static bool EqualToday(this DateOnly date)
        {
            return date == DateOnly.FromDateTime(DateTime.Now);
        }

        public static bool EqualTodayOrMinus(this DateOnly date)
        {
            return date <= DateOnly.FromDateTime(DateTime.Now);
        }

        public static bool EqualTodayOrBigger(this DateOnly date)
        {
            return date >= DateOnly.FromDateTime(DateTime.Now);
        }

        public static bool IsDateBigger(this DateOnly date)
        {
            return date > DateOnly.FromDateTime(DateTime.Now);
        }

        public static bool IsDateMinus(this DateOnly date)
        {
            return date < DateOnly.FromDateTime(DateTime.Now);
        }

        public static DateOnly Today(this DateTime date)
        {
            return DateOnly.FromDateTime(date);
        }

        public static TimeOnly ToTime(this DateTime date)
        {
            return TimeOnly.Parse(date.ToTimePTbr());
        }

        public static DateTime AddToDate(this DateOnly date, string hourAndMinute)
        {
            var arrs = !string.IsNullOrEmpty(hourAndMinute) && hourAndMinute.Contains(':') ?
                                    hourAndMinute.Split(':') :
                                    "00:00".Split(':');
            var timeOnly = new TimeOnly(int.Parse(arrs[0]), int.Parse(arrs[1]));
            return date.ToDateTime(timeOnly);
        }

        public static DateTime AddToDate(this DateTime date, string hourAndMinute)
        {
            var arrs = !string.IsNullOrEmpty(hourAndMinute) && hourAndMinute.Contains(':') ?
                                    hourAndMinute.Split(':') :
                                    "00:00".Split(':');
            var newDate = date.Date;
            return newDate.AddHours(int.Parse(arrs[0])).AddMinutes(int.Parse(arrs[1]));
        }

        public static int GetWeekly(this DateOnly date)
        {
            var weekly = date.Day / 7;
            if (date.Day % 7 > 0) weekly++;
            return weekly;
        }
    }
}