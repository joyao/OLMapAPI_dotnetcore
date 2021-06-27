using Microsoft.AspNetCore.Authorization;
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
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public AuthController(ApplicationDbContext context, UserManager<ApplicationUser> usrMgr, IConfiguration config)
        {
            _context = context;
            _userManager = usrMgr;
            this._config = config;
        }

        /// <summary>
        /// 由帳號密碼返回token
        /// </summary>
        /// <remarks>使用已註冊的帳號密碼取得token</remarks>
        /// <response code="200">OK</response>
        /// <response code="400">Not found</response>
        //介接api用的帳號，僅提供token返回，不提供token取得資訊，該token僅可使用一般API
        [HttpPost("validatesApiUser"), AllowAnonymous]
        public async Task<tokenObj> validatesApiUserAsync(loginInfo login)
        {
            AuthFunc auth = new AuthFunc(_context, _userManager, _config);
            tokenObj token = await auth.validatesApiUserAsync(login);
            return token;
        }
    }
}
