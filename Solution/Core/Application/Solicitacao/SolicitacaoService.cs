using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.Services;
using JaCaptei.Model.Model;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace JaCaptei.Application{

    public class SolicitacaoService : ServiceBase{


        SolicitacaoBLO BLO = new SolicitacaoBLO();
        SolicitacaoDAO DAO = new SolicitacaoDAO();


        public AppReturn Adicionar(Solicitacao entity) {

            entity.data                 =
            entity.dataAtualizacao      =
            entity.dataConsiderada      =   Utils.Date.GetLocalDateTime();

            short dayWeek = (short)entity.data.DayOfWeek;
            if(dayWeek == 7 || entity.data.Hour >= 17 ||  (dayWeek == 6 && entity.data.Hour >= 11)) {
                appReturn.AddException("O atendimento das solicitações de hoje já foram encerradas (favor enviar suas solicitações no próximo dia útil)");
                return appReturn;
            }

            appReturn = BLO.Validar(entity);

            if(!appReturn.status.success)
                return appReturn;

            entity = BLO.Normalizar(entity);

            entity.agendado = entity.dataVisita.Year > 1900;

            try {
                LocalidadeService localidade = new LocalidadeService();
                if(entity.idEstado == 0)
                    entity.idEstado = (localidade.ObterIdEstado(entity.estado)).result.id;
                if(entity.idCidade == 0)
                    entity.idCidade = (localidade.ObterIdCidade(entity.idEstado,entity.cidade)).result.id;
                if(entity.idBairro == 0)
                    entity.idBairro = (localidade.ObterIdBairro(entity.idCidade,entity.bairro)).result.id;
            } catch(Exception ex) { }

            entity.idStatus = 3;
            entity.status = "Aguardando";

            appReturn = DAO.Adicionar(entity);

            return appReturn;
        }




        public AppReturn AlterarDisponibilidade(Admin entity) {
            if(entity is null || entity?.id == 0) {
                appReturn.AddException("usuário não identificado.");
                return appReturn;
            }
            appReturn = DAO.AlterarDisponibilidade(entity);
            return appReturn;
        }



        public AppReturn ObterTodasSolicitacoesPeloId(Solicitacao entity) {
            return DAO.ObterTodasSolicitacoesPeloId(entity);
        }

        public AppReturn ObterTodos() {
            return   DAO.ObterTodos();
        }

        public AppReturn ObterTodosAdmin(Admin entity) {
            return   DAO.ObterTodosAdmin(entity);
        }
        
        public AppReturn ObterTodosParceiro(Solicitacao entity) {
            return   DAO.ObterTodosParceiro(entity);
        }



        public AppReturn ObterPeloId(int id) {

            if(id == 0) {
                appReturn.SetAsBadRequest("ID não informado.");
                return appReturn;
            }

            Solicitacao entity = DAO.ObterPeloId(id);

            if(entity is null || entity?.id == 0)
                appReturn.SetAsNotFound();
            else {
                appReturn.result = entity;
            }

            return appReturn;

        }









        public AppReturn AlterarStatus(Solicitacao entity){
            if(entity.idStatus == 3)
                entity.status = "Aguardando";
            else if(entity.idStatus == 5)
                entity.status = "Verificando";
            else if(entity.idStatus == 9)
                entity.status = "Finalizado";
            else {
                appReturn.AddException("Status não reconhecido");
                return appReturn;
            }
            appReturn = DAO.Alterar(entity);
            return appReturn;
        }

        



        
        public AppReturn Alterar(Solicitacao entity){

            appReturn = BLO.Validar(entity);

            if (!appReturn.status.success)
                return appReturn;

            entity = BLO.Normalizar(entity);

            try {
                LocalidadeService localidade = new LocalidadeService();
                if(entity.idEstado == 0)
                    entity.idEstado = (localidade.ObterIdEstado(entity.estado)).result.id;
                if(entity.idCidade == 0)
                    entity.idCidade = (localidade.ObterIdCidade(entity.idEstado,entity.cidade)).result.id;
                if(entity.idBairro == 0)
                    entity.idBairro = (localidade.ObterIdBairro(entity.idCidade,entity.bairro)).result.id;
            }catch(Exception ex){ }

            appReturn = DAO.Alterar(entity);

            return appReturn;
        }





        public AppReturn Captar(Solicitacao entity) {
            entity.idStatus     = 5;
            entity.status       = "Verificando";
            return DAO.Alterar(entity);
        }
        public AppReturn RealocarNaFila(Solicitacao entity) {
            return DAO.RealocarNaFila(entity);
        }
        public AppReturn RealocarParaAdmin(Solicitacao entity) {
            return DAO.RealocarParaAdmin(entity);
        }
        public AppReturn Cancelar(Solicitacao entity) {
            entity.idStatus     = 3;
            entity.status       = "Aguardando";
            entity.admin        = new Admin();
            entity.idAdmin      = 0;
            return DAO.Alterar(entity);
        }



        public AppReturn Finalizar(Solicitacao entity) {
            //entity.idStatus     = 9;
            //entity.status       = "Finalizada";
            appReturn = DAO.Alterar(entity);

            if(appReturn.status.success) {

                Mail mail    = new Mail();
                mail.emailTo = entity.parceiro.email;
                mail.about   = "Retorno de sua solicitação ID#"+entity.id.ToString();
                
                mail.message = "<b>Solicitação ID#" + entity.id.ToString() + " finalizada.</b>";

                if(entity.idStatus == 11){
                    mail.message += "<br><br>";
                    mail.message += "<b style='color:#ff3333'>Infelizmente não foi possível encontrar o imóvel solicitado.</b>";
                }

                mail.message += "<br><br><b>Proprietário:</b>";
                if(Utils.Validator.Is(entity.proprietarioCaptacao)) {
                    mail.message    += "<br>"+(Utils.Validator.Is(entity.proprietarioCaptacao)? entity.proprietarioCaptacao.Replace(",","<br>") : "não encontrado");
                    //if(!entity.validadoProprietario)
                    //    mail.message += "<br><b style='color:#ff3333'>Proprietário inválido</b>";
                }else
                    mail.message    += "<br>não encontrado";

                string endereco = "";
                mail.message += "<br><br><b>Localização:</b>";
                endereco += Utils.Validator.Is(entity.logradouro )?(entity.logradouro +", "  ) : "";
                endereco += Utils.Validator.Is(entity.numero     )?(entity.numero     +", "  ) : "";
                endereco += Utils.Validator.Is(entity.complemento)?(entity.complemento+", "  ) : "";
                endereco += Utils.Validator.Is(entity.cidade     )?(entity.cidade     +", "  ) : "";
                endereco += Utils.Validator.Is(entity.estado     )?(entity.estado            ) : "";
                endereco += Utils.Validator.Is(entity.cep        )?("<br>CEP: " + entity.cep ) : "";

                if(Utils.Validator.Is(endereco)){
                    mail.message += "<br>" + endereco;
                    //if(!entity.validadoEndereco)
                    //    mail.message += "<br><b style='color:#ff3333'>Endereço inválido</b>";
                }else
                    mail.message    += "<br>não encontrada";
                    


                mail.message    += "<br><br><b>URL:</b>";
                if(Utils.Validator.Is(entity.url)){
                    mail.message    += "<br><a href='"+entity.url+"'>"+entity.url+"</a>";
                    //if(!entity.validadaURL)
                    //    mail.message += "<br><b style='color:#ff3333'>URL inválida</b>";
                } else
                    mail.message    += "<br>não informada";


                mail.message += "<br><br><b>Obs:</b>";
                mail.message += "<br>" + (Utils.Validator.Is(entity.descricao) ? entity.descricao : "sem observações");


                mail.message += "<br><br><b>Avaliação:</b>";
                mail.message += "<br>" + (Utils.Validator.Is(entity.avaliacao        )? entity.avaliacao : "não avaliado");

                mail.Send();

            }

            return appReturn;


        }



        public AppReturn Buscar(Busca busca) {
            appReturn = DAO.Buscar(busca);
            return appReturn;
        }

        
        public AppReturn ObterDistribuicoes() {
            appReturn = DAO.ObterDistribuicoes();
            return appReturn;
        }


        public AppReturn Excluir(int id) {
            appReturn = DAO.Excluir(id);
            return appReturn;
        }











    }
}
