using Microsoft.AspNetCore.Mvc;
using JaCaptei.Model;
using System.Diagnostics;
using System.Globalization;
using JaCaptei.Application;

namespace JaCaptei.API.Controllers {

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

            string sessaoCRM = await CRM.ObterSessaoGlobal();
            Usuario user     = new Usuario { username = "",senha = "",sessaoCRMglobal = sessaoCRM };

            IDictionary<string,dynamic> dicModels = new SuporteService().ObterModelos(user);
            /*
            IDictionary<string,dynamic> dicModels = new Dictionary<string,dynamic>();
            dicModels.Add("log"         ,   new Log()       );
            dicModels.Add("imovelBusca" ,   new ImovelBusca { usuario = user });
            dicModels.Add("usuario"     ,   user);
            dicModels.Add("imovel"      ,   new Imovel());
            dicModels.Add("favorito"    ,   new ImovelFavorito());
             */  
            //appReturn.result = dicModels;
            //return Result(appReturn);

            return Ok(dicModels);

        }
            

        [Route("/KeepCRMsession")]
        [HttpGet]
        public async Task<IActionResult> KeepCRMsession() {
            string sessaoCRM = await CRM.ObterSessaoGlobal();
            return Ok(new { success = true, sessao = sessaoCRM });
        }


















    }
}

