using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiritProject.Common;
using SpiritProject.DBUtils;

namespace SpiritProject.Services
{
    public class Exam
    {
        public ExamStatModel GetExamStat(string memberId, int testNo)
        {
            var stat = new ExamStatModel();
            using (SpiritContext context = new SpiritContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" SELECT [MemberId],
                                           [TestNo],
                                           stat.ExamId,
                                           [ScoreSet],
                                           [ScoreGet],
                                           [TimeUsed],
                                           (TimeSet * 60) AS TimeSet,
                                           [StartDate],
                                           [FinishedDate],
                                           [PageNo],
                                           [TestStatus],exam.TotalQuestion,
                                         (COALESCE(exam.remind1,0) ) AS Remind1,
                                           (COALESCE(exam.remind2,0) ) AS Remind2
                                    FROM [TB_ExamStat] stat
                                    INNER JOIN dbo.TB_Exam exam ON exam.ExamId = stat.ExamId
                                    WHERE MemberId = '{0}' 
                                    AND TestNo = '{1}'
                                    AND TestStatus in ('0','1') ", memberId, testNo);

                stat = context.Database.SqlQuery<ExamStatModel>(sql.ToString()).FirstOrDefault();
            }


            return stat;
        }

        public static StatusResponse ExpiredExamCommit(ExamStatModel exam)
        {
            StatusResponse response = new StatusResponse();
            using (SpiritContext context = new SpiritContext())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" IF EXISTS
                                    (
                                        SELECT *
                                        FROM dbo.TB_ExamStat
                                        WHERE MemberId = '{1}'
                                              AND ExamId = {0}
                                              AND TestNo = {2}
                                    )
                                    BEGIN
                                        UPDATE stat
                                        SET stat.ExamAccessTimes += 1,
                                            stat.FinishedDate = GETDATE(),
                                            stat.TestStatus = 2
                                        FROM TB_ExamStat stat
                                        WHERE ExamId = {0}
                                              AND MemberId = '{1}'
                                              AND TestNo = '{2}';
                                    END;
                                ", exam.ExamId, exam.MemberId, exam.TestNo);

                response.status = context.Database.ExecuteSqlCommand(sql.ToString());

            }

            return response;
        }

        public List<AnswerSheetModel> GetExam(string MemnerId, int ExamId, int TestNo, string culture)
        {
            var lstQuest = new List<AnswerSheetModel>();
            using (SpiritContext context = new SpiritContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"  DECLARE @source NVARCHAR(2) = '{3}';
                                     SELECT sheet.*,     
                                     CASE WHEN @source = 'th' THEN q.DataTH
	                                       WHEN @source ='en' THEN q.DataEN
	                                       END AS QuestionName 
	                                       FROM dbo.TB_AnswerSheet sheet
                                     INNER JOIN dbo.TB_Question q
                                     ON q.QuestionId = sheet.QuestionId
                                     WHERE sheet.MemberId = '{0}'
                                     AND sheet.ExamId = {1}
                                     AND sheet.TestNo = {2}
                                     AND sheet.ChoiceId IS NULL 
                                     ORDER BY sheet.QuestionOrder asc", MemnerId, ExamId, TestNo, culture);

                lstQuest = context.Database.SqlQuery<AnswerSheetModel>(sql.ToString()).ToList<AnswerSheetModel>();
            }

            if (lstQuest.Count > 0)
            {
                GetChoice(ref lstQuest, culture);
            }

            return lstQuest;
        }

    
        public List<AnswerSheetModel> GetChoice(ref List<AnswerSheetModel> lstQuest, string culture)
        {
            using (SpiritContext context = new SpiritContext())
            {
                foreach (var q in lstQuest)
                {
                    q.QuestionName = Hash.Decrypt(q.QuestionName, true);

                    var lstChoice = new List<ChoiceModel>();

                    StringBuilder sql = new StringBuilder();
                    sql.AppendFormat(@" DECLARE @source NVARCHAR(2) = '{2}';

                                        SELECT *
                                        FROM
                                        (
                                            (SELECT c.ChoiceId,
                                                    c.QuestionId,
                                                    c.Point,
                                                    c.ChoiceType,
                                                    CASE
                                                        WHEN @source = 'th' THEN
                                                            c.ChoiceNameTH
                                                        WHEN @source = 'en' THEN
                                                            c.ChoiceNameEN
                                                    END AS ChoiceName
                                             FROM dbo.TB_Question q
                                                 INNER JOIN dbo.TB_ChoiceWG c
                                                     ON q.QuestionId = c.QuestionId
                                             WHERE q.QuestionId = {0} )
                                            UNION
                                            (SELECT 
                                                    f.FakeChoiceId AS ChoiceId,
                                                    0,
                                                    f.Point,
                                                    f.FakeChoiceType AS ChoiceType,
                                                    CASE
                                                        WHEN @source = 'th' THEN
                                                            f.FakeChoiceNameTH
                                                        WHEN @source = 'en' THEN
                                                            f.FakeChoiceNameEN
                                                    END AS ChoiceName
                                             FROM dbo.TB_FakeChoice f
                                             WHERE f.FakeChoiceId = {1})
                                        ) _choice
                                        ORDER BY NEWID() ", q.QuestionId, q.QuestionId, culture);

                    lstChoice = context.Database.SqlQuery<ChoiceModel>(sql.ToString()).ToList<ChoiceModel>();
                    foreach (var choice in lstChoice)
                    {
                        choice.ChoiceName = Hash.Decrypt(choice.ChoiceName, true);

                    }

                    q.Choice = lstChoice;
                }
            }

            return lstQuest;
        }

        public StatusResponse SaveAnswer(string memberId, int questionId, int questionOrder, int choiceId, string choiceType, int point, decimal timeUsed, int examId, int IsFinal, int testNo)
        {
            StatusResponse status = new StatusResponse();
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    // define a new output parameter
                    //var returnCode = new System.Data.Entity.Core.Objects.ObjectParameter("Result", typeof(int));
                    //var TotalScore = new System.Data.Entity.Core.Objects.ObjectParameter("TotalScore", typeof(int));

                    var returnCode = new SqlParameter("Result", SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };

                    var TotalScore = new SqlParameter("TotalScore", SqlDbType.Decimal)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    // var  = 0;// new System.Data.Entity.Core.Objects.ObjectParameter("TotalScore", typeof(int));
                    //returnCode.ParameterName = "@TotalScore";
                    //returnCode.SqlDbType = SqlDbType.Int;
                    //returnCode.Direction = ParameterDirection.Output;

                    //// assign the return code to the new output parameter and pass it to the sp
                    var data = context.Database.ExecuteSqlCommand(@"
                        SP_QIUD_EXAM @Result out,
                                            @TotalScore out,
                                            @MemberId,
                                            @QuestionId,
                                            @QuestionOrder,
                                            @ChoiceId,
                                            @ChoiceType,
                                            @Point,
                                            @TimeUsed,
                                            @ExamId,
                                            @Action,
                                            @IsFinal,
                                            @TestNo",
                                            returnCode,
                                            TotalScore,
                                            new SqlParameter("MemberId", memberId),
                                            new SqlParameter("QuestionId", questionId),
                                            new SqlParameter("QuestionOrder", questionOrder),
                                            new SqlParameter("ChoiceId", choiceId),
                                            new SqlParameter("ChoiceType", choiceType),
                                            new SqlParameter("Point", point),
                                            new SqlParameter("TimeUsed", timeUsed),
                                            new SqlParameter("ExamId", examId),
                                           new SqlParameter("Action", "U"),
                                            new SqlParameter("IsFinal", IsFinal),
                                           new SqlParameter("TestNo", testNo));
                    status.status = (int)returnCode.Value;
                    status.TotalScore = (decimal)TotalScore.Value;
                    //    var res = context.SP_QIUD_EXAM(returnCode, TotalScore, memberId, questionId, questionOrder, choiceId, choiceType, point, timeUsed, examId, "U", IsFinal, testNo);
                    //      status.status = Convert.ToInt32(returnCode.Value);
                }
            }
            catch
            {

            }
            return status;


        }


        public StatusResponse AutoSaveAnswer(string memberId, int questionId, int questionOrder, int? choiceId, string choiceType, int point, decimal timeUsed, int examId, int IsFinal, int testNo)
        {
            StatusResponse status = new StatusResponse();
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    // define a new output parameter
                    //var returnCode = new System.Data.Entity.Core.Objects.ObjectParameter("Result", typeof(int));
                    //var TotalScore = new System.Data.Entity.Core.Objects.ObjectParameter("TotalScore", typeof(int));

                    var returnCode = new SqlParameter("Result", SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };

                    var TotalScore = new SqlParameter("TotalScore", SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };
                    // var  = 0;// new System.Data.Entity.Core.Objects.ObjectParameter("TotalScore", typeof(int));
                    //returnCode.ParameterName = "@TotalScore";
                    //returnCode.SqlDbType = SqlDbType.Int;
                    //returnCode.Direction = ParameterDirection.Output;

                    //// assign the return code to the new output parameter and pass it to the sp
                    var data = context.Database.ExecuteSqlCommand(@"
                        SP_QIUD_EXAM @Result out,
                                            @TotalScore out,
                                            @MemberId,
                                            @QuestionId,
                                            @QuestionOrder,
                                            @ChoiceId,
                                            @ChoiceType,
                                            @Point,
                                            @TimeUsed,
                                            @ExamId,
                                            @Action,
                                            @IsFinal,
                                            @TestNo",
                                            returnCode,
                                            TotalScore,
                                            new SqlParameter("MemberId", memberId),
                                            new SqlParameter("QuestionId", questionId),
                                            new SqlParameter("QuestionOrder", questionOrder),
                                            new SqlParameter("ChoiceId", DBNull.Value),
                                            new SqlParameter("ChoiceType", choiceType),
                                            new SqlParameter("Point", point),
                                            new SqlParameter("TimeUsed", timeUsed),
                                            new SqlParameter("ExamId", examId),
                                           new SqlParameter("Action", "U"),
                                            new SqlParameter("IsFinal", IsFinal),
                                           new SqlParameter("TestNo", testNo));
                    status.status = (int)returnCode.Value;
                    status.TotalScore = (int)TotalScore.Value;
                    //    var res = context.SP_QIUD_EXAM(returnCode, TotalScore, memberId, questionId, questionOrder, choiceId, choiceType, point, timeUsed, examId, "U", IsFinal, testNo);
                    //      status.status = Convert.ToInt32(returnCode.Value);
                }
            }
            catch
            {

            }
            return status;


        }

        public static StatusResponse CreateNewAnswerSheet(string memberId, string CreatedByMemberId)
        {
            StatusResponse status = new StatusResponse();
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    var returnCode = new SqlParameter("Result", SqlDbType.Int)
                    {
                        Direction = System.Data.ParameterDirection.Output
                    };

                    //// assign the return code to the new output parameter and pass it to the sp
                    var data = context.Database.ExecuteSqlCommand(@"
                        SP_I_AnswerSheet @Result out,
                                           @MemberId,
                                            @CreatedByMemberId",
                                            returnCode,
                                            new SqlParameter("MemberId", memberId),
                                            new SqlParameter("CreatedByMemberId", CreatedByMemberId));
                    status.status = (int)returnCode.Value;
                }
            }
            catch(Exception ex)
            {
                var sg = ex.Message;
                throw;
            }
            return status;


        }

        public static List<ExamStatModel> GetExamList(string MemberId)
        {
            List<ExamStatModel> lstExam = new List<ExamStatModel>();
            using (SpiritContext context = new SpiritContext())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" SELECT (SELECT COUNT(DISTINCT QuestionId) FROM dbo.TB_AnswerSheet ans
                                                WHERE ExamId = stat.ExamId AND ans.MemberId=stat.MemberId
                                                AND ChoiceId IS NOT NULL AND ans.TestNo = stat.TestNo 
                                            ) AS Answered,
                                    stat.ExamId,exam.ExamDesc,TotalQuestion,TestNo,TestStatus AS Status,mem.MemberNameTH+' '+mem.MemberLastNameTH AS FullName,
                                    StartDate,FinishedDate,stat.TimeSet,
	                                CASE
                                           WHEN DATEDIFF(d, stat.StartDate, GETDATE()) > 30 THEN
                                               DATEDIFF(d, stat.StartDate, GETDATE()) / 30.0
                                           ELSE
                                               0
                                       END AS 'MonthDifference'
                                    FROM dbo.TB_ExamStat stat
                                    INNER JOIN dbo.TB_Exam exam
                                    ON exam.ExamId = stat.ExamId
                                    INNER JOIN dbo.TB_Member mem
                                    ON stat.MemberId = mem.MemberId
                                    WHERE stat.MemberId = '{0}'
                                   -- AND stat.TestStatus = 'A'  
                                      AND stat.ExamId IN (
	                                      SELECT ExamId FROM dbo.TB_AnswerSheet ans
                                            WHERE  ans.MemberId = stat.MemberId
                                               AND ans.ExamId = exam.ExamId
                                               AND ans.TestNo = stat.TestNo
	                                      )
                                    ORDER BY TestNo Desc 
                                        ", MemberId);

                lstExam = context.Database.SqlQuery<ExamStatModel>(sql.ToString()).ToList();

            }

            return lstExam;
        }


        public static ExamStatModel GetMemberStat(string MemberId, int ExamId, int TestNo)
        {
            ExamStatModel lstExam = new ExamStatModel();
            using (SpiritContext context = new SpiritContext())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" DECLARE @source NVARCHAR(2) = 'th';
                                    SELECT stat.TimeUsed / 60 AS minute,stat.TimeSet,
                                           stat.TimeUsed % 60 AS second,
	                                       stat.StartDate,
                                           CASE
                                               WHEN @source = 'th' THEN
                                                   mem.MemberNameTH + ' ' + mem.MemberLastNameTH
                                               WHEN @source = 'en' THEN
                                                   mem.MemberNameEN + ' ' + mem.MemberLastNameEN
                                           END AS MemberName,stat.TestNo,stat.MemberId,
                                            COALESCE(stat.ExamAccessTimes,0) AS ExamAccessTimes
                                    FROM dbo.TB_ExamStat stat
                                        INNER JOIN dbo.TB_Member mem
                                            ON mem.MemberId = stat.MemberId
                                    WHERE stat.MemberId = '{0}'
                                    AND stat.ExamId = {1}
                                    AND stat.TestNo ={2}
                                    ORDER BY StartDate DESC
                                        ", MemberId, ExamId, TestNo);

                lstExam = context.Database.SqlQuery<ExamStatModel>(sql.ToString()).FirstOrDefault();

            }

            return lstExam;
        }


        public static List<ExamStatModel> GetExamHist(string search, int order, string orderDir, int startRec, int pageSize)
        {
            List<ExamStatModel> lstExam = new List<ExamStatModel>();
            using (SpiritContext context = new SpiritContext())
            {
                order = order > 0 ? order + 1 : 5;
                orderDir = String.IsNullOrEmpty(orderDir) ? "desc " : orderDir;
                search = String.IsNullOrEmpty(search) ? "" : search;
                // startRec = String.IsNullOrEmpty(startRec) ? "" : "And " + orderDir;

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"  WITH rows AS ( SELECT (SELECT COUNT(DISTINCT QuestionId) FROM dbo.TB_AnswerSheet ans
                                               WHERE ExamId = stat.ExamId AND ans.MemberId=stat.MemberId
                                                AND ChoiceId IS NOT NULL AND ans.TestNo = stat.TestNo 
                                            ) AS Answered,COUNT(*) OVER()  AS TotalItem,
                                    stat.ExamId,exam.ExamDesc,TotalQuestion,TestNo,TestStatus AS Status,
                                    mem.MemberNameTH+' '+mem.MemberLastNameTH AS FullName,stat.MemberId,stat.StartDate,
									(SELECT COALESCE(SUM(Point),0) FROM dbo.TB_AnswerSheet ans
                                               WHERE ExamId = stat.ExamId AND ans.MemberId=stat.MemberId
                                                AND ChoiceId IS NOT NULL AND ans.TestNo = stat.TestNo 
                                            ) AS TotalScore
                                    FROM dbo.TB_ExamStat stat
                                    INNER JOIN dbo.TB_Exam exam
                                    ON exam.ExamId = stat.ExamId
                                    INNER JOIN dbo.TB_Member mem
                                    ON stat.MemberId = mem.MemberId
                                    WHERE 
                                     stat.TestStatus = '2'  AND
                                       stat.ExamId IN (
	                                      SELECT ExamId FROM dbo.TB_AnswerSheet ans
                                            WHERE  ans.MemberId = stat.MemberId
                                               AND ans.ExamId = exam.ExamId
                                               AND ans.TestNo = stat.TestNo
	                                      ))
                                SELECT rows.MemberId,FullName,ExamDesc,TotalScore,rows.TestNo,StartDate,TotalQuestion,rows.ExamId,Answered,TotalItem,Status,
								fac1.FactorShortName as FactorShortName1,fac1.Score as ScoreFac1,fac1.Norm as Norm1,
								fac2.FactorShortName as FactorShortName2,fac3.Score as ScoreFac2,fac2.Norm as Norm2,
								fac3.FactorShortName as FactorShortName3,fac4.Score as ScoreFac3,fac3.Norm as Norm3,
								fac4.FactorShortName as FactorShortName4,fac4.Score as ScoreFac4,fac4.Norm as Norm4,
								fac5.FactorShortName as FactorShortName5,fac5.Score as ScoreFac5,fac5.Norm as Norm5,
								fac6.FactorShortName as FactorShortName6,fac6.Score as ScoreFac6,fac6.Norm as Norm6
                                FROM rows
								inner join (SELECT stat.MemberId,stat.TestNo,stat.ExamId,f.FactorId,SUBSTRING(f.FactorNameEN,1,3) AS FactorShortName,
                                               SUM(COALESCE(ans.Point, 0)) Score,COUNT(ans.ChoiceId) AS DoneQuestion,Norm
                                        FROM dbo.TB_AnswerSheet ans
                                            INNER JOIN dbo.TB_ExamStat stat
                                            ON stat.ExamId = ans.ExamId AND stat.MemberId = ans.MemberId AND stat.TestNo = ans.TestNo
                                            INNER JOIN dbo.TB_Question q
                                            ON q.QuestionId = ans.QuestionId
                                            INNER JOIN dbo.TB_Factor f
                                            ON f.FactorId = q.FactorId
                                            INNER JOIN TB_FactorNorm fn 
											on fn.Fid = f.FactorId
											and Status= 'A'
                                             where stat.TestStatus = 2 and f.FactorId=1
                                        GROUP BY stat.MemberId,f.FactorId,stat.TestNo,
										stat.ExamId,f.FactorNameTH,fn.Norm,f.FactorNameEN
                                                    )
													fac1 on fac1.MemberId = rows.MemberId
													and fac1.TestNo = rows.TestNo
													and fac1.ExamId = rows.ExamId
								inner join (SELECT stat.MemberId,stat.TestNo,stat.ExamId,f.FactorId,SUBSTRING(f.FactorNameEN,1,3) AS FactorShortName,
                                               SUM(COALESCE(ans.Point, 0)) Score,COUNT(ans.ChoiceId) AS DoneQuestion,Norm
                                        FROM dbo.TB_AnswerSheet ans
                                            INNER JOIN dbo.TB_ExamStat stat
                                            ON stat.ExamId = ans.ExamId AND stat.MemberId = ans.MemberId AND stat.TestNo = ans.TestNo
                                            INNER JOIN dbo.TB_Question q
                                            ON q.QuestionId = ans.QuestionId
                                            INNER JOIN dbo.TB_Factor f
                                            ON f.FactorId = q.FactorId
                                            INNER JOIN TB_FactorNorm fn 
											on fn.Fid = f.FactorId
											and Status= 'A'
                                             where stat.TestStatus = 2 and f.FactorId=2
                                        GROUP BY stat.MemberId,f.FactorId,stat.TestNo,
										stat.ExamId,f.FactorNameTH,fn.Norm,f.FactorNameEN
                                                    )
													fac2 on fac2.MemberId = rows.MemberId
													and fac2.TestNo = rows.TestNo
													and fac2.ExamId = rows.ExamId
								inner join (SELECT stat.MemberId,stat.TestNo,stat.ExamId,f.FactorId,SUBSTRING(f.FactorNameEN,1,3) AS FactorShortName,
                                               SUM(COALESCE(ans.Point, 0)) Score,COUNT(ans.ChoiceId) AS DoneQuestion,Norm
                                        FROM dbo.TB_AnswerSheet ans
                                            INNER JOIN dbo.TB_ExamStat stat
                                            ON stat.ExamId = ans.ExamId AND stat.MemberId = ans.MemberId AND stat.TestNo = ans.TestNo
                                            INNER JOIN dbo.TB_Question q
                                            ON q.QuestionId = ans.QuestionId
                                            INNER JOIN dbo.TB_Factor f
                                            ON f.FactorId = q.FactorId
                                            INNER JOIN TB_FactorNorm fn 
											on fn.Fid = f.FactorId
											and Status= 'A'
                                             where stat.TestStatus = 2 and f.FactorId=3
                                        GROUP BY stat.MemberId,f.FactorId,stat.TestNo,
										stat.ExamId,f.FactorNameTH,fn.Norm,f.FactorNameEN
                                                    )
													fac3 on fac3.MemberId = rows.MemberId
													and fac3.TestNo = rows.TestNo
													and fac3.ExamId = rows.ExamId
								inner join (SELECT stat.MemberId,stat.TestNo,stat.ExamId,f.FactorId,SUBSTRING(f.FactorNameEN,1,3) AS FactorShortName,
                                               SUM(COALESCE(ans.Point, 0)) Score,COUNT(ans.ChoiceId) AS DoneQuestion,Norm
                                        FROM dbo.TB_AnswerSheet ans
                                            INNER JOIN dbo.TB_ExamStat stat
                                            ON stat.ExamId = ans.ExamId AND stat.MemberId = ans.MemberId AND stat.TestNo = ans.TestNo
                                            INNER JOIN dbo.TB_Question q
                                            ON q.QuestionId = ans.QuestionId
                                            INNER JOIN dbo.TB_Factor f
                                            ON f.FactorId = q.FactorId
                                            INNER JOIN TB_FactorNorm fn 
											on fn.Fid = f.FactorId
											and Status= 'A'
                                             where stat.TestStatus = 2 and f.FactorId=4
                                        GROUP BY stat.MemberId,f.FactorId,stat.TestNo,
										stat.ExamId,f.FactorNameTH,fn.Norm,f.FactorNameEN
                                                    )
													fac4 on fac4.MemberId = rows.MemberId
													and fac4.TestNo = rows.TestNo
													and fac4.ExamId = rows.ExamId
								inner join (SELECT stat.MemberId,stat.TestNo,stat.ExamId,f.FactorId,SUBSTRING(f.FactorNameEN,1,3) AS FactorShortName,
                                               SUM(COALESCE(ans.Point, 0)) Score,COUNT(ans.ChoiceId) AS DoneQuestion,Norm
                                        FROM dbo.TB_AnswerSheet ans
                                            INNER JOIN dbo.TB_ExamStat stat
                                            ON stat.ExamId = ans.ExamId AND stat.MemberId = ans.MemberId AND stat.TestNo = ans.TestNo
                                            INNER JOIN dbo.TB_Question q
                                            ON q.QuestionId = ans.QuestionId
                                            INNER JOIN dbo.TB_Factor f
                                            ON f.FactorId = q.FactorId
                                            INNER JOIN TB_FactorNorm fn 
											on fn.Fid = f.FactorId
											and Status= 'A'
                                             where stat.TestStatus = 2 and f.FactorId=5
                                        GROUP BY stat.MemberId,f.FactorId,stat.TestNo,
										stat.ExamId,f.FactorNameTH,fn.Norm,f.FactorNameEN
                                                    )
													fac5 on fac5.MemberId = rows.MemberId
													and fac5.TestNo = rows.TestNo
													and fac5.ExamId = rows.ExamId
								inner join (SELECT stat.MemberId,stat.TestNo,stat.ExamId,f.FactorId,SUBSTRING(f.FactorNameEN,1,3) AS FactorShortName,
                                               SUM(COALESCE(ans.Point, 0)) Score,COUNT(ans.ChoiceId) AS DoneQuestion,Norm
                                        FROM dbo.TB_AnswerSheet ans
                                            INNER JOIN dbo.TB_ExamStat stat
                                            ON stat.ExamId = ans.ExamId AND stat.MemberId = ans.MemberId AND stat.TestNo = ans.TestNo
                                            INNER JOIN dbo.TB_Question q
                                            ON q.QuestionId = ans.QuestionId
                                            INNER JOIN dbo.TB_Factor f
                                            ON f.FactorId = q.FactorId
                                            INNER JOIN TB_FactorNorm fn 
											on fn.Fid = f.FactorId
											and Status= 'A'
                                             where stat.TestStatus = 2 and f.FactorId=6
                                        GROUP BY stat.MemberId,f.FactorId,stat.TestNo,
										stat.ExamId,f.FactorNameTH,fn.Norm,f.FactorNameEN
                                                    )
													fac6 on fac6.MemberId = rows.MemberId
													and fac6.TestNo = rows.TestNo
													and fac6.ExamId = rows.ExamId
                                WHERE (
                                          Answered LIKE '%{4}%'
                                          OR TotalItem LIKE '%{4}%'
                                          OR rows.ExamId LIKE '%{4}%'
                                          OR ExamDesc LIKE '%{4}%'
                                          OR TotalQuestion LIKE '%{4}%'
                                          OR rows.TestNo LIKE '%{4}%'
                                          OR Status LIKE '%{4}%'
                                          OR FullName LIKE '%{4}%'
                                          OR rows.MemberId LIKE '%{4}%'
                                          OR CONVERT(NVARCHAR(10),StartDate,103) LIKE '%{4}%'
                                      )
                                     
                                    ORDER BY {0} {1} 
                                    OFFSET {2} ROWS FETCH NEXT  {3} ROWS ONLY
                                        ", order, orderDir, startRec, pageSize, search);

                lstExam = context.Database.SqlQuery<ExamStatModel>(sql.ToString()).ToList();

            }

            return lstExam;
        }

        public static StatusResponse StampAccessTimes(int ExamId, string MemberId, int TestNo)
        {
            StatusResponse response = new StatusResponse();
            using (SpiritContext context = new SpiritContext())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" IF EXISTS
                                    (
                                        SELECT *
                                        FROM dbo.TB_ExamStat
                                        WHERE MemberId = '{1}'
                                              AND ExamId = {0}
                                              AND TestNo = {2}
                                              AND StartDate IS NULL
                                    )
                                    BEGIN
                                        UPDATE stat
                                        SET stat.ExamAccessTimes += 1,
                                            stat.StartDate = GETDATE(),
                                            stat.TestStatus = 1
                                        FROM TB_ExamStat stat
                                        WHERE ExamId = {0}
                                              AND MemberId = '{1}'
                                              AND TestNo = '{2}';
                                    END;
                                    ELSE
                                    BEGIN
                                        UPDATE stat
                                        SET stat.ExamAccessTimes += 1
                                        FROM TB_ExamStat stat
                                        WHERE ExamId = {0}
                                              AND MemberId = '{1}'
                                              AND TestNo = '{2}';
                                    END;

                                ", ExamId, MemberId, TestNo);

                response.status = context.Database.ExecuteSqlCommand(sql.ToString());

            }

            return response;
        }

        public static string GetExamMean(string culture, int ExamId, decimal TotalScore)
        {
            string result = "";
            using (SpiritContext context = new SpiritContext())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" DECLARE @source NVARCHAR(2) = '{0}';
                                   WITH rows
                                    AS (SELECT 
                                               CASE
                                                   WHEN @source = 'th' THEN
                                                       score.Mean_TH
                                                   WHEN @source = 'en' THEN
                                                       score.Mean_EN
                                               END AS MeanName
                                                From TB_ExamScore score
                                                Where ExamId = {1} and {2} between score.SMin  and score.SMax  )
                                                select MeanName from rows
                                           ", culture, ExamId, TotalScore);

                result = context.Database.SqlQuery<string>(sql.ToString()).FirstOrDefault();
            }
            return result;

        }


        public static List<SummaryScoreModel> ReportScore(string culture, int ExamId, int TestNo, string MemberId)
        {
            List<SummaryScoreModel> lstSummary = new List<SummaryScoreModel>();
            using (SpiritContext context = new SpiritContext())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" DECLARE @source NVARCHAR(2) = '{0}';
                                   WITH rows
                                    AS (SELECT f.FactorId,
                                               CASE
                                                   WHEN @source = 'th' THEN
                                                       f.FactorNameTH
                                                   WHEN @source = 'en' THEN
                                                       f.FactorNameEN
                                               END AS FactorName,
                                                SUBSTRING(f.FactorNameEN,1,3) AS FactorShortName,
                                               SUM(COALESCE(ans.Point, 0)) Score,
                                               COUNT(ans.ChoiceId) AS DoneQuestion,
                                              COUNT(q.QuestionId) AS TotalQuestion,
                                            CONVERT(DECIMAL(10,2),fn.Norm) AS TotalScore,
                                           -- CONVERT(DECIMAL(10,0),COUNT(q.QuestionId)) AS TotalScore,
                                               exam.FullName as ExamFullName,
                                           CONVERT(CHAR(8), DATEADD(SECOND, SUM(ans.TimeUsed), ''), 114) AS TimeUsed,
                                           CONVERT(CHAR(8), DATEADD(SECOND, stat.TimeUsed, ''), 114) AS TotalTimeUsed,
                                            CONVERT(NVARCHAR(24),stat.FinishedDate,121) AS FinishedDate,
											en.Norm AS ExamNorm
                                        FROM dbo.TB_AnswerSheet ans
                                            INNER JOIN dbo.TB_Exam exam
                                                ON exam.ExamId = ans.ExamId
                                            INNER JOIN dbo.TB_ExamStat stat
                                                ON stat.ExamId = ans.ExamId
                                                   AND stat.MemberId = ans.MemberId
                                                   AND stat.TestNo = ans.TestNo
                                            INNER JOIN dbo.TB_Question q
                                                ON q.QuestionId = ans.QuestionId
                                            INNER JOIN dbo.TB_Factor f
                                                ON f.FactorId = q.FactorId
                                            LEFT JOIN dbo.TB_SubFactor sub
                                                ON sub.FactorId = q.FactorId
                                                   AND sub.SubFactorId = q.SubFactorId
                                            INNER JOIN TB_FactorNorm fn 
											on fn.Fid = f.FactorId
											and Status= 'A'
												INNER JOIN dbo.TB_ExamNorm en
                                                ON en.ExamId = exam.ExamId
												and en.Status= 'A'
                                        WHERE  ans.MemberId = '{2}'
                                              AND  ans.TestNo = '{3}'
                                              AND  ans.ExamId = {1}
                                             AND stat.TestStatus = 2
                                                     -- AND ans.ChoiceId IS NOT NULL
                                        GROUP BY f.FactorId,
                                                 f.FactorNameTH,
                                                 en.Norm,
                                                 fn.Norm,
                                                 f.FactorNameEN,
                                                 exam.TotalQuestion,
                                                    exam.FullName,stat.TestStatus,stat.FinishedDate,stat.TimeUsed,stat.ScoreSet)
                                     SELECT rows.*,
									FLevel AS Level,
									CASE
                                                   WHEN @source = 'th' THEN
                                                       fs.Mean_TH
                                                   WHEN @source = 'en' THEN
                                                       fs.Mean_EN
                                               END AS Mean
                                    FROM rows
									inner join TB_FactorScore fs
											on fs.Fid = FactorId
									and Score between fs.SMin and fs.SMax
                                    ORDER BY FactorId asc", culture, ExamId, MemberId, TestNo);

                lstSummary = context.Database.SqlQuery<SummaryScoreModel>(sql.ToString()).ToList();
            }
            return lstSummary;

        }


        public static List<SummaryScoreModel> SummaryScore(string culture, int ExamId, int TestNo, string MemberId)
        {
            List<SummaryScoreModel> lstSummary = new List<SummaryScoreModel>();
            using (SpiritContext context = new SpiritContext())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" DECLARE @source NVARCHAR(2) = '{0}';
                                   WITH rows
                                    AS (select
                                       SUM(COALESCE(ans.Point, 0)) Score,
                                       COUNT(ans.ChoiceId) AS DoneQuestion,
                                       FactorId,
                                        exam.TotalQuestion,
                                       exam.FullName AS ExamFullName,
                                       CONVERT(CHAR(8), DATEADD(SECOND, SUM(ans.TimeUsed), ''), 114) AS TimeUsed,
                                       CONVERT(CHAR(8), DATEADD(SECOND, stat.TimeUsed, ''), 114) AS TotalTimeUsed,
                                       CONVERT(NVARCHAR(16), stat.FinishedDate, 121) AS FinishedDate
                                FROM dbo.TB_AnswerSheet ans
                                    INNER JOIN dbo.TB_Exam exam
                                        ON exam.ExamId = ans.ExamId
                                    INNER JOIN dbo.TB_ExamStat stat
                                        ON stat.ExamId = ans.ExamId
                                           AND stat.MemberId = ans.MemberId
                                           AND stat.TestNo = ans.TestNo
                                    INNER JOIN dbo.TB_Question q
                                        ON q.QuestionId = ans.QuestionId
                                        WHERE  ans.MemberId = '{2}'
                                              AND  ans.TestNo = '{3}'
                                              AND  ans.ExamId = {1}
                                             AND stat.TestStatus = 2
                                                     -- AND ans.ChoiceId IS NOT NULL
                                        GROUP BY    FactorId,
                                                    exam.TotalQuestion,
                                                     exam.FullName,
                                                     stat.TestStatus,
                                                     stat.FinishedDate,
                                                     stat.TimeUsed)
                                    SELECT *,SUM(rows.Score) OVER() AS TotalScore,
									FLevel AS Level,
									CASE
                                                   WHEN @source = 'th' THEN
                                                       fs.Mean_TH
                                                   WHEN @source = 'en' THEN
                                                       fs.Mean_EN
                                               END AS Mean
                                    FROM rows
                                    inner join TB_FactorScore fs
                                    on fs.Fid = FactorId
									and Score between fs.SMin and fs.SMax
                                   ", culture, ExamId, MemberId, TestNo);

                lstSummary = context.Database.SqlQuery<SummaryScoreModel>(sql.ToString()).ToList();
                //lstSummary.Max(i => i.Score);
            }
            return lstSummary;

        }

    }
}


