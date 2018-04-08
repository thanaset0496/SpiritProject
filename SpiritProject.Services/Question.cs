using SpiritProject.Common;
using SpiritProject.DBUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Services
{
    public static class Question
    {
        public static List<QuestionModel> GetQuestionList(string search, int order, string orderDir, int startRec, int pageSize)
        {
            List<QuestionModel> lstQuestion = new List<QuestionModel>();
            List<QuestionModel> lstReturn = new List<QuestionModel>();
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    order = order > 0 ? order + 1 : 1;
                    orderDir = String.IsNullOrEmpty(orderDir) ? "asc " : orderDir;
                    search = String.IsNullOrEmpty(search) ? "" : search;
                    ///step 1 exitsed member ?
                    StringBuilder sql = new StringBuilder();
                    sql.AppendFormat(@" SELECT q.QuestionId,
                                               q.DataTH,
                                               q.DataEN,
                                               f.FactorNameTH,
                                               f.FactorNameEN,
                                               sub.SubFactorNameTH,
                                               sub.SubFactorNameEN,
                                               Convert(nvarchar(16),q.UpdatedDate,103) AS UpdatedDate,
                                               Count(q.QuestionId) OVER()  AS TotalItem
                                        FROM dbo.TB_Question q
                                            INNER JOIN TB_Factor f
                                                ON f.FactorId = q.FactorId
                                            LEFT JOIN dbo.TB_SubFactor sub
                                                ON sub.SubFactorId = q.SubFactorId
                                                   AND sub.FactorId = f.FactorId
                                        WHERE q.QuestionStatus = 'A'  and
                                     (
                                          q.QuestionId LIKE '%{4}%'
                                          OR q.DataTH LIKE '%{4}%'
                                          OR q.DataEN LIKE '%{4}%'
                                          OR f.FactorNameTH LIKE '%{4}%'
                                          OR f.FactorNameEN LIKE '%{4}%'
                                          OR sub.SubFactorNameTH LIKE '%{4}%'
                                          OR sub.SubFactorNameEN LIKE '%{4}%'
                                          OR CONVERT(NVARCHAR(10),q.UpdatedDate,103) LIKE '%{4}%'
                                      )
                                    ORDER BY {0} {1} 
                                    OFFSET {2} ROWS FETCH NEXT  {3} ROWS ONLY
                                        ", order, orderDir, startRec, pageSize, search);

                    lstQuestion = context.Database.SqlQuery<QuestionModel>(sql.ToString()).ToList();

                    foreach (var item in lstQuestion)
                    {
                        var newObj = new QuestionModel();

                        newObj.QuestionId = item.QuestionId;
                        newObj.DataTH = Services.Hash.Decrypt(item.DataTH, true);
                        newObj.DataEN = Services.Hash.Decrypt(item.DataEN, true);
                        newObj.FactorNameTH = item.FactorNameTH;
                        newObj.FactorNameEN = item.FactorNameEN;
                        newObj.SubFactorNameTH = item.SubFactorNameTH;
                        newObj.SubFactorNameEN = item.SubFactorNameEN;
                        newObj.UpdatedDate = item.UpdatedDate;
                        newObj.TotalItem = item.TotalItem;


                        lstReturn.Add(newObj);
                    }
                    if (lstQuestion.Count > 0)
                    {

                    }
                    else
                    {

                    }
                }
            }
            catch
            {

            }


            return lstReturn;

        }

        public static QuestionModel GetQuestion(int QuestionId)
        {
            QuestionModel question = new QuestionModel();
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    ///step 1 exitsed member ?
                    StringBuilder sql = new StringBuilder();
                    sql.AppendFormat(@" SELECT q.QuestionId,
                                               q.DataTH,
                                               q.DataEN,
                                               f.FactorNameTH,
                                               f.FactorNameEN,
                                               sub.SubFactorNameTH,
                                               sub.SubFactorNameEN,
                                               Convert(nvarchar(16),q.UpdatedDate,103) AS UpdatedDate
                                        FROM dbo.TB_Question q
                                            INNER JOIN TB_Factor f
                                                ON f.FactorId = q.FactorId
                                            LEFT JOIN dbo.TB_SubFactor sub
                                                ON sub.SubFactorId = q.SubFactorId
                                                   AND sub.FactorId = f.FactorId
                                        WHERE q.QuestionStatus = 'A'
                                        AND QuestionId = {0}", QuestionId);

                    question = context.Database.SqlQuery<QuestionModel>(sql.ToString()).FirstOrDefault();


                    question.DataTH = Services.Hash.Decrypt(question.DataTH, true);
                    question.DataEN = Services.Hash.Decrypt(question.DataEN, true);

                    if (question == null)
                    {

                    }
                    else
                    {
                        GetChoice(ref question);
                    }
                }
            }
            catch
            {

            }


            return question;

        }

        public static QuestionModel GetChoice(ref QuestionModel _question)
        {
            using (SpiritContext context = new SpiritContext())
            {

                var lstChoice = new List<ChoiceModel>();

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" 
                                         SELECT c.ChoiceId,
                                                    c.QuestionId,
                                                    c.Point,
                                                    c.ChoiceType,
                                                c.ChoiceNameTH,
                                                c.ChoiceNameEN
                                             FROM dbo.TB_Question q
                                            INNER JOIN dbo.TB_ChoiceWG c
                                            ON q.QuestionId = c.QuestionId
                                             WHERE q.QuestionId = {0}  ", _question.QuestionId);

                lstChoice = context.Database.SqlQuery<ChoiceModel>(sql.ToString()).ToList<ChoiceModel>();
                foreach (var choice in lstChoice)
                {
                    choice.ChoiceNameTH = Hash.Decrypt(choice.ChoiceNameTH, true);
                    choice.ChoiceNameEN = Hash.Decrypt(choice.ChoiceNameEN, true);
                }

                _question.Choice = lstChoice;

            }

            return _question;
        }

        public static StatusResponse UpdateQuestion(QuestionModel _question, string CurrentMember)
        {
            StatusResponse response = new StatusResponse();
            using (SpiritContext context = new SpiritContext())
            {
                _question.DataTH = Hash.Encrypt(_question.DataTH, true);
                _question.DataEN = Hash.Encrypt(_question.DataEN, true);

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"  update TB_Question set
											 DataTH = '{0}', DataEN='{1}'
											 ,UpdatedDate = GETDATE()
											 ,UpdatedBy ='{2}'
											 where QuestionId = '{3}'  ", _question.DataTH, _question.DataEN, CurrentMember, _question.QuestionId);

                response.status = context.Database.ExecuteSqlCommand(sql.ToString());

                response = UpdateChoice(context, _question, CurrentMember);
            }

            return response;
        }


        public static StatusResponse UpdateChoice(SpiritContext context, QuestionModel _question, string CurrentUser)
        {
            StatusResponse response = new StatusResponse();
            foreach (var choice in _question.Choice)
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" 
                                         update TB_ChoiceWG set
											 ChoiceNameTH = '{0}', ChoiceNameEN='{1}'
											 ,UpdatedDate = GETDATE()
											 ,UpdatedBy ='{2}'
											 where ChoiceId = '{3}' AND QuestionId = '{4}' ", Hash.Encrypt(choice.ChoiceNameTH, true), Hash.Encrypt(choice.ChoiceNameEN, true), CurrentUser, choice.ChoiceId, _question.QuestionId);
                response.status = context.Database.ExecuteSqlCommand(sql.ToString());

            }

            return response;
        }
    }
}
