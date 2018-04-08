using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using SpiritProject.Common;
using System.Globalization;

namespace SpiritProject.Services
{
    public static class ERecruitment
    {
        public static ERecruitmentModel CheckERecruitment(string username, string password)
        {
            ERecruitmentModel model = new ERecruitmentModel();
            //model.ApplicantCode ="01";
            //model.PersonalID = "TestMockup";
            //model.Email = "Test@mail.com";
            //model.title = "Mr";
            //model.FirstName_En = "Somsuk";
            //model.LastName_En = "Jiam";
            //model.FirstName_Th = "สมศัก";
            //model.LastName_Th = "เจียม";
            //model.BirthDate = DateTime.Now;

            var culture = new CultureInfo("en-US");

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ERecruitment"].ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(@"Select ApplicantCode,PersonalID,Email,title,FirstName_En,
                                                        LastName_En,FirstName_Th,LastName_Th,CONVERT(NVARCHAR(10),BirthDate,121)  AS BirthDate,CONVERT(NVARCHAR(10),BirthDate,112)  AS Password
                                                        from vApplicant_forPrescreenApp 
                                                        WHERE PersonalID = @PersonalID ", connection);
                command.Parameters.AddWithValue("@PersonalID", username);
                //command.Parameters.AddWithValue("@BirthDate", password);
                //command.Parameters.AddWithValue("@zip", "india");

                // int result = command.ExecuteNonQuery();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.ApplicantCode = reader["ApplicantCode"].ToString();
                        model.PersonalID = reader["PersonalID"].ToString();
                        model.Email = reader["Email"].ToString();
                        model.title = reader["title"].ToString();
                        model.FirstName_En = reader["FirstName_En"].ToString();
                        model.LastName_En = reader["LastName_En"].ToString();
                        model.FirstName_Th = reader["FirstName_Th"].ToString();
                        model.LastName_Th = reader["LastName_Th"].ToString();
                        model.BirthDate = reader["BirthDate"].ToString();
                        model.Password = reader["Password"].ToString();
                    }
                    else
                        model = null;
                }

                //using (var ds = new DataSet())
                //using (var da = new SqlDataAdapter(command))
                //{
                //    da.Fill(ds, "result_table");

                ////    return ds;
                //}
            }


            return model;
        }

        public static ERecruitmentModel GetMail(string username)
        {
            ERecruitmentModel model = new ERecruitmentModel();

            var culture = new CultureInfo("en-US");

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ERecruitment"].ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(@"Select ApplicantCode,PersonalID,Email,title,FirstName_En,
                                                        LastName_En,FirstName_Th,LastName_Th,CONVERT(NVARCHAR(10),BirthDate,121)  AS BirthDate
                                                        from vApplicant_forPrescreenApp 
                                                        WHERE PersonalID = @PersonalID ", connection);
                command.Parameters.AddWithValue("@PersonalID", username);
                //command.Parameters.AddWithValue("@zip", "india");

                // int result = command.ExecuteNonQuery();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.ApplicantCode = reader["ApplicantCode"].ToString();
                        model.PersonalID = reader["PersonalID"].ToString();
                        model.Email = reader["Email"].ToString();
                        model.title = reader["title"].ToString();
                        model.FirstName_En = reader["FirstName_En"].ToString();
                        model.LastName_En = reader["LastName_En"].ToString();
                        model.FirstName_Th = reader["FirstName_Th"].ToString();
                        model.LastName_Th = reader["LastName_Th"].ToString();
                        model.BirthDate = reader["BirthDate"].ToString();
                    }
                    else
                        model = null;
                }

                //using (var ds = new DataSet())
                //using (var da = new SqlDataAdapter(command))
                //{
                //    da.Fill(ds, "result_table");

                ////    return ds;
                //}
            }


            return model;
        }

    }
}
