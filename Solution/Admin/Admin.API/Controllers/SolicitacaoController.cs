using Microsoft.AspNetCore.Mvc;
using JaCaptei.Application;
using JaCaptei.Model;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JaCaptei.Administrativo.API.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class SolicitacaoController:ApiControllerBase {

        SolicitacaoService service = new SolicitacaoService();

        [HttpPost]
        [Route("adicionar")]
        public IActionResult Adicionar([FromBody] Solicitacao entity) {

            if(entity is null) {
                appReturn.AddException("Solicitação inexistente ou inválida");
                return Result(appReturn);
            }

            Usuario logado              = ObterUsuarioAutenticado();
            entity.inseridoPorId        = entity.atualizadoPorId      = logado.id;
            entity.inseridoPorNome      = entity.atualizadoPorNome    = logado.nome;
            entity.inseridoPorPerfil    = entity.atualizadoPorPerfil  = logado.roles;

            appReturn = service.Adicionar(entity);
            return Result(appReturn);
        }



        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        [HttpGet]
        [Route("obter/{id}")]
        public IActionResult ObterPeloId(string id) {
            appReturn = service.ObterPeloId(int.Parse(id));
            return Result(appReturn);
        }

        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
        [HttpPost]
        [Route("alterar")]
        public IActionResult Alterar([FromBody] Solicitacao entity) {

            if(entity is null) {
                appReturn.AddException("Solicitação inexistente ou inválida");
                return Result(appReturn);
            }

            Usuario logado = ObterUsuarioAutenticado();
            entity.atualizadoPorId      = logado.id;
            entity.atualizadoPorNome    = logado.nome;
            entity.atualizadoPorPerfil  = logado.roles;

            entity.admin = new Model.Admin { id = entity.idAdmin };

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
            appReturn = service.AlterarDisponibilidade(entity);
            return Result(appReturn);
        }




        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        [HttpGet]
        [Route("obter/distribuicoes")]
        public IActionResult Captar() {
            appReturn = service.ObterDistribuicoes();
            return Result(appReturn);
        }



        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
        [HttpPost]
        [Route("captar")]
        public IActionResult Captar([FromBody] Solicitacao entity) {
            Usuario logado = ObterUsuarioAutenticado();
            entity.admin        = new Model.Admin();
            entity.admin.id     = entity.idAdmin = logado.id;
            entity.admin.nome   = logado.nome;

            var dateUtil = new DateUtil();
            entity.dataVisita = dateUtil.ConvertToLocalDateTime(entity.dataVisita);

            appReturn = service.Captar(entity);
            return Result(appReturn);
        }


        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        [HttpPost]
        [Route("cancelar")]
        public IActionResult Cancelar([FromBody] Solicitacao entity) {
            Model.Admin logado  = ObterAdminAutenticado();
            entity.admin        = new Model.Admin();
            entity.admin.id     = entity.idAdmin = logado.id;
            entity.admin.nome   = logado.nome;
            appReturn = service.Cancelar(entity);
            return Result(appReturn);
        }


        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        [HttpPost]
        [Route("realocar")]
        public IActionResult Realocar([FromBody] Solicitacao entity) {
            //Admin logado        = ObterAdminAutenticado();
            //entity.admin        = new Admin();
            //entity.admin.id     = entity.idAdmin = logado.id;
            appReturn = service.RealocarNaFila(entity);
            return Result(appReturn);
        }
        
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        [HttpPost]
        [Route("realocar/fila")]
        public IActionResult RealocarNaFila([FromBody] Solicitacao entity) {
            //Admin logado        = ObterAdminAutenticado();
            //entity.admin        = new Admin();
            //entity.admin.id     = entity.idAdmin = logado.id;
            appReturn = service.RealocarNaFila(entity);
            return Result(appReturn);
        }
        

        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        [HttpPost]
        [Route("realocar/admin")]
        public IActionResult RealocarParaAdmin([FromBody] Solicitacao entity) {
            //Admin logado        = ObterAdminAutenticado();
            //entity.admin        = new Admin();
            //entity.admin.id     = entity.idAdmin = logado.id;
            appReturn = service.RealocarParaAdmin(entity);
            return Result(appReturn);
        }
        


        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
        [HttpPost]
        [Route("finalizar")]
        public IActionResult Finalizar([FromBody] Solicitacao entity) {
            Model.Admin logado  = ObterAdminAutenticado();
            entity.admin        = new Model.Admin();
            entity.admin.id     = entity.idAdmin = logado.id;
            entity.admin.nome   = logado.nome;

            var dateUtil = new DateUtil();
            entity.dataVisita = dateUtil.ConvertToLocalDateTime(entity.dataVisita);

            appReturn = service.Finalizar(entity);
            return Result(appReturn);
        }



        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
        [HttpPost]
        [Route("buscar")]
        public IActionResult Buscar([FromBody] Solicitacao entity) {

            Model.Admin logado = ObterAdminAutenticado();
            if(!logado.gestor)
                entity.idAdmin = logado.id;

            Search busca = new Search{item = entity };
            appReturn = service.Buscar(busca);
            return Result(appReturn);
        }

        
        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
        [HttpPost]
        [Route("buscarrange")]
        public IActionResult BuscarRange([FromBody] Search busca) {

            busca.item = JsonConvert.DeserializeObject<Solicitacao>(busca.item.ToString());

            Model.Admin logado = ObterAdminAutenticado();
            if(!logado.gestor)
                busca.item.idAdmin = logado.id;

            appReturn = service.Buscar(busca);
            return Result(appReturn);
        }



        //[Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
        //[HttpPost]
        //[Route("buscar")]
        //public IActionResult Buscar([FromBody] Search busca) {
        //    busca.item = JsonConvert.DeserializeObject<Solicitacao>(busca.item.ToString());
        //    appReturn = service.Buscar(busca);
        //    return Result(appReturn);
        //}




        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO")]
        [HttpGet]
        [Route("obter/todos/admin/{id:int}")]
        public IActionResult ObterTodosAdmin(int id) {

            Model.Admin logado = ObterAdminAutenticado();
            if(logado.god || logado.gestor)
                logado.id = id;

            appReturn = service.ObterTodosAdmin(logado);
            return Result(appReturn);
        }


        [HttpGet]
        [Route("obter/todos/parceiro")]
        public IActionResult ObterTodosParceiro(){
            Solicitacao entity = new Solicitacao();
            Usuario logado = ObterUsuarioAutenticado();
            entity.idParceiro = logado.id;

            appReturn = service.ObterTodosParceiro(entity);
            return Result(appReturn);
        }


        [Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR")]
        [HttpGet]
        [Route("excluir/{id:int}")]
        public IActionResult Excluir(int id) {
            appReturn = service.Excluir(id);
            return Result(appReturn);
        }







    }
}
