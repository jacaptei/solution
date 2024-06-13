using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JaCaptei.Administrativo.API.Controllers {

    //[Area("Req")]
    [ApiController]
    [Route("[controller]")]
    public class RestTestController : ApiControllerBase{

        [HttpGet]
        public IActionResult Index()
        {
            return Result();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Req1()
        {
            appReturn.SetAsBadRequest();
            return Result(appReturn);
        }

        [Route("Req2")]
        public IActionResult Req2()
        {
            appReturn.SetAsUnauthorized();
            return Result(appReturn);
        }

        [Route("Req3")]
        public IActionResult Req3()
        {
            appReturn.SetAsForbidden();
            return Result(appReturn);
        }

        [Route("Req4")]
        public IActionResult Req4()
        {
            appReturn.SetAsGone();
            return Result(appReturn);
        }

        [Route("Req5")]
        public IActionResult Req5()
        {
            appReturn.SetAsNotAcceptable();
            return Result(appReturn);
        }

        [Route("Req6")]
        public IActionResult Req6()
        {
            appReturn.SetAsNotFound();
            return Result(appReturn);
        }



    }
}
