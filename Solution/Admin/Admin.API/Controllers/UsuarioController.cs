using Microsoft.AspNetCore.Mvc;
using JaCaptei.Application;
using JaCaptei.Model;
using Microsoft.AspNetCore.Authorization;

namespace JaCaptei.Administrativo.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class UsuarioController:ApiControllerBase {

        UsuarioAdminService service = new UsuarioAdminService();


        [HttpPost]
        [Route("inserir")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public IActionResult Inserir([FromBody] Model.Admin entity) {

            Model.Admin   logado              = ObterAdminAutenticado();
            entity.inseridoPorId        = entity.atualizadoPorId      = logado.id;
            entity.inseridoPorNome      = entity.atualizadoPorNome    = logado.nome;

            appReturn = service.Inserir(entity);
            return Result(appReturn);
        }


        [HttpPost]
        [Route("autenticar")]
        public async Task<IActionResult> Autenticar([FromBody] Model.Admin entity) {

            appReturn = service.Autenticar(entity);

            if(appReturn.status.success) {
                entity = appReturn.result;
                entity.tokenJWT = JWTokenService.GenerateAdminToken(entity);
                //appReturn.result = await CRM.Autenticar(entity);
            }

            return Result(appReturn);

        }





        // parte do Admin
        // JWT Token Validator
        [HttpPost]
        [Route("alterar")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
        public IActionResult Alterar([FromBody] Model.Admin entity) {
            if(entity is null) {
                appReturn.AddException("Usuário inexistente ou inválido");
                return Result(appReturn);
            }

            Model.Admin   logado              = ObterAdminAutenticado();
            entity.atualizadoPorId      = logado.id;
            entity.atualizadoPorNome    = logado.nome;

            appReturn = service.Alterar(entity);
            return Result(appReturn);
        }



        // parte do Admin
        // JWT Token Validator
        [HttpPost]
        [Route("alterar/disponibilidade")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public IActionResult AlterarDisponibilidade([FromBody] Model.Admin entity) {
            if(entity is null) {
                appReturn.AddException("Usuário inexistente ou inválido");
                return Result(appReturn);
            }
            Model.Admin   logado              = ObterAdminAutenticado();
            entity.atualizadoPorId      = logado.id;
            entity.atualizadoPorNome    = logado.nome;
            appReturn = service.AlterarDisponibilidade(entity);
            return Result(appReturn);
        }




        [HttpPost]
        [Route("senha/solicitar")]
        public async Task<IActionResult> SolicitarAlterarSenha([FromBody] Model.Admin entity) {
            string msg = "";
            Model.Admin entityDB = (Model.Admin ) service.ObterPeloDocumentoOuEmail(entity).result;
            if(entityDB is not null) {
                MailAdmin mail    = new MailAdmin();
                mail.emailTo = entityDB.email;
                mail.about = "Recuperação de senha";
                mail.message = "Olá " + entityDB.apelido + ".<br><br>Clique (ou copie e cole no navegador) o link abaixo para alterar sua senha:<br><a href='" + Config.settings.baseURL + "/#/login?t=" + entityDB.token + "' target='_blank' style='color:#0072ff'>" + Config.settings.baseURL + "/#/login?t=" + entityDB.token + "</a>";
                mail.Send();
                appReturn.result = new Model.Admin { email = entityDB.email };
            }else
                appReturn.AddException("Usuário não encontrado");
            return Result(appReturn);
        }


        [HttpPost]
        [Route("senha/alterar")]
        public IActionResult AlterarSenha([FromBody] Model.Admin entity) {
            if(entity is null) {
                appReturn.AddException("Usuário inexistente ou inválido");
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
