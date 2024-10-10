using Microsoft.AspNetCore.Mvc;
using JaCaptei.Application;
using JaCaptei.Model;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JaCaptei.Administrativo.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
    public class ParceiroController : ApiControllerBase
    {
        ParceiroService service = new ParceiroService();

        [HttpPost]
        [Route("[action]")]
        public IActionResult Adicionar([FromBody] Parceiro entity)
        {

            Model.Admin logado = ObterAdminAutenticado();
            entity.inseridoPorId = entity.atualizadoPorId = logado.id;
            entity.inseridoPorNome = entity.atualizadoPorNome = logado.nome;

            appReturn = service.Adicionar(entity);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("pendentes")]
        public IActionResult ObterPendentesValidacao()
        {
            appReturn = service.ObterPendentesValidacao();
            return Result(appReturn);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public IActionResult Confirmar([FromBody] Parceiro entity)
        {
            if (entity is null)
                appReturn.AddValidationNote("<b>Parceiro</b> inexistente ou inválido");
            else if (entity.token != Config.settings.token)
                appReturn.AddValidationNote("Token inválido");

            if (appReturn.status.success)
                appReturn.result = service.Ativar(entity);

            return Result(appReturn);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public IActionResult Validar([FromBody] Parceiro entity)
        {
            if (entity is null)
            {
                appReturn.AddException("Parceiro inexistente ou inválido");
                return Result(appReturn);
            }

            Usuario logado = ObterUsuarioAutenticado();
            entity.atualizadoPorId = logado.id;
            entity.atualizadoPorNome = logado.nome;
            entity.atualizadoPorPerfil = logado.roles;

            if (appReturn.status.success)
                appReturn.result = service.Validar(entity);

            return Result(appReturn);
        }


        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public IActionResult Ativar([FromBody] Parceiro entity)
        {
            if (entity is null)
            {
                appReturn.AddValidationNote("<b>Parceiro</b> inexistente ou inválido");
                return Result(appReturn);
            }
            Usuario logado = ObterUsuarioAutenticado();
            entity.atualizadoPorId = logado.id;
            entity.atualizadoPorNome = logado.nome;
            entity.atualizadoPorPerfil = logado.roles;

            appReturn.result = service.Ativar(entity);
            return Result(appReturn);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public IActionResult Desativar([FromBody] Parceiro entity)
        {

            if (entity is null)
            {
                appReturn.AddValidationNote("<b>Parceiro</b> inexistente ou inválido");
                return Result(appReturn);
            }

            Usuario logado = ObterUsuarioAutenticado();
            entity.atualizadoPorId = logado.id;
            entity.atualizadoPorNome = logado.nome;
            entity.atualizadoPorPerfil = logado.roles;

            appReturn.result = service.Desativar(entity);
            return Result(appReturn);

        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public async Task<IActionResult> Excluir([FromBody] Parceiro entity)
        {

            if (entity is null)
                appReturn.AddValidationNote("<b>Parceiro</b> inexistente ou inválido");

            if (appReturn.status.success)
                appReturn = service.Excluir(entity);

            return Result(appReturn);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Alterar([FromBody] Parceiro entity)
        {

            if (entity is null)
            {
                appReturn.AddException("Parceiro inexistente ou inválido");
                return Result(appReturn);
            }

            Model.Admin logado = ObterAdminAutenticado();
            entity.atualizadoPorId = logado.id;
            entity.atualizadoPorNome = logado.nome;

            appReturn = service.Alterar(entity);
            return Result(appReturn);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public IActionResult AtualizarConfiguracoesConta([FromBody] ConfiguracoesContaRequest request)
        {
            if (request == null || request.tableData == null || !request.tableData.Any())
            {
                appReturn.AddException("Parceiro inexistente ou inválido");
                return Result(appReturn);
            }

            Model.Admin operador = ObterAdminAutenticado();

            foreach (var item in request.tableData)
            {
                appReturn = service.AtualizarConfiguracoesConta(item, operador);
                return Result(appReturn);
            }

            return Result(appReturn);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public IActionResult Obter(string id)
        {
            appReturn = service.ObterPeloId(int.Parse(id));
            return Result(appReturn);
        }

        [HttpGet]
        [Route("[action]/{idConta}")]
        public IActionResult ObterContaPorId(string idConta)
        {
            appReturn = service.ObterContaPorId(int.Parse(idConta));
            return Result(appReturn);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Buscar([FromBody] Busca busca)
        {
            busca.item = JsonConvert.DeserializeObject<Parceiro>(busca.item.ToString());
            // busca.item = JObject.Parse(busca.item);
            appReturn = service.Buscar(busca);
            return Result(appReturn);
        }
    }
}
