using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLMapAPI_Core_PoC.Models;
using OLMapAPI_Core_PoC.Models.auth;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OLMapAPI_Core_PoC.Infrastructure.auth
{
    public class AuthFunc
    {
        private readonly IConfiguration config;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> userManager;
        public AuthFunc(ApplicationDbContext context, UserManager<ApplicationUser> usrMgr, IConfiguration config)
        {
            _context = context;
            userManager = usrMgr;
            this.config = config;
        }

        /// <summary>
        /// apiuser-驗證帳號
        /// </summary>
        public async Task<tokenObj> validatesApiUserAsync(loginInfo login)
        {
            string sqlstr = @"SELECT userId,aspnetUserId,aspnetPassword FROM sys_apiUser where lockYN='0' and userId=@userId";
            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            conn.Open();
            cmd.Parameters.AddWithValue("@userId", login.userid);
            SqlDataReader dr = cmd.ExecuteReader();
            string userId = "";
            string aspnetUserId = "";
            while (dr.Read())
            {
                userId = dr["userId"].ToString();
                aspnetUserId = dr["aspnetUserId"].ToString();
            }
            dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();

            if (userId != "" && aspnetUserId != "" && login.password != "")
            {
                var manager = userManager;
                ApplicationUser user = await manager.FindByNameAsync(aspnetUserId);
                bool YN = await manager.CheckPasswordAsync(user, "apiUser@_" + login.password);
                if (YN)
                {
                    string token = createToken(aspnetUserId, "apiuser");
                    tokenObj tokenobj = new tokenObj()
                    {
                        status = "success",
                        token = token
                    };
                    return tokenobj;
                }
                else
                {
                    tokenObj error = new tokenObj()
                    {
                        status = "error-no dotnet user",
                        token = ""
                    };
                    return error;
                }
            }
            else
            {
                tokenObj error = new tokenObj()
                {
                    status = "error-no user",
                    token = ""
                };
                return error;
            }
        }

        private string createToken(string userid, string type)
        {
            string salt = GetRandomString(10);
            string token = generateToken(userid, salt);
            delExpiredToken();
            string sqlstr = @"INSERT INTO sys_tokens (userid,type,token,issuedOn,expiredOn,salt) Values (@userid,@type,@token,@issuedOn,@expiredOn,@salt)";
            sqlstr += @"INSERT INTO CRM_Record (LoginName,AccessTime,Remark) Values (@userid,GETDATE(),'OLMap')";
            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            conn.Open();
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@token", token);
            cmd.Parameters.AddWithValue("@issuedOn", DateTime.Now);
            cmd.Parameters.AddWithValue("@expiredOn", DateTime.Now.AddHours(8)); //8小時後刪除
            cmd.Parameters.AddWithValue("@salt", salt);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
            return "OLMapAPI " + token;
        }


        private void delExpiredToken()
        {
            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand("DELETE sys_tokens Where GETDATE() > expiredOn", conn);
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
        }


        protected string generateToken(string userid, string salt)
        {
            DateTime dt = DateTime.Now;
            byte[] useridAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(dt.ToFileTime().ToString() + "-" + userid + salt);
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(useridAndSaltBytes);
            string hashString = Convert.ToBase64String(hashBytes);
            return hashString;
        }


        //public static string GetRandomString(int length)
        //{
        //    Random r = new Random();

        //    string code = "";

        //    for (int i = 0; i < length; ++i)
        //        switch (r.Next(0, 3))
        //        {
        //            case 0: code += r.Next(0, 10); break;
        //            case 1: code += (char)r.Next(65, 91); break;
        //            case 2: code += (char)r.Next(97, 122); break;
        //        }

        //    return code;
        //}

        public static string GetRandomString(int length)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = "";
            str += "0123456789";
            str += "abcdefghijklmnopqrstuvwxyz";
            str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        /// <summary>
        /// 驗證token
        /// </summary>
        public string validatesToken(string token)
        {
            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand("select userid from sys_tokens where token=@token and  GETDATE()<=expiredOn", conn);
            conn.Open();
            // 移除 "OLMapAPI "
            cmd.Parameters.AddWithValue("@token", token.Replace("OLMapAPI ", ""));
            SqlDataReader dr = cmd.ExecuteReader();
            string userid = "";
            while (dr.Read())
            {
                userid = dr["userid"].ToString();
            }
            if (userid != "") { reNewToken(token.Replace("OLMapAPI ", "")); }
            dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
            return userid;
        }

        private void reNewToken(string token)
        {
            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand("UPDATE sys_tokens SET expiredOn=@expiredOn Where token=@token", conn);
            conn.Open();
            cmd.Parameters.AddWithValue("@token", token);
            cmd.Parameters.AddWithValue("@expiredOn", DateTime.Now.AddHours(8));
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
        }

        public string getUserNameByToken_true(string token)
        {
            string userName = "";

            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            string cmdStr = @"SELECT a.[userId]
                                FROM [OLDemo].[dbo].[sys_apiUser] a , [OLDemo].[dbo].[sys_tokens] b
                                where a.aspnetUserId=b.userid and b.token=@token";


            cmd.CommandType = CommandType.Text;
            cmd.CommandText = cmdStr;
            cmd.Parameters.AddWithValue("@token", token);

            try
            {
                conn.Open();

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    userName = sdr[0].ToString();
                }
                conn.Close();
                cmd.Dispose();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                throw;
            }
            return userName;

        }
        public static bool IsValidJson(object obj)
        {
            string strInput = "";
            strInput = JsonConvert.SerializeObject(obj);



            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var parsedObj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    //Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    //Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static dynamic translateNullIntoEmpty(object o)
        {

            var props = o.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.GetValue(o) == null)
                {
                    prop.SetValue(o, "");
                }
            }


            return o;
        }

        public static string parseTokenFromHeader(HttpRequestMessage request)
        {
            string token = "";

            IEnumerable<string> authenticationHeaders;
            if (request.Headers.TryGetValues("Authorization", out authenticationHeaders))
            {
                string fullTokenStr = authenticationHeaders.ToList()[0];

                if (fullTokenStr.Contains("OLMapAPI"))
                {
                    token = fullTokenStr.Split(' ')[1];

                }
            }

            return token;


        }


    }
}
