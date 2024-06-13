using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using static JaCaptei.Model.Enums;
using JaCaptei.Services;
using JaCaptei.Model;
using JaCaptei.Application;

namespace JaCaptei.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class AutenticacaoController : ApiControllerBase{

        //AutenticacaoService service         =   new AutenticacaoService();
        ParceiroService     serviceParceiro =   new ParceiroService();
        Mail mail                           =   new Mail();



        [Route("hii")]
        public IActionResult Hi() {
            return Result(appReturn);
        }



        /*
                [HttpGet]
                [Route("parceiro/obter/{token}")]
                public IActionResult ObterParceiroPeloToken(string token)
                {
                    appReturn = serviceParceiro.ObterPeloToken(token);
                    if (appReturn.status.success){
                        Parceiro entity = (Parceiro)appReturn.result;
                        entity.tokenJWT = JWTokenService.GenerateToken(entity);
                        appReturn.result = entity;
                    }
                    return Result(appReturn);
                }
        */



        /*
                [Authorize]
                [HttpPost]
                [Route("usuario/atualizar")]
                public IActionResult AtualizarPerfil([FromBody] Parceiro entity)
                {
                    if (entity == null)
                        appReturn.SetAsNotAcceptable("Usuário não identificado (talvez seja necessário novo username).");
                    else
                    {

                        appReturn = service.AtualizarPerfil(entity);

                        if (!appReturn.status.success)
                            return Result(appReturn);
                    }
                    return Result(appReturn);
                }

        */










    }
}
