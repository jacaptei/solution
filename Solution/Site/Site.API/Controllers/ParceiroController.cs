using Microsoft.AspNetCore.Mvc;
using JaCaptei.Services;
using System.Numerics;
using JaCaptei.Application;
using JaCaptei.Model;

namespace JaCaptei.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class ParceiroController:ApiControllerBase {

        ParceiroService service = new ParceiroService();


        [HttpPost]
        [Route("inserir")]
        public IActionResult Inserir([FromBody] Parceiro entity) {
            appReturn = service.Adicionar(entity);
            return Result(appReturn);
        }

        [HttpPost]
        [Route("adicionar")]
        public IActionResult Adicionar([FromBody] Parceiro entity) {
            appReturn = service.Adicionar(entity);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("confirmar/{token}")]
        public IActionResult Confirmar(string token) {
            appReturn = service.Confirmar(token);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("termos/aceitar")]
        public IActionResult AceitarTermos()
        {
            Usuario user = ObterUsuarioAutenticado();
            appReturn = service.AceitarTermos(user.id);
            return Result(user);
        }

        [HttpPost]
        [Route("autenticar")]
        public async Task<IActionResult> Autenticar([FromBody] Parceiro entity) {

            appReturn = service.Autenticar(entity);

            if(appReturn.status.success) {
                entity              = appReturn.result;
                entity.roles        = "PARCEIRO";
                entity.tokenJWT     = JWTokenService.GenerateToken(entity);
                appReturn.result    = entity;
                //appReturn.result    = entity;
            }


            return Result(appReturn);

        }





        // parte do Admin
        // JWT Token Validator
        [HttpPost]
        [Route("ativar")]
        public IActionResult Ativar([FromBody] Parceiro entity) {
            if(entity is null) {
                appReturn.AddException("Parceiro inexistente ou inválido");
                return Result(appReturn);
            }
            appReturn = service.Ativar(entity);
            return Result(appReturn);
        }




        [HttpPost]
        [Route("senha/solicitar")]
        public async Task<IActionResult> SolicitarAlterarSenha([FromBody] Parceiro entity) {
            string msg = "";
            bool res = false;
            Parceiro entityDB = (Parceiro) service.ObterPeloDocumentoOuEmail(entity).result;
             if(entityDB is not null) {
                Mail mail    = new Mail();
                mail.emailTo = entityDB.email;
                mail.about   = "Recuperação de senha";
                var nome = (entityDB.tipoPessoa == "PF")? Utils.String.Capitalize(entityDB.nome.Split(' ')[0]) : entityDB.razao;
                mail.message = "Olá " + nome + ".<br><br>Clique (ou copie e cole no navegador) o link abaixo para alterar sua senha:<br><a href='"+ Config.settings.baseURL +"/senha?t=" + entityDB.token + "' target='_blank' style='color:#ef5924'>"+ Config.settings.baseURL +"/senha?t=" + entityDB.token + "</a>";
                mail.Send();
                res = true;
            }
            return Result(appReturn);
        }


        [HttpPost]
        [Route("senha/alterar")]
        public IActionResult AlterarSenha([FromBody] Parceiro entity) {
            if(entity is null) {
                appReturn.AddException("Parceiro inexistente ou inválido");
                return Result(appReturn);
            }
            appReturn = service.AlterarSenha(entity);
            return Result(appReturn);
        }
        

        [HttpPost]
        [Route("perfil/alterar")]
        public IActionResult AlterarPerfil([FromBody] Parceiro entity) {
            if(entity is null) {
                appReturn.AddException("Parceiro inexistente ou inválido");
                return Result(appReturn);
            }
            appReturn = service.AlterarSenha(entity);
            return Result(appReturn);
        }



        [HttpGet]
        [Route("obter/{id}")]
        public IActionResult Obter(string id) {
            appReturn = service.ObterPeloId(int.Parse(id));
            return Result(appReturn);
        }















    }
}
