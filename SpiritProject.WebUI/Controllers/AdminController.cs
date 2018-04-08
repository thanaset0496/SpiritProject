using MoreLinq;
using Rotativa;
using SpiritProject.Common;
using SpiritProject.WebUI.ActionFilters;
using SpiritProject.WebUI.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SpiritProject.WebUI.Controllers
{
    public class AdminController : BaseController
    {
        //
        // GET: /Admin/
        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult Index()
        {
            string culture = (string)RouteData.Values["culture"];

            var x = Resources.Resource.lbl_hello;
            if (culture == "en")
            {
                ViewBag.Fullname = RuntimeVariable.CurrentUser.Title + " " + RuntimeVariable.CurrentUser.MemberNameEN + " " + RuntimeVariable.CurrentUser.MemberLastNameEN;
            }
            else if (culture == "th")
            {
                ViewBag.Fullname = RuntimeVariable.CurrentUser.Title + " " + RuntimeVariable.CurrentUser.MemberNameTH + " " + RuntimeVariable.CurrentUser.MemberLastNameTH;
            }
            return View();
        }

        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult NewAdmin()
        {
            return View();
        }

        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult MemberList()
        {
            return View();
        }


        [Utilities.SessionAuthen]
        [HttpPost]
        public JsonResult GetAdminList(DataTableAjaxPostModel model)
        {
            JsonResult result = new JsonResult();

            string culture = (string)RouteData.Values["culture"];
            string search = Request.Form.GetValues("search[value]")[0];
            int draw = model.draw;//Request.Form.GetValues("draw")[0];
            int order = Convert.ToInt32(Request.Form.GetValues("order[0][column]")[0]);
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];
            int startRec = model.start;//Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = model.length; // Convert.ToInt32(Request.Form.GetValues("length")[0]);
            var lstAdmin = Services.Member.GetAdminList(search, order, orderDir, startRec, pageSize, culture);
            if (lstAdmin.Count > 0)
            {

                var TotalRec = lstAdmin.Select(i => i.TotalItem).FirstOrDefault();
                // return Json(new { data = lstExam });
                result = this.Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = TotalRec,
                    recordsFiltered = TotalRec,
                    data = lstAdmin
                }, JsonRequestBehavior.AllowGet);
            }
            else result = this.Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = 0,
                recordsFiltered = 0,
                data = new List<MemberModel>()
            }, JsonRequestBehavior.AllowGet);

            //throw new Exception("TEST");

            return result;
        }

        [Utilities.SessionAuthen]
        [HttpPost]
        public JsonResult GetMemberList(DataTableAjaxPostModel model)
        {
            JsonResult result = new JsonResult();
            string culture = (string)RouteData.Values["culture"];

            string search = Request.Form.GetValues("search[value]")[0];
            int draw = model.draw;//Request.Form.GetValues("draw")[0];
            int order = Convert.ToInt32(Request.Form.GetValues("order[0][column]")[0]);//Request.Form.GetValues("order[0][column]")[0];
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];// Request.Form.GetValues("order[0][dir]")[0];
            int startRec = model.start;//Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = model.length; // Convert.ToInt32(Request.Form.GetValues("length")[0]);
            var lstExam = Services.Member.GetMemberList(search, order, orderDir, startRec, pageSize, culture);
            if (lstExam.Count > 0)
            {

                var TotalRec = lstExam.Select(i => i.TotalItem).FirstOrDefault();
                // return Json(new { data = lstExam });
                result = this.Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = TotalRec,
                    recordsFiltered = TotalRec,
                    data = lstExam
                }, JsonRequestBehavior.AllowGet);
            }
            else result = this.Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = 0,
                recordsFiltered = 0,
                data = new List<MemberModel>()
            }, JsonRequestBehavior.AllowGet);

            return result;
        }

        [Utilities.SessionAuthen]
        [HttpPost]
        public JsonResult GetExamHist(DataTableAjaxPostModel model)
        {
            JsonResult result = new JsonResult();

            string search = Request.Form.GetValues("search[value]")[0];
            //string searchable = Request.Form.GetValues("search[regxe]")[0];
            int draw = model.draw;//Request.Form.GetValues("draw")[0];
            int order = Convert.ToInt32(Request.Form.GetValues("order[0][column]")[0]);//Request.Form.GetValues("order[0][column]")[0]
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];// Request.Form.GetValues("order[0][dir]")[0];
            int startRec = model.start;//Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = model.length; // Convert.ToInt32(Request.Form.GetValues("length")[0]);
            var lstExam = Services.Exam.GetExamHist(search, order, orderDir, startRec, pageSize);
            if (lstExam.Count > 0)
            {

                var TotalRec = lstExam.Select(i => i.TotalItem).FirstOrDefault();
                // return Json(new { data = lstExam });
                result = this.Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = TotalRec,
                    recordsFiltered = TotalRec,
                    data = lstExam
                }, JsonRequestBehavior.AllowGet);
            }
            else result = this.Json(new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = 0,
                recordsFiltered = 0,
                data = new List<MemberModel>()
            }, JsonRequestBehavior.AllowGet);
            return result;
        }

        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult Config()
        {

            MemberModel info = new MemberModel();
            info = Services.Member.GetMember(RuntimeVariable.CurrentUser.MemberId);

            return View(info);
        }

        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult Analytics()
        {
            return View();
        }

        public ActionResult Login(String Message = "")
        {
            ViewBag.Message = Message;
            return View();
        }

        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult AdminList()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Login", "Admin");
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            Session.Clear();
            MemberModel login = new MemberModel();
            /// check login
            /// 
            //if (username == "Admin")
            //{
            //    return RedirectToAction("Index", "Admin");

            //}
            //else
            //{
            if (RuntimeVariable.CurrentUser != null)
            {
                if (username != RuntimeVariable.CurrentUser.MemberId)
                {
                    //   return View("~/Views/Shared/ErrorLogin.cshtml");
                    return RedirectToAction("Error_One", "CommonViews");
                }
                else if (username == "Admin")
                {
                    return RedirectToAction("Index", "Admin");

                }
                //    }
            }

            login = Services.Member.CheckAdminUser(username, password);
            if (login != null)
            {
                if (!String.IsNullOrEmpty(login.Message))
                {
                    return RedirectToAction("Login", "Admin", new { Message = login.Message });
                }

                if (login.FailLogin > 0)
                {
                    if (login.FailLogin >= 3)
                        return RedirectToAction("Login", "Admin", new { Message = "Wrong 3 Times Password" });
                    else
                        return RedirectToAction("Login", "Admin", new { Message = "Wrong Password" });

                }
                else
                {

                    RuntimeVariable.CurrentUser = new MemberModel();
                    RuntimeVariable.CurrentUser.MemberId = login.MemberId;
                    RuntimeVariable.CurrentUser.MemberNameTH = login.MemberNameTH;
                    RuntimeVariable.CurrentUser.MemberNameEN = login.MemberNameEN;
                    RuntimeVariable.CurrentUser.MemberLastNameTH = login.MemberLastNameTH;
                    RuntimeVariable.CurrentUser.MemberLastNameEN = login.MemberLastNameEN;
                    RuntimeVariable.CurrentUser.MemberDOB = login.MemberDOB;
                    RuntimeVariable.CurrentUser.Role = login.Role;
                    //if(Culture == "th-TH")
                    //{
                    //    RuntimeVariable.CurrentUser.MemberName = login.MemberNameTH;
                    //    RuntimeVariable.CurrentUser.MemberLastName = login.MemberLastNameTH;
                    //}
                    //else 
                    //{
                    RuntimeVariable.CurrentUser.Status = login.Status;
                    RuntimeVariable.CurrentUser.MemberName = login.MemberNameEN;
                    RuntimeVariable.CurrentUser.MemberLastName = login.MemberLastNameEN;
                    //}
                    //   return RedirectToAction("Dashboard", "Index");

                    if (login.Status == 0)
                    {
                        //login.Status = 1;
                        //RuntimeVariable.CurrentUser.Status = 1;
                        //login.Password = String.Empty;
                        //   Services.Member.ConfigAdmin(login);
                        return RedirectToAction("NewLogin", "Member");
                    }
                    return RedirectToAction("Index", "Admin");

                }
            }
            else
            {
                return RedirectToAction("Login", "Admin", new { message = "Username or Password incorrect" });

            }

            //  return RedirectToAction("Login", "Login");
        }

        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult QuestionView()
        {
            //var lstQuestion = Services.Question.GetQuestionList();
            //return View(lstQuestion);
            return View();
        }

        [Utilities.SessionAuthen]
        [HttpPost]
        public ActionResult GetQuestionList(DataTableAjaxPostModel model)
        {
            JsonResult result = new JsonResult();
            string search = Request.Form.GetValues("search[value]")[0];
            int draw = model.draw;//Request.Form.GetValues("draw")[0];
            int order = Convert.ToInt32(Request.Form.GetValues("order[0][column]")[0]);//Request.Form.GetValues("order[0][column]")[0];
            string orderDir = Request.Form.GetValues("order[0][dir]")[0];// Request.Form.GetValues("order[0][dir]")[0];
            int startRec = model.start;//Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = model.length; // Convert.ToInt32(Request.Form.GetValues("length")[0]);

            var lstQuestion = Services.Question.GetQuestionList(search, order, orderDir, startRec, pageSize);

            if (lstQuestion.Count > 0)
            {
                var TotalRec = lstQuestion.Select(i => i).FirstOrDefault();
                //if (pageSize < 0)
                //    TotalRec = 0;
                result = Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = TotalRec.TotalItem,
                    recordsFiltered = TotalRec.TotalItem,
                    data = lstQuestion
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                result = Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<QuestionModel>()
                }, JsonRequestBehavior.AllowGet);
            }
            return result;
        }

        [Utilities.SessionAuthen]
        [HttpPost]
        public ActionResult CreateNewAdmin(MemberModel member)
        {
            var response = Services.Member.CreateNewAdmin(member);

            return Json(new { status = response });
        }

        [Utilities.SessionAuthen]
        [HttpPost]
        public ActionResult ConfigAdmin(MemberModel member)
        {
            member.Username = RuntimeVariable.CurrentUser.Username;
            member.MemberId = RuntimeVariable.CurrentUser.MemberId;
            var response = Services.Member.ConfigAdmin(member);

            return Json(new { status = response });
        }

        [Utilities.SessionAuthen]
        [HttpPost]
        public ActionResult UnlockMember(string username)
        {
            StatusResponse response = new StatusResponse();
            try
            {
                response = Services.Exam.CreateNewAnswerSheet(username, RuntimeVariable.CurrentUser.MemberId);
            }
            catch
            {
                throw new Exception("error");
            }
            //throw new Exception("TEST");

            return Json(new { status = response.status });
        }


        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult EditQuestionView()
        {
            int QuestionId = 0;
            if (Request.Params["QuestionId"] == null)
            {
                if (Session["QuestionId"] == null)
                {
                    QuestionId = 0;
                }
                else
                {
                    QuestionId = Convert.ToInt32(Session["QuestionId"]);
                }
            }
            else
            {
                QuestionId = Convert.ToInt32(Request.Params["QuestionId"]);
            }

            Session["QuestionId"] = QuestionId.ToString();

            var questionOBj = Services.Question.GetQuestion(QuestionId);
            return View(questionOBj);
        }

        [Utilities.SessionAuthen]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditQuestionSubmit(QuestionModel question)
        {
            Services.Question.UpdateQuestion(question, RuntimeVariable.CurrentUser.MemberId);

            return RedirectToAction("QuestionView", "Admin");

        }


        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult MemberReport(int ExamId, int TestNo, string MemberId)
        {
            string culture = (string)RouteData.Values["culture"];
            var summary = Services.Exam.ReportScore(culture, ExamId, TestNo, MemberId);

            var stat = Services.Exam.GetMemberStat(MemberId, ExamId, TestNo);
            var max = summary.MaxBy(i => i.Score);
            var min = summary.MinBy(i => i.Score);

            var Totalsum = summary.Sum(i => i.Score);

            ViewBag.ExamId = ExamId;
            ViewBag.TestNo = TestNo;
            ViewBag.MemberId = MemberId;
            ViewBag.MaxObj = max;
            ViewBag.MinObj = min;
            ViewBag.User = stat;

            var allMean = Services.Exam.GetExamMean(culture, ExamId, Totalsum);


            ViewBag.chartList = Json(new
            {
                items = summary.Select(item => new
                {
                    name = item.FactorName,
                    values = item.Score,
                    totals = item.TotalScore
                })
            });

            ViewBag.ExamNorm = Json(new
            {
                ExamNorm = max.ExamNorm,
                ExamScore = summary.Sum(i => i.Score)
            });

            ViewBag.allMean = allMean;
            return View(summary);
            //return new RouteAsPdf ("Index", new { name = "Giorgio" }) { FileName = "Test.pdf" };
        }

        [Utilities.SessionAuthen]
        [Internationalization]
        [HttpPost]
        public ActionResult MemberReportData(int ExamId, int TestNo, string MemberId)
        {
            string culture = (string)RouteData.Values["culture"];
            var summary = Services.Exam.ReportScore(culture, ExamId, TestNo, MemberId);
            var max = summary.MaxBy(i => i.Score);
            if (summary.Count > 0)
                ViewBag.MaxObj = max;
            else
                ViewBag.MaxObj = new SummaryScoreModel();

            var result = Json(new { TotalScore = Convert.ToInt32(max.TotalScore + 12).ToString(), Norm = "", PerFactor = summary });
            return result;

        }

        [Utilities.SessionAuthen]
        [Internationalization]
        public ActionResult PrintAllReport(int ExamId, int TestNo, string MemberId, string Bar, string BarTotal, string Radar)
        {

            string culture = (string)RouteData.Values["culture"];
            var summary = Services.Exam.ReportScore(culture, ExamId, TestNo, MemberId);
            var stat = Services.Exam.GetMemberStat(MemberId, ExamId, TestNo);
            var max = summary.MaxBy(i => i.Score);
            var min = summary.MinBy(i => i.Score);

            var Totalsum = summary.Sum(i => i.Score);

            ViewBag.MinObj = min;
            ViewBag.ExamId = ExamId;
            ViewBag.TestNo = TestNo;
            ViewBag.MemberId = MemberId;
            ViewBag.MaxObj = max;
            ViewBag.User = stat;
            ViewBag.Bar = Bar;
            ViewBag.BarTotal = BarTotal;
            ViewBag.Radar = Radar;

            var allMean = Services.Exam.GetExamMean(culture, ExamId, Totalsum);

            ViewBag.chartList = Json(new
            {
                items = summary.Select(item => new
                {
                    name = item.FactorName,
                    values = item.Score,
                    totals = item.TotalScore
                })
            });

            var report = new ViewAsPdf(summary)
            {
                FileName = MemberId + "_report_" + TestNo + ".pdf",
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A4
            };
            ViewBag.allMean = allMean;

            return report;
        }


        [Utilities.SessionAuthen]
        [HttpPost]
        public ActionResult UpdateNewPassword(string username, string newpassword)
        {

            ///update new password
            ///

            string response = "";

            var result = Services.Member.UpdateNewPassword(RuntimeVariable.CurrentUser.MemberId, newpassword);
            if (result > 0) response = "Success";
            else response = "Fail";


            return Json(new
            {
                result = response
            });
        }
    }
}
