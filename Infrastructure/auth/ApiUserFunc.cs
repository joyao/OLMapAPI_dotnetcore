using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OLMapAPI_Core_PoC.Models;
using OLMapAPI_Core_PoC.Models.auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLMapAPI_Core_PoC.Infrastructure.auth
{
    /// <summary>
    /// admin編輯apiUser各項方法
    /// </summary>
    public class ApiUserFunc
    {

        private readonly IConfiguration config;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> userManager;
        public ApiUserFunc(ApplicationDbContext context, UserManager<ApplicationUser> usrMgr,IConfiguration config)
        {
            _context = context;
            userManager = usrMgr;
            this.config = config;
        }
        /// <summary>
        /// 取得列表
        /// </summary>
        /// <returns></returns>
        public List<apiUserObj> getApiUserList()
        {
            List<apiUserObj> apiUsers = new List<apiUserObj>();

            string sqlstr = @"SELECT id,userId,company,note,password,lockYN FROM sys_apiUser";
            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            conn.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string id = dr["id"].ToString();
                string userId = dr["userId"].ToString();
                string unit = dr["company"].ToString();
                string note = dr["note"].ToString();
                string password = dr["password"].ToString();
                password = password.Substring(0, 1) + "****" + password.Substring(password.Length - 1, 1);
                string lockYN = dr["lockYN"].ToString();

                apiUserObj obj = new apiUserObj(){
                    id = id,
                    userId =  userId,
                    unit = unit,
                    note = note,
                    password = password,
                    lockYN = lockYN
                };
                apiUsers.Add(obj);
            }
            dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
            return apiUsers;
        }


        /// <summary>
        /// 新增apiUser
        /// </summary>
        /// <returns></returns>
        public async Task<string> insertApiUserAsync(insertApiUserObj apiUser)
        {
            AccountFunc acc = new AccountFunc(_context, userManager);
            string aspnetUserId = "apiUser_" + apiUser.userId.Trim();
            string aspnetPassword = "apiUser@_" + apiUser.password.Trim();

            var newUser = new ApplicationUser() { UserName = aspnetUserId };

            // part1.使用account建立User於AspNet.Identity產生的[AspNetUsers]資料表中
            bool bresult = await acc.CreateUserAsync(newUser, aspnetPassword);

            // part2.同時建立在自訂的[sys_apiUser]資料表中
            if (bresult)
            {
                SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
                SqlCommand cmd = new SqlCommand("INSERT INTO sys_apiUser (userId,company,note,password,issuedOn,expiredOn,lockYN,aspnetUserId,aspnetPassword) Values (@userId,@company,@note,@password,@issuedOn,@expiredOn,@lockYN,@aspnetUserId,@aspnetPassword)", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("@userId", apiUser.userId.Trim());
                cmd.Parameters.AddWithValue("@company", apiUser.unit);
                cmd.Parameters.AddWithValue("@note", apiUser.note);
                cmd.Parameters.AddWithValue("@password", apiUser.password.Trim());
                cmd.Parameters.AddWithValue("@issuedOn", DateTime.Now);
                cmd.Parameters.AddWithValue("@expiredOn", DateTime.Now.AddYears(100));
                cmd.Parameters.AddWithValue("@lockYN", "0");
                cmd.Parameters.AddWithValue("@aspnetUserId", aspnetUserId);
                cmd.Parameters.AddWithValue("@aspnetPassword", aspnetPassword);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
                return "ok";
            }
            else
            {
                return "error";
            }
        }

        /// <summary>
        /// 移除ApiUser
        /// </summary>
        /// <returns></returns>
        public string removeApiUser(removeApiUserObj removeApiUser)
        {
            string aspnetUserId = getApiUserIdFromId(removeApiUser.id);
            AccountFunc acc = new AccountFunc(_context, userManager);
            bool bresult = acc.DeleteUserOnlyASPNET(aspnetUserId);

            if (bresult)
            {
                SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
                SqlCommand cmd = new SqlCommand("DELETE sys_apiUser Where id=@id", conn);
                conn.Open();
                cmd.Parameters.AddWithValue("@id", removeApiUser.id);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
                return "ok";
            }
            else
            {
                return "error";
            }
        }


        private string getApiUserIdFromId(string id)
        {
            string sqlstr = @"SELECT id,aspnetUserId FROM sys_apiUser where id=@id";
            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            conn.Open();
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string aspnetUserId = dr["aspnetUserId"].ToString();
                return aspnetUserId;
            }
            dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
            return "";
        }

        /// <summary>
        /// 鎖ApiUser
        /// </summary>
        /// <returns></returns>
        public string lockApiUser(lockApiUserObj lockApiUser)
        {
            string lockYN = "0";
            if (getApiUserLockFromId(lockApiUser.id) == "0")
            {
                lockYN = "1";
            }
            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand("update sys_apiUser SET lockYN=@lockYN Where id=@id", conn);
            conn.Open();
            cmd.Parameters.AddWithValue("@id", lockApiUser.id);
            cmd.Parameters.AddWithValue("@lockYN", lockYN);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
            return "ok";
        }

        private string getApiUserLockFromId(string id)
        {
            string sqlstr = @"SELECT id,lockYN FROM sys_apiUser where id=@id";
            SqlConnection conn = new SqlConnection(config.GetConnectionString("DotNetCore_DB"));
            SqlCommand cmd = new SqlCommand(sqlstr, conn);
            conn.Open();
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string lockYN = dr["lockYN"].ToString();
                return lockYN;
            }
            dr.Close(); dr.Dispose(); conn.Close(); conn.Dispose();
            return "";
        }

    }
}
