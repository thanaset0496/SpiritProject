using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiritProject.Common;
using SpiritProject.DBUtils;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Core.Objects;
using SpiritProject.Services.Util;

namespace SpiritProject.Services
{
    public class Member
    {
        public static MemberModel CheckAdminUser(string username, string password)
        {
            var admin = new MemberModel();
            StatusResponse status = new StatusResponse();
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    password = Hash.sha256(password);
                    List<int> MemberStatus = new List<int>() { 0, 1 };
                    ///step 1 exitsed member ?
                    var response = context.TB_Member.Where(i => i.MemberId.Equals(username) && i.Role.Equals("1") && MemberStatus.Contains(i.Status)).Select(i => i).FirstOrDefault();

                    // admin = context.Database.SqlQuery<MemberModel>(sql.ToString()).FirstOrDefault();
                    if (response != null)
                    {
                        admin = new MemberModel()
                        {
                            MemberId = response.MemberId,
                            Password = response.Password,
                            Email = response.Email,
                            MemberDOB = response.MemberDOB,
                            Role = response.Role,
                            MemberLastNameTH = response.MemberLastNameTH,
                            MemberLastNameEN = response.MemberLastNameEN,
                            MemberNameEN = response.MemberNameEN,
                            MemberNameTH = response.MemberNameTH,
                            Username = response.UserName,
                            Status = response.Status,
                            FailLogin = response.FailLogin

                        };

                        var WrongPassword = false;
                        if (admin.Password != password)
                        {
                            admin.FailLogin += 1;
                            WrongPassword = true;
                        }
                        else
                            admin.FailLogin = 0;

                        StampLastAccess(admin.MemberId, WrongPassword);
                    }
                    else return null;
                }
            }
            catch (Exception ex)
            {
                LogActivity.Log(new MemberModel() { MemberId = "CheckLoginUser" }, ex.Message, "Exception");
                return null;
            }

            return admin;

        }

        public static List<MemberModel> GetAdminList(string search, int order, string orderDir, int startRec, int pageSize, string culture)
        {
            List<MemberModel> lstExam = new List<MemberModel>();
            using (SpiritContext context = new SpiritContext())
            {
                order = order > 0 ? order + 1 : 2;
                orderDir = String.IsNullOrEmpty(orderDir) ? "desc " : orderDir;
                search = String.IsNullOrEmpty(search) ? "" : search;
                // startRec = String.IsNullOrEmpty(startRec) ? "" : "And " + orderDir;

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"
                                        DECLARE @source NVARCHAR(2) = '{5}';
                                        WITH rows
                                        AS (SELECT DISTINCT
                                                mem.MemberId,
                                                CASE
                                                   WHEN @source = 'th' THEN
                                                  COALESCE(Title,'')+' '+mem.MemberNameTH + ' ' + mem.MemberLastNameTH
                                                   WHEN @source = 'en' THEN
                                                  COALESCE(Title,'')+' '+mem.MemberNameEN + ' ' + mem.MemberLastNameEN
                                               END AS FullName,
                                                CONVERT(NVARCHAR(19),mem.LastAccess,103)+' '+CONVERT(NVARCHAR(19),mem.LastAccess,108) AS LastAccess,
                                                COUNT(mem.MemberId) OVER() AS TotalItem
                                            FROM dbo.TB_Member mem
                                            WHERE mem.Role = 1  And Status in (0,1)
                                                    )
                                        SELECT MemberId,FullName,LastAccess,TotalItem
                                        FROM rows
                                        WHERE (
                                                MemberId LIKE '%{4}%'
                                                  OR FullName LIKE N'%{4}%'
		                                        OR LastAccess LIKE '%{4}%'
                                              )
                                    ORDER BY {0} {1} 
                                    OFFSET {2} ROWS FETCH NEXT  {3} ROWS ONLY
                                        ", order, orderDir, startRec, pageSize, search, culture);

                lstExam = context.Database.SqlQuery<MemberModel>(sql.ToString()).ToList();

            }

            return lstExam;
        }


        public static MemberModel CheckLoginUser(string username, string password)
        {
            var member = new MemberModel();
            var examStat = new List<ExamStatModel>();
            StatusResponse status = new StatusResponse();
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    password = Hash.sha256(password);
                    List<int> MemberStatus = new List<int>() { 0, 1 };
                    ///step 1 exitsed member ?
                    ///
                    var response = context.TB_Member.Where(i => i.MemberId.Equals(username) && i.Role.Equals("0") && MemberStatus.Contains(i.Status)).Select(i => i).FirstOrDefault();
                    //var response = context.TB_Member.Where(i => i.MemberId.Equals(username)).Select(i => i).FirstOrDefault();


                    StringBuilder sql = new StringBuilder();
                    //sql.AppendFormat(@" SELECT m.MemberId
                    //               ,m.MemberNameTH
                    //               ,m.MemberLastNameTH
                    //               ,m.MemberNameEN
                    //                ,m.role
                    //               ,m.MemberLastNameEN
                    //               ,m.MemberDOB
                    //              FROM TB_Member m
                    //                WHERE m.MemberId = '{0}' AND m.password = '{1}' ", username, Hash.sha256(password));

                    //member = context.Database.SqlQuery<MemberModel>(sql.ToString()).FirstOrDefault();

                    if (response != null)
                    {
                        member = new MemberModel()
                        {
                            MemberId = response.MemberId,
                            Password = response.Password,
                            Email = response.Email,
                            MemberDOB = response.MemberDOB,
                            Role = response.Role,
                            Title = response.Title,
                            MemberLastNameTH = response.MemberLastNameTH,
                            MemberLastNameEN = response.MemberLastNameEN,
                            MemberNameEN = response.MemberNameEN,
                            MemberNameTH = response.MemberNameTH,
                            Username = response.UserName,
                            Status = response.Status,
                            FailLogin = response.FailLogin

                        };

                        if (member.Role == "1")
                            return null;

                        var WrongPassword = false;
                        if (member.Password != password)
                        {
                            member.FailLogin += 1;
                            WrongPassword = true;
                        }
                        else
                            member.FailLogin = 0;
                        ///LastAccess
                        StampLastAccess(member.MemberId, WrongPassword);
                        ///check e recuitment
                        ///

                        /*
                        sql = new StringBuilder();
                     .AppendFormat(@" SELECT e.*
                                  FROM TB_ExamStat e
                                    WHERE e.MemberId = '{0}'  ", username, password);

                        examStat = context.Database.SqlQuery<ExamStatModel>(sql.ToString()).ToList();
                        if (examStat.Count == 0)
                        {
                            {
                                var returnCode = new System.Data.Entity.Core.Objects.ObjectParameter("Result", typeof(int));
                                var TotalScore = new System.Data.Entity.Core.Objects.ObjectParameter("TotalScore", typeof(int));
                                context.SP_QIUD_EXAM(returnCode, TotalScore, username, 0, 0, 0, "", 0, 0, 0, "I", 0, 0);

                            }
                        }
                        */
                    }
                    else
                        return null;


                }
            }
            catch (Exception ex)
            {
                LogActivity.Log(new MemberModel() { MemberId = "CheckLoginUser" }, ex.Message, "Exception");
                member = new MemberModel();
                member.Message = "Error Check Login";
                return member;
            }

            return member;

        }


        public static MemberModel CheckNewLoginUser(string username, string password)
        {
            var member = new MemberModel();
            var examStat = new List<ExamStatModel>();
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            ///step 1 exitsed member ?
                            StringBuilder sql = new StringBuilder();
                            sql.AppendFormat(@" SELECT m.MemberId
                                   ,m.Title
                                   ,m.MemberNameTH
                                   ,m.MemberLastNameTH
                                   ,m.MemberNameEN
                                    ,m.role
                                   ,m.MemberLastNameEN
                                   ,m.MemberDOB
                                  FROM TB_Member m
                                    WHERE m.MemberId = '{0}'  ", username);

                            member = context.Database.SqlQuery<MemberModel>(sql.ToString()).FirstOrDefault();

                            //if (member == null)
                            //{
                            //    status.status = -1;
                            //    status.description = "fail";

                            //}
                            //else 
                            if (member != null)
                            {
                                member.Status = -2;
                                member.Message = "duplicate";

                            }
                            else
                            {
                                member = new MemberModel();
                                ///check e recuitment
                                ///
                                var ERecruitRespons = ERecruitment.CheckERecruitment(username, password);
                                if (ERecruitRespons != null)
                                {

                                    if (ERecruitRespons.Password != password)
                                    {
                                        member.Status = -4;
                                        member.Message = "wrongpassword";
                                    }
                                    else
                                    {
                                        var random = RandomPassword.Generate(10, 10);
                                        var NewPassword = Hash.sha256(random);

                                        sql = new StringBuilder();
                                        sql.AppendFormat(@" INSERT INTO [dbo].[TB_Member]
                                                       ([MemberId],username,MemberNameTH,MemberLastNameTH
                                                       ,MemberNameEN,MemberLastNameEN,MemberDOB,Status,Role,Password,RegisterDate,Email,Title
                                                       )
                                                     VALUES
                                                       ('{0}','{0}','{2}','{3}','{4}','{5}','{6}','0',0,'{1}',GETDATE(),'{7}','{8}'
                                                        )  ", username, NewPassword, ERecruitRespons.FirstName_Th, ERecruitRespons.LastName_Th,
                                                            ERecruitRespons.FirstName_En, ERecruitRespons.LastName_En, ERecruitRespons.BirthDate, ERecruitRespons.Email, ERecruitRespons.title);


                                        context.Database.ExecuteSqlCommand(sql.ToString());

                                        //member = new MemberModel();
                                        member.Password = random;
                                        member.FullName = ERecruitRespons.title + " " + ERecruitRespons.FirstName_Th + " " + ERecruitRespons.LastName_Th;
                                        member.MemberNameTH = ERecruitRespons.FirstName_Th;
                                        member.MemberLastNameTH = ERecruitRespons.LastName_Th;

                                        member.MemberNameEN = ERecruitRespons.FirstName_En;
                                        member.MemberLastNameEN = ERecruitRespons.LastName_En;

                                        member.Email = ERecruitRespons.Email;

                                        var returnCode = new SqlParameter("Result", SqlDbType.Int)
                                        {
                                            Direction = System.Data.ParameterDirection.Output
                                        };

                                        //// assign the return code to the new output parameter and pass it to the sp
                                        var data = context.Database.ExecuteSqlCommand(@"
                                                            SP_NEW_Member @Result out,
                                                                    @MemberId,
                                                                    @Action",
                                                                returnCode,
                                                                new SqlParameter("MemberId", username),
                                                               new SqlParameter("Action", "I"));
                                        member.Status = (int)returnCode.Value;
                                        //status.description = "success";
                                    }
                                }
                                else
                                {
                                    member.Status = -3;
                                    member.Message = "not Existed";
                                }
                            }

                            dbContextTransaction.Commit();
                        }
                        catch
                        {
                            member.Status = -1;
                            dbContextTransaction.Rollback();

                        }
                    }
                }
            }
            catch
            {
                member.Status = -1;
                // Console.Write(ex.Message);
            }

            return member;

        }

        public static MemberModel GetMember(string username)
        {
            MemberModel member = new MemberModel();
            using (SpiritContext context = new SpiritContext())
            {
                StringBuilder sql = new StringBuilder();
                //sql.AppendFormat(@"SELECT MemberId,Title,MemberNameTH,MemberLastNameTH,MemberNameEN,MemberLastNameEN,Email,
                //                    COALESCE(Title,'') + ' ' + COALESCE(MemberNameTH,'') + ' ' + COALESCE(MemberLastNameTH,'') AS FullName
                //                    From TB_Member where Username = @username
                //                        ", username);


                //member = context.Database.SqlQuery<MemberModel>(sql.ToString()).FirstOrDefault();

                member = context.TB_Member.Where(i => i.UserName == username).Select(i => new MemberModel()
                {
                    MemberId = i.MemberId,
                    Title = i.Title,
                    MemberNameTH = i.MemberNameTH,
                    MemberLastNameTH = i.MemberLastNameTH,
                    MemberNameEN = i.MemberNameEN,
                    MemberLastNameEN = i.MemberLastNameEN,
                    Email = i.Email,
                    FullName = i.Title + " " + i.MemberNameTH + " " + i.MemberLastNameTH

                })
                    .FirstOrDefault();


            }

            return member;
        }


        public static List<ExamStatModel> GetMemberList(string search, int order, string orderDir, int startRec, int pageSize, string culture)
        {
            List<ExamStatModel> lstExam = new List<ExamStatModel>();
            using (SpiritContext context = new SpiritContext())
            {
                order = order > 0 ? order + 1 : 3;
                orderDir = String.IsNullOrEmpty(orderDir) ? "desc " : orderDir;
                search = String.IsNullOrEmpty(search) ? "" : search;
                // startRec = String.IsNullOrEmpty(startRec) ? "" : "And " + orderDir;

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"
                                        DECLARE @source NVARCHAR(2) = '{5}';
                                        WITH rows
                                        AS (SELECT DISTINCT
                                                mem.MemberId,
                                                CASE
                                                   WHEN @source = 'th' THEN
                                                  COALESCE(Title,'')+' '+mem.MemberNameTH + ' ' + mem.MemberLastNameTH
                                                   WHEN @source = 'en' THEN
                                                  COALESCE(Title,'')+' '+mem.MemberNameEN + ' ' + mem.MemberLastNameEN
                                               END AS FullName,
                                                stat.TestNo,
                                                stat.TestStatus,
                                                mem.LastAccess
                                            FROM dbo.TB_Member mem
                                                LEFT JOIN TB_ExamStat stat
                                                    ON stat.MemberId = mem.MemberId
                                            WHERE mem.Role = 0 and stat.TestNo =
                                            (
                                                SELECT MAX(stat2.TestNo)
                                                FROM TB_ExamStat stat2
                                                WHERE stat2.MemberId = mem.MemberId
                                            )
                                            GROUP BY stat.TestNo,
                                                     mem.MemberId,
													 MemberNameTH,
													 MemberLastNameTH,
                                                    MemberNameEN,
													 MemberLastNameEN,
                                                        Title,
                                                     stat.TestStatus,mem.LastAccess)
                                        SELECT MemberId,FullName,LastAccess,TestNo,TestStatus
                                        FROM rows
                                        WHERE (
                                                MemberId LIKE '%{4}%'
                                                OR FullName LIKE '%{4}%'
                                                OR TestNo LIKE '%{4}%'
                                                OR TestStatus LIKE '%{4}%'
		                                        OR LastAccess LIKE '%{4}%'
                                              )
                                    ORDER BY {0} {1} 
                                    OFFSET {2} ROWS FETCH NEXT  {3} ROWS ONLY
                                        ", order, orderDir, startRec, pageSize, search, culture);

                lstExam = context.Database.SqlQuery<ExamStatModel>(sql.ToString()).ToList();

            }


            return lstExam;
        }

        public static int CreateNewPassword(string username, string oldPassword, string newPassword)
        {
            int result = 0;
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    ///step 1 exitsed member ?
                    ///




                    ///step 2 update password
                    StringBuilder sql = new StringBuilder();
                    sql.AppendFormat(@" UPDATE  TB_Member
                                    SET Password = '{2}'
                                    WHERE UserName = '{0}'
                                   AND password = '{1}'
                        
                         ", username, oldPassword, Hash.sha256(newPassword));


                    result = context.Database.ExecuteSqlCommand(sql.ToString());
                }
            }
            catch
            {

            }
            return result;
        }

        public static string GenerateNewPassword(string username)
        {
            int result = 0;
            var random = RandomPassword.Generate(10, 10);
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    ///step 1 exitsed member ?
                    ///
                    var newPassword = Hash.sha256(random);


                    ///step 2 update password
                    StringBuilder sql = new StringBuilder();
                    sql.AppendFormat(@" UPDATE  TB_Member
                                    SET Password = '{1}'
                                    WHERE MemberId = '{0}' ", username, newPassword);


                    result = context.Database.ExecuteSqlCommand(sql.ToString());
                }
            }
            catch
            {

            }
            if (result > 0)
                return random;
            else return null;
        }

        public static int UpdateNewPassword(string username, string newPassword)
        {
            int result = 0;
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    ///step 1 exitsed member ?

                    var response = context.Database.ExecuteSqlCommand(@"UPDATE TB_Member SET password = @NewPassword,Status = 1 where MemberId=@MemberId",

                         new SqlParameter("MemberId", username),
                         new SqlParameter("NewPassword", Hash.sha256(newPassword))
                         );

                    result = response;
                    //  result = context.Database.ExecuteSqlCommand(sql.ToString());
                }
            }
            catch
            {

            }
            return result;
        }

        public static string UpdatePassword(string username, string oldPassword, string newPassword)
        {
            string result = String.Empty;
            try
            {
                using (SpiritContext context = new SpiritContext())
                {

                    var response = new SqlParameter("Result", SqlDbType.NVarChar)
                    {
                        Direction = System.Data.ParameterDirection.Output,
                        Size = 2000
                    };

                    ///step 1 exitsed member ?

                    context.Database.ExecuteSqlCommand(@"EXEC SP_U_PASSWORD_Member @Result OUT,@MemberId,@OldPassword,@NewPassword",
                        response,
                        new SqlParameter("MemberId", username),
                        new SqlParameter("OldPassword", Hash.sha256(oldPassword)),
                        new SqlParameter("NewPassword", Hash.sha256(newPassword))
                        );

                    result = response.Value.ToString();
                    //  result = context.Database.ExecuteSqlCommand(sql.ToString());
                }
            }
            catch
            {

            }
            return result;
        }

        public static string UpdateMail(string username, string mail)
        {
            string result = String.Empty;
            try
            {
                using (SpiritContext context = new SpiritContext())
                {

                    var res = context.Database.ExecuteSqlCommand(@" Update TB_Member Set Email = @Mail Where MemberId = @MemberId",
                         new SqlParameter("MemberId", username),
                         new SqlParameter("Mail", mail)
                         );

                    result = res.ToString();
                    //  result = context.Database.ExecuteSqlCommand(sql.ToString());
                }
            }
            catch
            {

            }
            return result;
        }


        public static int CreateNewAdmin(MemberModel member)
        {
            int result = 0;
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    ///step 1 exitsed member ?
                    StringBuilder sql = new StringBuilder();
                    sql.AppendFormat(@" 
                                        IF NOT EXISTS (
                                        SELECT MemberId FROM TB_Member Where  MemberId = N'{0}'                                        )
                                        INSERT INTO dbo.TB_Member
                                    (
                                        MemberId,MemberNameTH,
                                        MemberLastNameTH,MemberNameEN,
                                        MemberLastNameEN,RegisterDate,
                                        Status,Role,UserName,Password,Email,Title
                                    )
                                    VALUES
                                    (   N'{0}',       -- MemberId - nvarchar(13)
                                        N'{1}',       -- MemberNameTH - nvarchar(1000)
                                        N'{2}',       -- MemberLastNameTH - nvarchar(1000)
                                        N'{3}',       -- MemberNameEN - nvarchar(1000)
                                        N'{4}',       -- MemberLastNameEN - nvarchar(1000)
                                        GETDATE(), -- RegisterDate - datetime
                                        0,         -- Status - int
                                        N'1',       -- Role - nvarchar(50)
                                        N'{0}',       -- UserName - nvarchar(50)
                                        N'{5}',        -- Password - nvarchar(50)
                                        N'{6}',
                                        N'{7}'
                                        )", member.Username, member.MemberNameTH, member.MemberLastNameTH
                                    , member.MemberNameEN, member.MemberNameEN, Hash.sha256(member.Password), member.Email, member.Title);


                    result = context.Database.ExecuteSqlCommand(sql.ToString());
                }
            }
            catch
            {

            }
            return result;
        }


        public static int ConfigAdmin(MemberModel member)
        {
            int result = 0;
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    ///step 1 exitsed member ?
                    StringBuilder sql = new StringBuilder();
                    sql.AppendFormat(@" Update  dbo.TB_Member SET
                                        MemberNameTH = N'{1}',
                                        MemberLastNameTH = N'{2}',MemberNameEN = N'{3}',
                                        MemberLastNameEN = N'{4}',
                                        Email = N'{5}',
                                        Title = N'{6}',
                                        Status = 1 ", member.MemberId, member.MemberNameTH, member.MemberLastNameTH
                                    , member.MemberNameEN, member.MemberNameEN, member.Email, member.Title);
                    if (!String.IsNullOrEmpty(member.Password))
                    {
                        var EncodePassword = Hash.sha256(member.Password);
                        sql.AppendFormat(@" ,Password = N'{0}' ", EncodePassword);
                    }
                    sql.AppendFormat(@"  Where Username = '{0}' ", member.MemberId);
                    result = context.Database.ExecuteSqlCommand(sql.ToString());
                }
            }
            catch
            {

            }
            return result;
        }

        public static StatusResponse StampLastAccess(string MemberId, bool WrongPassword)
        {
            StatusResponse response = new StatusResponse();
            using (SpiritContext context = new SpiritContext())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" IF EXISTS
                                    (
                                        SELECT *
                                        FROM dbo.TB_Member
                                        WHERE MemberId = '{0}'
                                    )
                                    BEGIN
                                        UPDATE mem
                                        SET  mem.LastAccess = GETDATE()
                                       ", MemberId);
                if (WrongPassword)
                {
                    sql.AppendFormat(@" ,FailLogin += COALESCE(FailLogin, 0) + 1 ");
                }
                else
                    sql.AppendFormat(@" ,FailLogin = 0  ");

                sql.AppendFormat(@"  FROM TB_Member mem  WHERE  MemberId = '{0}'
                                    END;
                                ", MemberId);

                response.status = context.Database.ExecuteSqlCommand(sql.ToString());

            }

            return response;
        }

    }

}


