using Microsoft.AspNetCore.Mvc;
using JaCaptei.Model;
using System.Diagnostics;
using JaCaptei.Application;

namespace JaCaptei.API.Controllers{

    [ApiController]
    [Route("[controller]")]
    public class AdminController : ApiControllerBase{


        ParceiroService parceiroService = new ParceiroService();


        [HttpGet]
        [Route("parceiros/inativos/{token}")]
        public IActionResult ObterParceirosInativos(string token) {
            if(Config.settings.token == token)
                appReturn = parceiroService.ObterInativos();
            else
                appReturn.AddValidationNote("Token admin inválido");
            return Result(appReturn);
        }








        [HttpPost]
        [Route("parceiro/ativar")]
        public async Task<IActionResult> AtivarParceiro([FromBody] Parceiro entity) {

            if(entity is null)
                appReturn.AddValidationNote("<b>Parceiro</b> inexistente ou inválido");
            else if(entity.token != Config.settings.token)
                appReturn.AddValidationNote("Token inválido");
            
            if(appReturn.status.success) 
                appReturn.result = parceiroService.Ativar(entity);

            return Result(appReturn);

        }



        [HttpPost]
        [Route("parceiro/desativar")]
        public async Task<IActionResult> DesativarParceiro([FromBody] Parceiro entity) {

            if(entity is null)
                appReturn.AddValidationNote("<b>Parceiro</b> inexistente ou inválido");
            else if(entity.token != Config.settings.token)
                appReturn.AddValidationNote("Token inválido");
            
            if(appReturn.status.success) 
                appReturn.result = parceiroService.Desativar(entity);

            return Result(appReturn);

        }



        [HttpPost]
        [Route("parceiro/alterar")]
        public async Task<IActionResult> AlterarParceiro([FromBody] Parceiro entity) {

            if(entity is null)
                appReturn.AddValidationNote("<b>Parceiro</b> inexistente ou inválido");
            else if(entity.token != Config.settings.token)
                appReturn.AddValidationNote("Token inválido");
            
            if(appReturn.status.success) 
                appReturn.result = parceiroService.Alterar(entity);

            return Result(appReturn);

        }



        [HttpPost]
        [Route("parceiro/obter")]
        public async Task<IActionResult> ObterParceiro([FromBody] Parceiro entity) {

            if(entity.token != Config.settings.token)
                appReturn.AddValidationNote("Token inválido");
            else if(entity is null)
                appReturn.AddValidationNote("Parceiro inexistente ou inválido");
            else { 
                if(Utils.Validator.Not(entity.username))
                    appReturn.AddValidationNote("CPF, CNPJ ou E-Mail não informado");
            }

            if(appReturn.status.success) 
                appReturn = parceiroService.ObterPorDocumentoOuEmail(entity);

            return Result(appReturn);

        }












    }

}

