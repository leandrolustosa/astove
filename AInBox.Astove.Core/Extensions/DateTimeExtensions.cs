using System;
using System.Linq;

namespace AInBox.Astove.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToBrazilianTimeZone(this DateTime s)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.GetSystemTimeZones().Single(t => t.Id.Equals("E. South America Standard Time", StringComparison.CurrentCultureIgnoreCase));
            return TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
        }

        public static DateTime ToEndTime(this DateTime s)
        {
            DateTime endTime = new DateTime(s.Year, s.Month, s.Day, 23, 59, 59, 999);
            return endTime;
        }

        public static int CalculateAge(this DateTime birthDate)
        {
            // cache the current time
            DateTime now = DateTime.Today; // today is fine, don't need the timestamp from now
            // get the difference in years
            int years = now.Year - birthDate.Year;
            // subtract another year if we're before the
            // birth day in the current year
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
                --years;

            return years;
        }

        public static DateTime SomarDiaUtil(this DateTime date, int dias)
        {
            for (int i = 0; i <= dias; i++)
            {
                date = date.AddDays(1);
                while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    date = date.AddDays(1);
            }

            return date;
        }

        public static DateTime SubtrairDiaUtil(this DateTime date, int dias)
        {
            for (int i = 0; i <= dias; i++)
            {
                date = date.AddDays(-1);
                while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    date = date.AddDays(-1);
            }

            return date;
        }

        public static DateTime DiaUtil(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
                date = date.AddDays(2);

            if (date.DayOfWeek == DayOfWeek.Sunday)
                date = date.AddDays(1);

            return date;
        }


        public static int DiasUteis(this DateTime date, DateTime dataFinal)
        {
            date = date.DiaUtil();
            dataFinal = dataFinal.DiaUtil();

            if (dataFinal == date)
                return 0;

            int dias = 0;
            if (dataFinal > date)
            {
                while (dataFinal > date)
                {
                    date = date.AddDays(1);
                    if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
                        dias++;
                }

                return dias;
            }

            if (dataFinal < date)
            {
                while (dataFinal < date)
                {
                    date = date.AddDays(-1);
                    if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
                        dias--;
                }

                return dias;
            }

            return 0;
        }

        public static string GetSaudacao(this DateTime data)
        {
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 12)
                return "Bom dia";
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
                return "Boa tarde";
            else
                return "Boa noite";
        }
    }
}
