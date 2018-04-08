using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Common
{
    public class DateTimeFormat
    {
        public static DateTime DateFormat(string date)
        {
            //date = "2008-05-19 17:25";
            if (!string.IsNullOrEmpty(date)) // if input = ""// it's not null so can't use if(date != Null)
            {
                string[] splitDay = date.Split(' ');
                string[] today = splitDay[0].Split('-');
                string[] time = splitDay[1].Split(':');
                int y = Convert.ToInt32(today[0]);
                int m = Convert.ToInt32(today[1]);
                int d = Convert.ToInt32(today[2]);
                int H = Convert.ToInt32(time[0]);
                int Mm = Convert.ToInt32(time[1]);
                // ให้ 0 = second
                DateTime dt = new DateTime(y, m, d, H, Mm, 0);
                return dt;
            }
            else
            {
                throw new Exception("input error");
            }

        }

        public static DateTime DateFormatBySlash(string date)
        {
            //date = "2008-05-19 17:25";
            if (!string.IsNullOrEmpty(date)) // if input = ""// it's not null so can't use if(date != Null)
            {
                string[] splitDay = date.Split(' ');
                string[] today = splitDay[0].Split('/');
                string[] time = splitDay[1].Split(':');
                int y = Convert.ToInt32(today[0]);
                int m = Convert.ToInt32(today[1]);
                int d = Convert.ToInt32(today[2]);
                int H = Convert.ToInt32(time[0]);
                int Mm = Convert.ToInt32(time[1]);
                // ให้ 0 = second
                DateTime dt = new DateTime(y, m, d, H, Mm, 00);
                return dt;
            }
            else
            {
                throw new Exception("input error");
            }

        }

        public static DateTime DateFormatDateOnlyMonthFirst(string date)
        {
            //date = "2008-05-19 17:25";
            if (!string.IsNullOrEmpty(date)) // if input = ""// it's not null so can't use if(date != Null)
            {
                string[] splitDay = date.Split(' ');
                string[] today = splitDay[0].Split('/');
                int m = Convert.ToInt32(today[0]);
                int d = Convert.ToInt32(today[1]);
                int y = Convert.ToInt32(today[2]);
                DateTime dt = new DateTime(y, m, d);
                return dt;
            }
            else
            {
                throw new Exception("input error");
            }
        }
        public static DateTime DateFormatDateOnlyDayfirst(string date)
        {
            //date = "2008-05-19 17:25";
            if (!string.IsNullOrEmpty(date)) // if input = ""// it's not null so can't use if(date != Null)
            {
                string[] splitDay = date.Split(' ');
                string[] today = splitDay[0].Split('/');
                int d = Convert.ToInt32(today[0]);
                int m = Convert.ToInt32(today[1]);
                int y = Convert.ToInt32(today[2]);
                DateTime dt = new DateTime(y, m, d);
                return dt;
            }
            else
            {
                throw new Exception("input error");
            }
        }
        public static DateTime DateFormatDateOnlyMonthfirstByMinus(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                string[] splitDay = date.Split(' ');
                string[] today = date.Split('-');
                int m = Convert.ToInt32(today[0]);
                int d = Convert.ToInt32(today[1]);
                int y = Convert.ToInt32(today[2]);
                DateTime dt = new DateTime(y, m, d);
                return dt;
            }
            else
            {
                throw new Exception("input error");
            }
        }
        public static string DateFormatToString(DateTime? date)
        {
            if (date == null)
            {
                return "";
            }
            else
            {
                return date.Value.ToString("dd/MM/") + (int.Parse(date.Value.ToString("yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo))).ToString();
            }
        }

        public static string TimeFormatToString(DateTime? date)
        {
            if (date == null)
            {
                return "";
            }
            else
            {
                return date.Value.ToString("HH:mm");
            }
        }
    }

}
