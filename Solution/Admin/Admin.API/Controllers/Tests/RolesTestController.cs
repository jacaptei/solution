using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static JaCaptei.Model.Enums;

namespace JaCaptei.Administrativo.API.Controllers {

    //[Area("Req")]
    [ApiController]
    [Route("[controller]")]
    public class RolesTestController : ApiControllerBase
    {

        [HttpGet]
        public IActionResult Index()
        {
            return Result();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Req1(){
            appReturn.AddNote("NO ROLES REQUEST");
            appReturn.result = ObterUsuarioAutenticado();
            return Result(appReturn);
        }

        [Authorize(Roles = "GOD,SUPER,DEFAULT")]
        [Route("req2")]
        public IActionResult Req2()
        {
            appReturn.AddNote("ROLE = ADMIN");
            appReturn.result = ObterUsuarioAutenticado();
            return Result(appReturn);
        }

        [Authorize(Roles = "GOD,SUPER")]
        [Route("req3")]
        public IActionResult Req3()
        {
            appReturn.AddNote("ROLE = SUPER");
            appReturn.result = ObterUsuarioAutenticado();
            return Result(appReturn);
        }

        [Authorize(Roles = "GOD")]
        [Route("req4")]
        public IActionResult Req4()
        {
            appReturn.AddNote("ROLE = GOD");
            appReturn.result = ObterUsuarioAutenticado();
            return Result(appReturn);
        }

        [Authorize(Roles = "SUPER,GOD")]
        [Route("req5")]
        public IActionResult Req5()
        {
            appReturn.AddNote("ROLE = SUPER,GOD");
            appReturn.result = ObterUsuarioAutenticado();
            return Result(appReturn);
        }


    }
}
