using JaCaptei.Application;
using JaCaptei.Model;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace JaCaptei.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    //[Authorize(Roles = "ADMIN_GOD,ADMIN_GESTOR,ADMIN_PADRAO,PARCEIRO,PESSOA")]
    public class SolicitacaoController:ApiControllerBase {

        SolicitacaoService service = new SolicitacaoService();

        [HttpPost]
        [Route("adicionar")]
        public IActionResult Adicionar([FromBody] Solicitacao entity) {


            if(entity is null) {
                appReturn.AddException("Solicitação inexistente ou inválida");
                return Result(appReturn);
            }

            bool dayoff = false;
            if(dayoff) {
                appReturn.AddException("O atendimento das solicitações de hoje estão suspensas ou já foram encerradas.");
                return Result(appReturn);
            }

            Usuario logado = ObterUsuarioAutenticado();

            if(logado is null) {
                appReturn.AddException("Erro ao atender requisição.<br>Atualize o site limpando o cache de seu navegador e/ou tentando com CTRL + F5 (se o problema persistir favor entrar em contato).");
                //appReturn.result = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                return Result(appReturn);
            }

            entity.idParceiro           = entity.inseridoPorId        = entity.atualizadoPorId      = logado.id;
            entity.inseridoPorNome      = entity.atualizadoPorNome    = logado.nome;

            appReturn = service.Adicionar(entity);
            return Result(appReturn);
        }

        
        [HttpPost]
        [Route("agendar")]
        public IActionResult Agendar([FromBody] Solicitacao entity) {


            if(entity is null) {
                appReturn.AddException("Agendamento inexistente ou inválido");
                return Result(appReturn);
            }

            bool dayoff = false;
            if(dayoff) {
                appReturn.AddException("O atendimento dos agendamentos de hoje estão suspensas ou já foram encerrados.");
                return Result(appReturn);
            }

            Usuario logado = ObterUsuarioAutenticado();

            if(logado is null) {
                appReturn.AddException("Erro ao atender requisição.<br>Atualize o site limpando o cache de seu navegador e/ou tentando com CTRL + F5 (se o problema persistir favor entrar em contato).");
                //appReturn.result = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                return Result(appReturn);
            }

            entity.idParceiro           = entity.inseridoPorId        = entity.atualizadoPorId      = logado.id;
            entity.inseridoPorNome      = entity.atualizadoPorNome    = logado.nome;

            appReturn = service.Agendar(entity);
            return Result(appReturn);
        }


        [HttpPost]
        [Route("alterar")]
        public IActionResult Alterar([FromBody] Solicitacao entity) {
            if(entity is null || entity?.id == 0 || entity?.idParceiro == 0) {
                appReturn.AddException("Solicitação inexistente ou inválida");
                return Result(appReturn);
            }

            Usuario logado              = ObterUsuarioAutenticado();
            entity.atualizadoPorId      = logado.id;
            entity.atualizadoPorNome    = logado.nome;

            appReturn = service.Alterar(entity);
            return Result(appReturn);
        }
        
        [HttpPost]
        [Route("agenda/confirmar")]
        public IActionResult ConfirmarAgendaVisita([FromBody] Solicitacao entity) {
            if(entity is null || entity?.id == 0 || entity?.idParceiro == 0) {
                appReturn.AddException("Solicitação inexistente ou inválida");
                return Result(appReturn);
            }

            Usuario logado              = ObterUsuarioAutenticado();
            entity.atualizadoPorId      = logado.id;
            entity.atualizadoPorNome    = logado.nome;

            appReturn = service.ConfirmarAgendaVisita(entity);
            return Result(appReturn);
        }


        [HttpGet]
        [Route("obter/{id}")]
        public IActionResult Obter(string id) {
            appReturn = service.ObterPeloId(int.Parse(id));
            return Result(appReturn);
        }

        [HttpGet]
        [Route("obter/logado")]
        public IActionResult ObterLogado() {
            appReturn.result = ObterUsuarioAutenticado();
            return Result(appReturn);
        }

        [HttpPost]
        [Route("buscar")]
        public IActionResult Buscar([FromBody] Busca busca) {
            busca.item = JsonConvert.DeserializeObject<Solicitacao>(busca.item.ToString());
           // busca.item = JObject.Parse(busca.item);
            appReturn = service.Buscar(busca);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("obter/todos")]
        public IActionResult ObterTodos() {
            
            Solicitacao entity = new Solicitacao();

            Usuario logado = ObterUsuarioAutenticado();
            if(logado.idTipoUsuario == 3)
                entity.idAdmin = logado.id;
            else
                entity.idParceiro = logado.id;

            appReturn = service.ObterTodasSolicitacoesPeloId(entity);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("obter/todos/parceiro")]
        public IActionResult ObterTodosParceiro() {
            Solicitacao entity = new Solicitacao();
            Usuario logado = ObterUsuarioAutenticado();
            entity.idParceiro = logado.id;

            appReturn = service.ObterTodosParceiro(entity);
            return Result(appReturn);
        }

        [HttpGet]
        [Route("obter/solicitacoes/parceiro")]
        public IActionResult ObterTodosSemVisitaParceiro() {
            Solicitacao entity = new Solicitacao();
            Usuario logado = ObterUsuarioAutenticado();
            entity.idParceiro = logado.id;

            appReturn = service.ObterTodosSemVisitaParceiro(entity);
            return Result(appReturn);
        }


        [HttpGet]
        [Route("obter/visitas/parceiro")]
        public IActionResult ObterTodosComVisitaParceiro() {
            Solicitacao entity = new Solicitacao();
            Usuario logado = ObterUsuarioAutenticado();
            entity.idParceiro = logado.id;

            appReturn = service.ObterTodosComVisitaParceiro(entity);
            return Result(appReturn);
        }




        [HttpGet]
        [Route("excluir/{id:int}")]
        public IActionResult Buscar(int id) {
            appReturn = service.Excluir(id);
            return Result(appReturn);
        }
    }
}
