using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OLMapAPI_Core_PoC.Infrastructure.BasicInfo;
using OLMapAPI_Core_PoC.MessageHandler;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OLMapAPI_Core_PoC.Controllers
{
    /// <summary>
    /// A controller performs operations on Layers
    /// </summary>
    [Authorize]
    //[Authorize(AuthenticationSchemes = CustomTokenAuthOptions.DefaultScemeName)]
    [Route("api/[controller]")]
    [ApiController]
    public class LayersController : ControllerBase
    {
        private IConfiguration _config;
        public LayersController(IConfiguration config)
        {
            this._config = config;
        }

        /// <summary>
        /// To Get Layer List
        /// </summary>
        /// <remarks>Awesomeness!</remarks>
        /// <response code="200">Return List data</response>
        /// <response code="400">Product has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your product right now</response>
        // GET api/<LayersController>/getLayerResource
        [HttpGet("getLayerResource")]
        public async Task<ActionResult> getLayerResource()
        {
            try
            {
                BasicInfoFunc b = new BasicInfoFunc(_config);

                return Ok(await b.getLayerResourceList());

            }
            catch (Exception SqlException)
            {
                #if (DEBUG)
                    return StatusCode(StatusCodes.Status500InternalServerError, SqlException.Message);
                #else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
                #endif
            }

        }
    }
}
