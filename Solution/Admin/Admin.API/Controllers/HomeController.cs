using Microsoft.AspNetCore.Mvc;
using JaCaptei.Model;
using System.Diagnostics;
using System.Globalization;
using JaCaptei.Application;

namespace JaCaptei.Administrativo.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class HomeController : ApiControllerBase {

        /*
        [Route("/[action]")]
        public IActionResult Index() {
            return Result(appReturn);
        }
        */

        [Route("/")]
        [Route("/info")]
        public IActionResult Info() {
            appReturn.result = "API OK";
            return Result(appReturn);
        }

        [Route("/modelos")]
        [Route("/ObterModelos")]
        public  async Task<IActionResult> ObterModelos() {

            Usuario user     = new Usuario { username = "",senha = "" };

            IDictionary<string,dynamic> dicModels = new SuporteService().ObterModelos(user);

            return Ok(dicModels);

        }
            

        [Route("/KeepCRMsession")]
        [HttpGet]
        public async Task<IActionResult> KeepCRMsession() {
            return Ok(new { success = true, sessao = "" });
        }


















    }
}

