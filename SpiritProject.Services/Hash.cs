using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using SpiritProject.DBUtils;
using SpiritProject.Common;
using System.Configuration;

namespace SpiritProject.Services
{
    public class Hash
    {
        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file

            string key = "0000";// (string)settingsReader.GetValue("SecurityKey",typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = "0000";// (string)settingsReader.GetValue("SecurityKey",typeof(String));

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static void EncodeQuestion()
        {
            List<QuestionModel> lstQ = new List<QuestionModel>();
            using (SpiritContext context = new SpiritContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" SELECT  QuestionId,DataTH,DataEN
                                  FROM [TB_Question]
                                    ");

                lstQ = context.Database.SqlQuery<QuestionModel>(sql.ToString()).ToList();


                foreach (var q in lstQ)
                {
                    var THEncrypt = Encrypt(q.DataTH, true);
                    var ENEncrypt = Encrypt(q.DataEN, true);

                    sql.AppendFormat(@" update TB_Question set DataTH='{0}',DataEN = '{1}'
                                        where QuestionId = {2}
                                    ", THEncrypt, ENEncrypt, q.QuestionId);

                    context.Database.ExecuteSqlCommand(sql.ToString());
                }
            }

        }

        public static void EncodeChoiceWG()
        {
            List<ChoiceModel> lstChoice = new List<ChoiceModel>();
            using (SpiritContext context = new SpiritContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" SELECT * 
                                  FROM [TB_ChoiceWG]
                                    ");

                lstChoice = context.Database.SqlQuery<ChoiceModel>(sql.ToString()).ToList();


                foreach (var choice in lstChoice)
                {
                    var THEncrypt = Encrypt(choice.ChoiceNameTH, true);
                    var ENEncrypt = Encrypt(choice.ChoiceNameEN, true);

                    sql.AppendFormat(@" update TB_ChoiceWG set ChoiceNameTH='{0}',ChoiceNameEN = '{1}'
                                        where ChoiceId = {2} and QuestionId = {3}
                                    ", THEncrypt, ENEncrypt, choice.ChoiceId,choice.QuestionId);

                    context.Database.ExecuteSqlCommand(sql.ToString());
                    sql = new StringBuilder();
                }
            }

        }

        public static void EncodeChoiceFake()
        {
            List<ChoiceModel> lstChoice = new List<ChoiceModel>();
            using (SpiritContext context = new SpiritContext())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@" SELECT FakeChoiceId as ChoiceId, FakeChoiceNameTH as ChoiceNameTH,FakeChoiceNameEN as ChoiceNameEN
                                  FROM [TB_FakeChoice]
                                    ");

                lstChoice = context.Database.SqlQuery<ChoiceModel>(sql.ToString()).ToList();


                foreach (var choice in lstChoice)
                {
                    var THEncrypt = Encrypt(choice.ChoiceNameTH, true);
                    var ENEncrypt = Encrypt(choice.ChoiceNameEN, true);

                    sql.AppendFormat(@" update TB_FakeChoice set FakeChoiceNameTH='{0}',FakeChoiceNameEN = '{1}'
                                        where FakeChoiceId = {2}
                                    ", THEncrypt, ENEncrypt, choice.ChoiceId);

                    context.Database.ExecuteSqlCommand(sql.ToString());
                    sql = new StringBuilder();
                }
            }

        }

        public static string sha256(string randomString)
        {
            SHA256Managed crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString), 0, Encoding.ASCII.GetByteCount(randomString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }
    }
}
