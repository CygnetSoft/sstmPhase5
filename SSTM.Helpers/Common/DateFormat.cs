using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTM.Helpers.Common
{
    public class DateFormat
    {
        public DateTime getDate(string date)
        {
            DateTime dt;
            try
            { 
                dt = DateTime.ParseExact(date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-AU"), DateTimeStyles.None);

                return dt;
            }
            catch (Exception)
            {
                dt = new DateTime();
                return dt;
            }
        }

        public DateTime currentdate()
        {
            //return getDate(DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-"));
            return DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy").Replace("-", "/"), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-AU"), DateTimeStyles.None);
        }
        public dynamic timezone(string date)
        {
            string data = "";
            if (date.Contains("-"))
            {
                data = date.Replace("-", "/");
            }
            else
            {
                data = date;
            }

            TimeZone zone = TimeZone.CurrentTimeZone;
            // var dt = DateTime.ParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dt = DateTime.ParseExact(data, "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-AU"), DateTimeStyles.None);

            var s = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dt, zone.StandardName);

            return s;
            //return s.ToString().Replace("\\","-");
        }

        public dynamic timezonewithtime(string date)
        {
            string data = "";
            if (date.Contains("-"))
            {
                data = date.Replace("-", "/");
            }
            else
            {
                data = date;
            }

            TimeZone zone = TimeZone.CurrentTimeZone;
            var dt = DateTime.ParseExact(date, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
            var s = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dt, zone.StandardName);
            return s;
            //return s.ToString().Replace("\\","-");
        }
        public DateTime GetSingaporeTime()
        {
            TimeZone time2 = TimeZone.CurrentTimeZone;
            var test = time2.ToUniversalTime(DateTime.Now);
            var singaporeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            var singaporeTime = TimeZoneInfo.ConvertTimeFromUtc(test, singaporeTimeZone);
            return singaporeTime;
        }
        public DateTime ConvertSingaporeTime(DateTime dt)
        {
            TimeZone time2 = TimeZone.CurrentTimeZone;
            var test = time2.ToUniversalTime(dt);
            var singaporeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            var singaporeTime = TimeZoneInfo.ConvertTimeFromUtc(test, singaporeTimeZone);
            return singaporeTime;
        }
    }
   
}
