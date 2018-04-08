using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpiritProject.Common;

namespace SpiritProject.WebUI
{
    public class RuntimeVariable
    {
        public static List<AnswerSheetModel> CurrentAnswerSheet
        {
            get
            {
                return HttpContext.Current.Session["CurrentAnswerSheet"] != null ? (List<AnswerSheetModel>)HttpContext.Current.Session["CurrentAnswerSheet"] : new List<AnswerSheetModel>();
            }
            set
            {
                HttpContext.Current.Session["CurrentAnswerSheet"] = value;
            }
        }

        public static MemberModel CurrentUser
        {
            get
            {
                return HttpContext.Current.Session["CurrentUser"] != null ? (MemberModel)HttpContext.Current.Session["CurrentUser"] : null;
            }
            set
            {
                HttpContext.Current.Session["CurrentUser"] = value;
            }
        }

        public static DateTime startTime
        {
            get
            {
                return HttpContext.Current.Session["StartTime"] != null ? Convert.ToDateTime(HttpContext.Current.Session["StartTime"]) : DateTime.Now;
            }
            set
            {
                HttpContext.Current.Session["StartTime"] = value;
            }
        }


        public static DateTime endTime
        {
            get
            {
                return HttpContext.Current.Session["EndTime"] != null ? Convert.ToDateTime(HttpContext.Current.Session["EndTime"]) : DateTime.Now;
            }
            set
            {
                HttpContext.Current.Session["EndTime"] = value;
            }
        }

        public static int TotalTime
        {
            get
            {
                return HttpContext.Current.Session["TotalTime"] != null ? Convert.ToInt32(HttpContext.Current.Session["TotalTime"]) : 0;
            }
            set
            {
                HttpContext.Current.Session["TotalTime"] = value;
            }
        }

    }

}