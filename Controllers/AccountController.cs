using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OLMapAPI_Core_PoC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OLMapAPI_Core_PoC.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> userManager;

        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> usrMgr)
        {
            _context = context;
            userManager = usrMgr;
        }

        #region 管理員使用
        [HttpPost("UserExistsAsync")]
        // 判斷使用者是否已在存在
        public async Task<bool> UserExistsAsync(string name)
        {
            var username = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[0], name)
                );
            ApplicationUser user = await userManager.GetUserAsync(username);


            if (user == null)
            {
                return false;

            }
            else
            {
                return true;
            }
        }

        [HttpPost("CreateUserAsync")]
        // 新增使用者
        public async Task<bool> CreateUserAsync(ApplicationUser userName, string password)
        {
            var idResult = await userManager.CreateAsync(userName, password);
            return idResult.Succeeded;
        }

        [HttpPost("CreateUser_stringAsync")]
        // 新增使用者_使用userName
        public async Task<bool> CreateUser_stringAsync(string userName, string password)
        {
            var user = new ApplicationUser() { UserName = userName };
            var idResult = await userManager.CreateAsync(user, password);
            return idResult.Succeeded;
        }

        [HttpPost("DeleteUserOnlyASPNET")]
        //刪除使用者
        public bool DeleteUserOnlyASPNET(string username)
        {
            var Db = _context;
            var user = Db.Users.First(u => u.UserName == username);
            Db.Users.Remove(user);
            Db.SaveChanges();
            return true;
        }


        ////刪除使用者
        //public bool DeleteUser(string username)
        //{
        //    var Db = _context;
        //    var user = Db.Users.First(u => u.UserName == username);
        //    Db.Users.Remove(user);
        //    Db.SaveChanges();

        //    System.Web.UI.WebControls.SqlDataSource sds = new System.Web.UI.WebControls.SqlDataSource();
        //    sds.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    String SQLStr = "DELETE AspNetLinkUserData Where username = @username";
        //    sds.DeleteParameters.Add("username", username);
        //    sds.DeleteCommand = SQLStr;
        //    sds.Delete();

        //    return true;
        //}

        [HttpGet("getUserList")]
        //列出使用者列表
        public List<string> getUserList()
        {
            List<string> list = new List<string>();
            var Db = _context;
            var users = Db.Users;
            foreach (var user in users)
            {
                list.Add(user.UserName);
            }
            return list;
        }

        [HttpGet("getRoleList")]
        //列出權限列表
        public List<string> getRoleList()
        {
            List<string> list = new List<string>();

            var Db = _context;
            var roles = Db.Roles;
            foreach (var role in roles)
            {
                list.Add(role.Name);
            }
            return list;
        }
        //// 確認使用者是否重複
        //public bool UserisExist(string userName)
        //{
        //    var manager = userManager;
        //    ApplicationUser user = manager.FindByName(userName);
        //    bool bresult = false;
        //    if (user != null)
        //    {
        //        bresult = true;
        //    }
        //    return bresult;
        //}
        ////變更密碼
        //public void changePassword(string userName, string Currentpassword, string Newpassword)
        //{
        //    var manager = userManager;
        //    manager.ChangePassword(userName, Currentpassword, Newpassword);
        //}

        ////admin幫使用者變更密碼
        //public void adminChangePassword(string userName, string Newpassword)
        //{
        //    var manager = userManager;
        //    manager.RemovePassword(userName);
        //    manager.AddPassword(userName, Newpassword);
        //}

        //public void deluser(string userName)
        //{
        //    var manager = userManager;
        //    ApplicationUser user = manager.FindByName(userName);
        //    manager.Delete(user);

        //}

        //public DataView getUserListData()
        //{
        //    System.Web.UI.WebControls.SqlDataSource sds = new System.Web.UI.WebControls.SqlDataSource();
        //    sds.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //    String SQLStr = "SELECT username,name,dep,mail,note FROM AspNetLinkUserData";
        //    sds.SelectParameters.Clear();
        //    //sds.SelectParameters.Add("group_name", "%" + group_name + "%");
        //    sds.SelectCommand = SQLStr;

        //    DataView dv = (DataView)sds.Select(DataSourceSelectArguments.Empty);

        //    return dv;
        //}
        #endregion
    }
}
