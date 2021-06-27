using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OLMapAPI_Core_PoC.Infrastructure.auth;
using OLMapAPI_Core_PoC.Models;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace OLMapAPI_Core_PoC.MessageHandler
{
    public class CustomTokenAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScemeName = "CustomTokenAuthenticationScheme";
        public string TokenHeaderName { get; set; } = "Authorization";
    }

    public class CustomTokenAuthHandler : AuthenticationHandler<CustomTokenAuthOptions>
    {
        private IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public CustomTokenAuthHandler(IOptionsMonitor<CustomTokenAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ApplicationDbContext context, UserManager<ApplicationUser> usrMgr, IConfiguration config)
    : base(options, logger, encoder, clock) {
            _context = context;
            _userManager = usrMgr;
            this._config = config;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var username = "";
            if (_config["lockYN"] != "N")
            {
                if (!Request.Headers.ContainsKey(Options.TokenHeaderName))
                    return Task.FromResult(AuthenticateResult.Fail($"Missing Header For Token: {Options.TokenHeaderName}"));
                var token = Request.Headers[Options.TokenHeaderName];
                if(token.ToString().Contains("OLMapAPI "))
                {
                    // get username from db or somewhere else accordining to this token
                    AuthFunc auth = new AuthFunc(_context, _userManager, _config);
                    username = auth.validatesToken(token.First());
                }
                else
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Token Type!"));
                }
            }
            if (username != "" || _config["lockYN"] == "N")
            {
                var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username),
                // add other claims/roles as you like
                };
                var id = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(id);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            return Task.FromResult(AuthenticateResult.Fail("UnAuthorized"));
        }
    }
}
