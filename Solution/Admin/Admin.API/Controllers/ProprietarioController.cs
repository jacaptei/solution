using Microsoft.AspNetCore.Mvc;
using JaCaptei.Application;
using JaCaptei.Model;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JaCaptei.Administrativo.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
    public class ProprietarioController:ApiControllerBase {

        ProprietarioService service = new ProprietarioService();

        [HttpPost]
        [Route("[action]")]
        public IActionResult Adicionar([FromBody] Proprietario entity) {

            Model.Admin logado                = ObterAdminAutenticado();
            entity.inseridoPorId        = entity.atualizadoPorId      = logado.id;
            entity.inseridoPorNome      = entity.atualizadoPorNome    = logado.nome;

            appReturn = service.Adicionar(entity);
            return Result(appReturn);
        }


        [HttpPost]
        [Route("[action]")]
        public IActionResult Alterar([FromBody] Proprietario entity) {
            if(entity is null) {
                appReturn.AddException("Proprietário inexistente ou inválido");
                return Result(appReturn);
            }
            else if(entity.id == 1) {
                appReturn.AddException("Este proprietário não pode ser editado");
                return Result(appReturn);
            }

            Model.Admin logado          = ObterAdminAutenticado();
            entity.atualizadoPorId      = logado.id;
            entity.atualizadoPorNome    = logado.nome;

            appReturn = service.Alterar(entity);
            return Result(appReturn);
        }


        [HttpGet]
        [Route("[action]/{id}")]
        public IActionResult Obter(string id) {
            appReturn = service.ObterPeloId(int.Parse(id));
            return Result(appReturn);
        }



        [HttpPost]
        [Route("buscar")]
        public IActionResult Buscar([FromBody] Busca busca) {
            busca.item = JsonConvert.DeserializeObject<Proprietario>(busca.item.ToString());
           // busca.item = JObject.Parse(busca.item);
            appReturn = service.Buscar(busca);
            return Result(appReturn);
        }


        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        public async Task<IActionResult> Excluir([FromBody] Proprietario entity) {

            if(entity is null)
                appReturn.AddValidationNote("<b>Proprietário</b> inexistente ou inválido");

            if(appReturn.status.success)
                appReturn = service.Excluir(entity);

            return Result(appReturn);

        }






    }
}
