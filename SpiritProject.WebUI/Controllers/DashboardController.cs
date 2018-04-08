using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using SpiritProject.Services;
using SpiritProject.Common;
using SpiritProject.WebUI.ActionFilters;

namespace SpiritProject.WebUI.Controllers
{
    [Utilities.SessionAuthen]
    [Internationalization]
    public class DashboardController : BaseController
    {
        Hash hashService = new Hash();
        public ActionResult Index()
        {
            var lstExam = Services.Exam.GetExamList(RuntimeVariable.CurrentUser.MemberId);

            ///check > 6 month set Complete
            var listExpired = lstExam.Where(i => i.MonthDifference > 6).ToList();
            foreach (var exam in listExpired)
            {
                exam.MemberId = RuntimeVariable.CurrentUser.MemberId;
                Services.Exam.ExpiredExamCommit(exam);
            }
            //hashService.EncodeQuestion();
            //Hash.EncodeChoiceWG();
            //Hash.EncodeChoiceFake();

            //Get DashBorad Item 
            List<MenuModel> menu = new List<MenuModel>();
            foreach (var detail in menu)
            {
                //get List From ExamStat
            }

            return View(lstExam);
        }


    }
}
