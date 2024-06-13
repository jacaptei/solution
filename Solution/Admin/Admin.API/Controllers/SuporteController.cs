using Microsoft.AspNetCore.Mvc;
using JaCaptei.Model;
using System.Diagnostics;
using JaCaptei.Application;

namespace JaCaptei.Administrativo.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class SuporteController : ApiControllerBase{


        SuporteService      suporteService      = new SuporteService();
        LocalidadeService   localidadeService   = new LocalidadeService();
        Mail mailService = new Mail();


        [HttpGet]
        [Route("modelos/obter")]
        public async Task<IActionResult> ObterModelos() {
            IDictionary<string,dynamic> dicModels = new SuporteService().ObterModelos();
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
        [Route("estado/obter/id/{nome}")]
        public IActionResult ObterIdEstado(string nome) {
            appReturn = localidadeService.ObterIdEstado(nome);
            return Result(appReturn);
        }



        [HttpGet]
        [Route("cidades/obter/{id_estado}")]
        public IActionResult ObterCidadesPorEstadoId(int id_estado) {
            appReturn = localidadeService.ObterCidadesPorEstadoId(id_estado);
            return Result(appReturn);
        }
        [HttpGet]
        [Route("cidade/obter/id/{idEstado:int}/{nome}")]
        public IActionResult ObterIdCidade(int idEstado, string nome) {
            appReturn = localidadeService.ObterIdCidade(idEstado,nome);
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
        [Route("bairro/obter/id/{idCidade:int}/{nome}")]
        public IActionResult ObterIdBairro(int idCidade,string nome) {
            appReturn = localidadeService.ObterIdBairro(idCidade,nome);
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

