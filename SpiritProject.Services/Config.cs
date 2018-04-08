using SpiritProject.Common;
using SpiritProject.DBUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritProject.Services
{
    public static class Config
    {
        public static ConfigModel GetConfig(string Type)
        {
            ConfigModel result = new ConfigModel();
            using (SpiritContext context = new SpiritContext())
            {



                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"  SELECT * FROM TB_Config WHERE Type = '{0}'", Type);

                result = context.Database.SqlQuery<ConfigModel>(sql.ToString()).FirstOrDefault();


            }

            return result;
        }

    }
}
