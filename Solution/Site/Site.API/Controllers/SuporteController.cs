using Microsoft.AspNetCore.Mvc;
using JaCaptei.Model;
using System.Diagnostics;
using JaCaptei.Application;

namespace JaCaptei.API.Controllers{

    [ApiController]
    [Route("[controller]")]
    public class SuporteController : ApiControllerBase{


        SuporteService      suporteService      = new SuporteService();
        LocalidadeService   localidadeService   = new LocalidadeService();
        Mail mailService = new Mail();


        [HttpGet]
        [Route("modelos/obter")]
        public async Task<IActionResult> ObterModelos() {

            string sessaoCRM = await CRM.ObterSessaoGlobal();
            Usuario user     = new Usuario { username = "",senha = "",sessaoCRMglobal = sessaoCRM };

            IDictionary<string,dynamic> dicModels = new SuporteService().ObterModelos(user);

            return Ok(dicModels);

        }


        [HttpPost]
        [Route("log/registrar")]
        public void RegistrarLog([FromBody] Log log) {
            //if (Config.settings.enableLog)
            //    suporteService.RegistrarLog(log);
        }


        [HttpGet]
        [Route("estados/obter")]
        public IActionResult ObterEstados() {
            appReturn = localidadeService.ObterEstados();
            return Result(appReturn);
        }



        [HttpGet]
        [Route("cidades/obter/{id_estado}")]
        public IActionResult ObterCidadesPorEstadoId(int id_estado) {
            appReturn = localidadeService.ObterCidadesPorEstadoId(id_estado);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("cidades/obter/uf/{uf}")]
        public IActionResult ObterCidadesPorUF(string uf) {
            appReturn = localidadeService.ObterCidadesPorUF(uf);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("bairros/obter/{id_cidade}")]
        public IActionResult ObterBairrosPorCidadeId(int id_cidade) {
            appReturn = localidadeService.ObterBairrosPorCidadeId(id_cidade);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("bairros/obter/cidade/{nome}")]
        public IActionResult ObterBairrosPorCidadeNome(string nome) {
            appReturn = localidadeService.ObterBairrosPorCidadeNome(nome);
            return Result(appReturn);
        }





    }

}

