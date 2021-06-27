using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OLMapAPI_Core_PoC.Infrastructure.auth;
using OLMapAPI_Core_PoC.Models;
using OLMapAPI_Core_PoC.Models.auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLMapAPI_Core_PoC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUserController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public ApiUserController(ApplicationDbContext context, UserManager<ApplicationUser> usrMgr, IConfiguration config)
        {
            _context = context;
            _userManager = usrMgr;
            this._config = config;
        }

        #region API User操作，不對外開放


        /// <summary>
        /// 取得apiUser列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("getApiUserList")]
        public List<apiUserObj> getApiUserList()
        {
            ApiUserFunc apiUserF = new ApiUserFunc(_context, _userManager, _config);
            return apiUserF.getApiUserList();
        }

        /// <summary>
        /// 新增apiUser
        /// </summary>
        /// <returns></returns>
        [HttpPost("insertApiUser")]
        public async Task<string> insertApiUserAsync(insertApiUserObj apiUser)
        {
            ApiUserFunc apiUserF = new ApiUserFunc(_context, _userManager, _config);
            return await apiUserF.insertApiUserAsync(apiUser);
        }

        /// <summary>
        /// 移除apiUser
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("removeApiUser")]
        public string removeApiUser(removeApiUserObj obj)
        {
            ApiUserFunc apiUserF = new ApiUserFunc(_context, _userManager, _config);
            return apiUserF.removeApiUser(obj);
        }

        /// <summary>
        /// 更新apiUser狀態(鎖1→不鎖0、不鎖0→鎖1)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("lockApiUser")]
        public string lockApiUser(lockApiUserObj obj)
        {
            ApiUserFunc apiUserF = new ApiUserFunc(_context, _userManager, _config);
            return apiUserF.lockApiUser(obj);
        }

        #endregion

    }
}
