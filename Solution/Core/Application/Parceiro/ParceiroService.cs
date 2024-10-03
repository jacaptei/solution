using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Application.DAL;

namespace JaCaptei.Application
{

    public class ParceiroService : ServiceBase, IDisposable
    {

        readonly ParceiroBLO BLO;
        readonly ParceiroDAO DAO;

        public ParceiroService()
        {
            BLO = new ParceiroBLO();
            DAO = new ParceiroDAO();
        }

        public ParceiroService(DBcontext context)
        {
            DAO = new ParceiroDAO(context);
        }

        public AppReturn ObterContaPorId(int idConta)
        {
            if (idConta == 0)
            {
                appReturn.SetAsBadRequest("ID não informado.");
                return appReturn;
            }

            var entities = DAO.ObterContaPorId(idConta);

            if (entities == null || !entities.Any())
            {
                appReturn.SetAsNotFound();
            }

            else
            {
                foreach (var entity in entities)
                {
                    entity.RemoverDadosSensiveis(); // Remover dados sensíveis de cada parceiro
                }
                appReturn.result = entities; // Atribui a coleção de parceiros
            }

            return appReturn;
        }

        public AppReturn ObterPeloId(int id)
        {

            if (id == 0)
            {
                appReturn.SetAsBadRequest("ID não informado.");
                return appReturn;
            }

            Parceiro entity = DAO.ObterPorId(id);

            if (entity is null || entity?.id == 0)
                appReturn.SetAsNotFound();
            else
            {
                entity.RemoverDadosSensiveis();
                appReturn.result = entity;
            }

            return appReturn;

        }

        public AppReturn AceitarTermos(int id)
        {
            appReturn = DAO.AceitarTermos(id);
            return appReturn;
        }

        public AppReturn ObterPeloToken(string token)
        {

            if (Utils.Validator.Not(token))
            {
                appReturn.SetAsBadRequest("Token não informado.");
                return appReturn;
            }

            Parceiro entity = DAO.ObterPeloToken(token);

            if (entity is null || entity?.id == 0)
                appReturn.SetAsNotFound("Parceiro não encontrado");
            else
            {
                entity.RemoverDadosSensiveis();
                appReturn.result = entity;
            }

            return appReturn;

        }




        public AppReturn ObterPeloDocumentoOuEmail(Parceiro entity)
        {

            appReturn = BLO.ValidarDadosChaves(entity);

            if (!appReturn.status.success)
                return appReturn;

            entity = DAO.ObterPorDocumentoOuEmail(entity);

            if (entity is null || entity?.id == 0)
                appReturn.SetAsNotFound("Parceiro não encontrado");
            else
            {
                entity.RemoverDadosSensiveis();
                appReturn.result = entity;
            }

            return appReturn;

        }



        public AppReturn Obter(Parceiro entity)
        {
            return ObterPeloDocumentoOuEmail(entity);
        }



        public AppReturn Adicionar(Parceiro entity)
        {

            appReturn = BLO.Validar(entity);

            if (!appReturn.status.success)
                return appReturn;

            entity = BLO.Normalizar(entity);

            Parceiro entityDB = DAO.ObterPorCamposChaves(entity);

            if (entityDB is not null && entityDB?.id > 0)
            {
                if (!entityDB.confirmado)
                    appReturn.AddException("Já existe um Parceiro cadastrado com este CPF, CNPJ ou E-mail. Se já fez o cadastro, não esqueça de o confirmar através do link enviado pelo e-mail para ter o acesso liberado.");
                else if (!entityDB.validado)
                    appReturn.AddException("Aguarde a liberação de seu acesso (será notificado via e-mail.");
                else if (entityDB.excluido)
                    appReturn.AddException("Login indisponível. Entre em contato e verifique se sua conta ainda é válida.");
                else if (!entityDB.ativo || !entityDB.ativoCRM)
                    appReturn.AddException("Acesso indisponível. Entre em contato e verifique se sua conta ainda está ativa.");
                else
                    appReturn.AddException("Já existe um Parceiro cadastrado com este CPF, CNPJ ou E-mail.");

                return appReturn;

            }

            try
            {
                LocalidadeService localidade = new LocalidadeService();
                if (entity.idEstado == 0)
                    entity.idEstado = (localidade.ObterIdEstado(entity.estado)).result.id;
                if (entity.idCidade == 0)
                    entity.idCidade = (localidade.ObterIdCidade(entity.idEstado, entity.cidade)).result.id;
                if (entity.idBairro == 0)
                    entity.idBairro = (localidade.ObterIdBairro(entity.idCidade, entity.bairro)).result.id;
            }
            catch (Exception ex) { }

            appReturn = DAO.Adicionar(entity);

            if (appReturn.status.success){
                Mail mail = new Mail();
                mail.emailTo = entity.email;
                mail.about = "Confirme seu cadastro";
                mail.message = "Olá " + entity.apelido + ".<br><br>Clique (ou copie e cole no navegador) o link abaixo para confirmar seu cadastro:<br><a href='" + Config.settings.baseURL + "/confirma?t=" + entity.token + "' target='_blank' style='color:#ef5924'>" + Config.settings.baseURL + "/confirma?t=" + entity.token + "</a>";
                mail.Send();
            }

            return appReturn;
        }





        public AppReturn Confirmar(string token)
        {
            appReturn = DAO.Confirmar(token);
            return appReturn;
        }





        public AppReturn Validar(Parceiro entity)
        {

            if (entity is null || entity?.id == 0)
            {
                appReturn.AddException("Parceiro não identificado.");
                return appReturn;
            }

            appReturn = DAO.Validar(entity);

            if (appReturn.status.success)
            {
                Mail mail = new Mail();
                mail.emailTo = entity.email;
                mail.about = "Seja Bem " + (entity.sexo == "FEMININO" ? "Vinda" : "Vindo") + "!";
                //mail.message    = "Seja bem " + (entity.sexo == "FEMININO" ? "vinda" : "vindo") + " "+ Utils.String.Capitalize(entity.nome.Split(' ')[0]) + " à <b style='color:#ef5924'>JáCaptei</b>.<br><br>Seu acesso estará liberado logo após o pagamento da assinatura. <br>Para isso, basta acessar o link abaixo e aproveitar todos os benefícios JáCaptei.<br><a href='https://www.asaas.com/c/a6w4nl67dwjhqhq7' target='_blank' style='color:#ef5924'>https://www.asaas.com/c/a6w4nl67dwjhqhq7</a>";
                mail.message = "Seja bem " + (entity.sexo == "FEMININO" ? "vinda" : "vindo") + " " + entity.apelido + " à <b style='color:#ef5924'>JáCaptei</b>.<br><br>Seu acesso está liberado!";
                if (entity.donoConta)
                {
                    mail.message += "<br><br>Entraremos em contato para cuidar da concretização de seu plano.";
                    mail.message += "<br><br>Passe o seguinte <b>TOKEN DA CONTA</b> abaixo para que seus colaboradores possam se cadastrar na sua conta (devem selecionar, na tela de cadastro do site, a opção \"INSERIR TOKEN DE UMA CONTA EXISTENTE\").";
                    mail.message += "<br><br><b>" + entity.tokenConta + "</b>";
                }
                mail.Send();
            }
            return appReturn;
        }



        public AppReturn Ativar(Parceiro entity)
        {

            if (entity is null || entity?.id == 0)
            {
                appReturn.AddException("Parceiro não identificado.");
                return appReturn;
            }

            appReturn = DAO.Ativar(entity);
            /*
            if(appReturn.status.success) {
                Mail mail = new Mail();
                mail.emailTo    = entity.email;
                mail.about      = "Seja Bem " + (entity.sexo == "FEMININO" ? "Vinda" : "Vindo") + "!";
                //mail.message    = "Seja bem " + (entity.sexo == "FEMININO" ? "vinda" : "vindo") + " "+ Utils.String.Capitalize(entity.nome.Split(' ')[0]) + " à <b style='color:#ef5924'>JáCaptei</b>.<br><br>Seu acesso estará liberado logo após o pagamento da assinatura. <br>Para isso, basta acessar o link abaixo e aproveitar todos os benefícios JáCaptei.<br><a href='https://www.asaas.com/c/a6w4nl67dwjhqhq7' target='_blank' style='color:#ef5924'>https://www.asaas.com/c/a6w4nl67dwjhqhq7</a>";
                mail.message    = "Seja bem " + (entity.sexo == "FEMININO" ? "vinda" : "vindo") + " "+ entity.apelido + " à <b style='color:#ef5924'>JáCaptei</b>.<br><br>Seu acesso está liberado!";
                if(entity.donoConta) { 
                    mail.message    += "<br><br>Entraremos em contato para cuidar da concretização de seu plano.";
                    mail.message    += "<br><br>Passe o seguinte <b>TOKEN DA CONTA</b> abaixo para que seus colaboradores possam se cadastrar na sua conta (devem selecionar, na tela de cadastro do site, a opção \"INSERIR TOKEN DE UMA CONTA EXISTENTE\").";
                    mail.message    += "<br><br><b>" + entity.tokenConta+"</b>";
                }
                mail.Send();
            }
            */
            return appReturn;
        }



        public AppReturn Desativar(Parceiro entity)
        {

            if (entity is null || entity?.id == 0)
            {
                appReturn.AddException("Parceiro não identificado.");
                return appReturn;
            }

            appReturn = DAO.Desativar(entity);

            return appReturn;

        }



        public AppReturn Alterar(Parceiro entity)
        {

            if (entity is null || entity?.id == 0)
            {
                appReturn.AddException("Parceiro não identificado.");
                return appReturn;
            }

            appReturn = BLO.ValidarAlteracao(entity);

            if (!appReturn.status.success)
                return appReturn;

            entity = BLO.NormalizarAlteracao(entity);

            Parceiro parceiroExiste = DAO.ObterPorCamposChavesParaAlteracao(entity);

            if (parceiroExiste is not null && parceiroExiste?.id > 0)
            {
                appReturn.AddException("Já existe um Parceiro cadastrado com este documento (CPF ou CNPJ) ou E-mail.");
                return appReturn;
            }


            try
            {
                LocalidadeService localidade = new LocalidadeService();
                if (entity.idEstado == 0)
                    entity.idEstado = (localidade.ObterIdEstado(entity.estado)).result.id;
                if (entity.idCidade == 0)
                    entity.idCidade = (localidade.ObterIdCidade(entity.idEstado, entity.cidade)).result.id;
                if (entity.idBairro == 0)
                    entity.idBairro = (localidade.ObterIdBairro(entity.idCidade, entity.bairro)).result.id;
            }
            catch (Exception ex) { }


            appReturn = DAO.Alterar(entity);

            return appReturn;
        }



        public AppReturn Excluir(Parceiro entity)
        {
            appReturn = DAO.Excluir(entity);
            return appReturn;
        }






        public AppReturn AlterarSenha(Parceiro entity)
        {

            if (entity is null || Utils.Validator.Not(entity?.token))
            {
                appReturn.AddException("Parceiro não identificado.");
                return appReturn;
            }
            else if (Utils.Validator.Not(entity.senha))
            {
                appReturn.AddException("Senha não informada.");
                return appReturn;
            }

            entity.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());

            appReturn = DAO.AlterarSenha(entity);

            return appReturn;

        }


        public AppReturn AlterarPerfil(Parceiro entity)
        {

            if (entity is null || entity?.id == 0 || Utils.Validator.Not(entity?.token))
            {
                appReturn.AddException("Parceiro não identificado.");
                return appReturn;
            }
            else
            {
                if (Utils.Validator.Is(entity.email))
                {
                    entity.email = Utils.String.HigienizeMail(entity.email);
                    Parceiro entityDB = DAO.ObterPorDocumentoOuEmail(entity);
                    if (entityDB is not null)
                    {
                        if (entityDB.token != entity.token)
                        {
                            appReturn.AddException("Já existe um usuário cadastrado com este E-Mail");
                            return appReturn;
                        }
                    }
                }
            }

            appReturn = DAO.AlterarPerfil(entity);

            return appReturn;

        }




        public AppReturn ObterPorDocumentoOuEmail(Parceiro entity)
        {

            entity = DAO.ObterPorDocumentoOuEmail(entity);

            if (entity is null || entity?.id == 0)
                appReturn.AddException("Nada encontrado.");
            else
                appReturn.result = entity;

            return appReturn;

        }





        public AppReturn Autenticar(Parceiro entity)
        {

            appReturn = BLO.ValidarDadosLogin(entity);

            if (!appReturn.status.success)
                return appReturn;

            appReturn = DAO.Autenticar(entity);

            entity = appReturn.result;

            if (entity is null)
                appReturn.SetAsNotFound("Parceiro não encontrado.");
            else
            {
                entity.RemoverDadosSensiveis();
                if (!entity.ativo)
                    appReturn.SetAsGone("Parceiro não ativo.");
                else
                {
                    appReturn.result = entity;
                }
            }
            return appReturn;
        }




        public AppReturn ObterPendentesValidacao()
        {

            List<Parceiro> entities = DAO.ObterPendentesValidacao();

            appReturn.result = entities;

            return appReturn;

        }
        public async Task<List<ParceiroList>> ObterParceirosAtivos()
        {
            try
            {
                var parceirosAtivos = await DAO.ObterParceirosAtivos();
                if (parceirosAtivos == null || !parceirosAtivos.Any())
                {
                    return new List<ParceiroList>();
                }
                return parceirosAtivos;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao obter parceiros ativos.", ex);
            }
        }

        public AppReturn ObterInativos()
        {

            List<Parceiro> entities = DAO.ObterInativos();

            appReturn.result = entities;

            return appReturn;

        }






        public AppReturn Buscar(Busca busca)
        {
            busca.item = BLO.NormalizarParaBusca(busca.item);
            appReturn = DAO.Buscar(busca);
            return appReturn;
        }

        public Task<Parceiro?> ObterPorCPF(string cpf)
        {
            return DAO.ObterPorCPF(cpf);
        }

        public Task<Parceiro?> ObterPorCNPJ(string cnpj)
        {
            return DAO.ObterPorCNPJ(cnpj);
        }

        public Task<Plano?> ObterPlanoParceiro(Parceiro parceiro)
        {
            return DAO.ObterPlanoParceiro(parceiro);
        }

        public void Dispose()
        {
            DAO.Dispose();
        }
        /*
       public AppReturn ObterViaCPF(Shared.Model.Parceiro entity){

           return DAO.ObterViaToken(entity);
       }

       public AppReturn ObterViaToken(Shared.Model.Parceiro entity){
           return DAO.ObterViaToken(entity);
       }

       public AppReturn ObterViaTokenUID(Shared.Model.Parceiro entity){
           return DAO.ObterViaTokenUID(entity);
       }
*/



    }
}
