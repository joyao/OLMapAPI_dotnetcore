using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OLMapAPI_Core_PoC.Infrastructure.BasicInfo;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OLMapAPI_Core_PoC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayersController : ControllerBase
    {
        private IConfiguration _config;
        public LayersController(IConfiguration config)
        {
            this._config = config;
        }


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
