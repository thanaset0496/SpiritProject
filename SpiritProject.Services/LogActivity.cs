using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiritProject.Common;
using SpiritProject.DBUtils;

namespace SpiritProject.Services
{
    public static class LogActivity
    {
        public static void Log(MemberModel member, string Action,String IPAddress)
        {
            try
            {
                using (SpiritContext context = new SpiritContext())
                {
                    ///step 1 exitsed member ?
                    context.Database.ExecuteSqlCommand(@"
                        insert into TB_LogHistory (MemberId,LogText,LogType,CreatedDate,Address)
                        values (@MemberId,@LogText,@LogType,GetDate(),@Address)
                    ", new SqlParameter("MemberId", member.MemberId)
                    , new SqlParameter("LogText", Action)
                    , new SqlParameter("LogType", "ACT")
                    , new SqlParameter("Address", IPAddress)
                    );


                }
            }
            catch
            {
             
            }
        }
    }
}
