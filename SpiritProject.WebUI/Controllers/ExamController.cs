using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoreLinq;
using SpiritProject.Common;
using SpiritProject.Services;
using SpiritProject.WebUI.ActionFilters;
using SpiritProject.WebUI.Utilities;
using System.Threading;
using System.Globalization;

namespace SpiritProject.WebUI.Controllers
{
    [Utilities.SessionAuthen]
    [Internationalization]
    public class ExamController : BaseController
    {
        Exam examServices = new Exam();

        // GET: Exam
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DoExam(int ExamId, int TestNo)
        {
            ViewBag.SessionTime = Session.Timeout;
            //string[] userLanguages = (string)filterContext.RouteData.Values["lang"];
            string culture = (string)RouteData.Values["culture"];
            ViewBag.index = 0;
            ViewBag.isFinal = 0;
            Member memberServices = new Member();

            //var login = memberServices.CheckLoginUser("", "");
            //if (login != null)
            //{
            //    RuntimeVariable.CurrentUser = new MemberModel();
            //    RuntimeVariable.CurrentUser.MemberId = login.MemberId;
            //    RuntimeVariable.CurrentUser.FirstName = login.FirstName;
            //    RuntimeVariable.CurrentUser.LastName = login.LastName;
            //    RuntimeVariable.CurrentUser.DOB = login.DOB;
            //    //   return RedirectToAction("Dashboard", "Index");
            //}
            //else
            //    return RedirectToAction("Login", "Login");

            Exam.StampAccessTimes(ExamId, RuntimeVariable.CurrentUser.MemberId, TestNo);

            ExamStatModel stat = examServices.GetExamStat(RuntimeVariable.CurrentUser.MemberId, TestNo);
            //ExamStatModel stat = examServices.GetExamStat(r);
            RuntimeVariable.CurrentAnswerSheet = new List<AnswerSheetModel>();
            RuntimeVariable.CurrentAnswerSheet = examServices.GetExam(RuntimeVariable.CurrentUser.MemberId, ExamId, TestNo, culture);


            if (RuntimeVariable.CurrentAnswerSheet.Count() == 0)
                return RedirectToAction("Index", "Dashboard");
            else if (RuntimeVariable.CurrentAnswerSheet.Count() == 1)
                ViewBag.isFinal = 1;
            ViewBag.stat = stat;
            return View(RuntimeVariable.CurrentAnswerSheet.FirstOrDefault());

        }

        [HttpPost]
        public ActionResult GetExam(int orderRank, int isFinal, string Answered, decimal TimeUsed = 0)
        {
            StatusResponse result = new StatusResponse();
            AnswerSheetModel response = new AnswerSheetModel();
            RuntimeVariable.startTime = DateTime.Now;
            response.isFinal = 0;
            orderRank = orderRank > 0 ? orderRank : 0;
            var index = RuntimeVariable.CurrentAnswerSheet.IndexOf(RuntimeVariable.CurrentAnswerSheet.Single(i => i.QuestionOrder == orderRank));

            ///SAVE THIS SUBMITTION FIRST
            if (isFinal == 0)
            {
                response = RuntimeVariable.CurrentAnswerSheet.Where(i => i.QuestionOrder == orderRank).FirstOrDefault();
                //response = RuntimeVariable.CurrentAnswerSheet;
                var choice = response.Choice.Where(c => c.ChoiceType == Answered).Select(c => c).FirstOrDefault();
                result = SaveAnswer(RuntimeVariable.CurrentUser.MemberId, response.QuestionId, response.QuestionOrder, choice.ChoiceId, choice.ChoiceType, choice.Point, TimeUsed, response.ExamId, isFinal, response.TestNo);
            }
            else if (isFinal == 1)
            {
                response = RuntimeVariable.CurrentAnswerSheet.Where(i => i.QuestionOrder == orderRank).FirstOrDefault();
                var choice = response.Choice.Where(c => c.ChoiceType == Answered).Select(c => c).FirstOrDefault();
                if (choice != null)
                {
                    result = SaveAnswer(RuntimeVariable.CurrentUser.MemberId, response.QuestionId, response.QuestionOrder, choice.ChoiceId, choice.ChoiceType, choice.Point, TimeUsed, response.ExamId, isFinal, response.TestNo);
                }
                else
                {
                    result = SaveAnswer(RuntimeVariable.CurrentUser.MemberId, response.QuestionId, response.QuestionOrder, 0, "N", 0, TimeUsed, response.ExamId, isFinal, response.TestNo);

                }
                return Json(new { url = Url.Action("Index", "Dashboard"), status = result.status, TotalScore = result.TotalScore });
            }


            /// GET NEXT OF LIST
            if (index + 1 == (RuntimeVariable.CurrentAnswerSheet.Count() - 1))
            {
                //End of Answer Sheet
                //response = RuntimeVariable.CurrentAnswerSheet.Where(i => i.QuestionOrder == (index + 1)).FirstOrDefault();
                response = RuntimeVariable.CurrentAnswerSheet[index + 1];
                response.isFinal = 1;

            }
            else if (index + 1 < (RuntimeVariable.CurrentAnswerSheet.Count() - 1))
            {
                response = RuntimeVariable.CurrentAnswerSheet[index + 1];
                //response = RuntimeVariable.CurrentAnswerSheet.Where(i => i.QuestionOrder == index + 1).FirstOrDefault();
                response.isFinal = 0;
            }



            return Json(new { data = response, status = result.status });


        }


        [HttpPost]
        public ActionResult AutoSave(int index, decimal TimeUsed = 0)
        {
            StatusResponse result = new StatusResponse();
            AnswerSheetModel response = new AnswerSheetModel();
            RuntimeVariable.startTime = DateTime.Now;
            response.isFinal = 0;
            response = RuntimeVariable.CurrentAnswerSheet.Where(i => i.QuestionOrder == index).FirstOrDefault();

            result = AutoSaveAnswer(RuntimeVariable.CurrentUser.MemberId, response.QuestionId, response.QuestionOrder, null, "N", 0, TimeUsed, response.ExamId, 0, response.TestNo);

            return Json(new { data = response, status = result.status });


        }


        public StatusResponse SaveAnswer(string memberId, int questionId, int questionOrder, int choiceId, string choiceType, int point, decimal timeUsed, int examId, int IsFinal, int testNo)
        {
            /// update answersheeet answered durations after press next
            /// AnswerSheetId
            return examServices.SaveAnswer(memberId, questionId, questionOrder, choiceId, choiceType, point, timeUsed, examId, IsFinal, testNo);
        }

        public StatusResponse AutoSaveAnswer(string memberId, int questionId, int questionOrder, int? choiceId, string choiceType, int point, decimal timeUsed, int examId, int IsFinal, int testNo)
        {
            /// update answersheeet answered durations after press next
            /// AnswerSheetId
            return examServices.AutoSaveAnswer(memberId, questionId, questionOrder, choiceId, choiceType, point, timeUsed, examId, IsFinal, testNo);
        }

        public ActionResult ExamOverView()
        {
            return View();

        }

        [HttpPost]
        public ActionResult PreExamIntro(int ExamId, int TestNo, int TotalQuestion, int TimeSet, string lang)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
            RouteData.Values["culture"] = lang;

            return View("ExamIntro", new { ExamId, TestNo, TotalQuestion, TimeSet });
        }

        public ActionResult ExamIntro(int ExamId, int TestNo, int TotalQuestion, int TimeSet, string lang)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
            RouteData.Values["culture"] = lang;
            ViewBag.lang = lang;
            ViewBag.ExamId = ExamId;
            ViewBag.TestNo = TestNo;
            ViewBag.TimeSet = TimeSet;
            ViewBag.TotalQuestion = TotalQuestion;
            return View();
        }

        public ActionResult ExamSummary(int ExamId, int TestNo, string MemberId = "")
        {
            MemberId = string.IsNullOrEmpty(MemberId) ? RuntimeVariable.CurrentUser.MemberId : MemberId;
            string culture = (string)RouteData.Values["culture"];
            var summary = Services.Exam.SummaryScore(culture, ExamId, TestNo, MemberId);
            var max = summary.MaxBy(i => i.Score);
            max.DoneQuestion = summary.Sum(i => i.DoneQuestion);
            if (summary.Count > 0)
                ViewBag.MaxObj = max;
            else
                ViewBag.MaxObj = new SummaryScoreModel();

            var allMean = max.Mean;
            ViewBag.allMean = allMean;

            // ViewBag.TotalScore = summary.TotalScore;

            return View(summary);

        }

        public ActionResult CreateNewAdmin(MemberModel member)
        {


            return RedirectToAction("Index", "Admin");
        }
    }

}
