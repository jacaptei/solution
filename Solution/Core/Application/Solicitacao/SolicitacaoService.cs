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
            if(entity.idImovel == 0) { // nao veio do agendamento da pagina do imovel
                if(dayWeek == 7 || entity.data.Hour >= 17 ||  (dayWeek == 6 && entity.data.Hour >= 11)) {
                    appReturn.AddException("O atendimento das solicitações de hoje já foram encerradas (favor enviar suas solicitações no próximo dia útil)");
                    return appReturn;
                }
            }

            appReturn = BLO.Validar(entity);

            if(!appReturn.status.success)
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
            } catch(Exception ex) { }

            //entity.dataVisita = Utils.Date.GetLocalDateTime(entity.dataVisita);
            entity.dataAgendamento = entity.dataReagendamento = entity.dataVisita;
            entity.visita = entity.agendado = entity.dataAgendamento.Year > 2000;
            entity.captacao = !entity.visita;

            entity.idStatus = 3;
            entity.status = "Aguardando";

            appReturn = DAO.Adicionar(entity);

            return appReturn;
        }



        public AppReturn Agendar(Solicitacao entity) {

            entity.data                 =
            entity.dataAtualizacao      =
            entity.dataConsiderada      =   Utils.Date.GetLocalDateTime();

            short dayWeek = (short)entity.data.DayOfWeek;
            if(entity.idImovel == 0) { // nao veio do agendamento da pagina do imovel
                if(dayWeek == 7 || entity.data.Hour >= 17 ||  (dayWeek == 6 && entity.data.Hour >= 11)) {
                    appReturn.AddException("O atendimento das solicitações de hoje já foram encerradas (favor enviar suas solicitações no próximo dia útil)");
                    return appReturn;
                }
            }

            appReturn = BLO.Validar(entity);

            if(!appReturn.status.success)
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
            } catch(Exception ex) { }

            entity.imovelJC = entity.imovel.id > 0;
            //entity.dataVisita = Utils.Date.GetLocalDateTime(entity.dataVisita);
            entity.dataAgendamento = entity.dataReagendamento = entity.dataVisita;
            entity.visita = entity.agendado = entity.dataVisita.Year > 2000;

            if(!entity.visita){
                appReturn.AddException("Agendamento da visita não definido.");
                return appReturn;
            }

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
        public AppReturn ObterSolicitacoesAdmin(Admin entity) {
            return   DAO.ObterSolicitacoesAdmin(entity);
        }
        public AppReturn ObterVisitasAdmin(Admin entity) {
            return   DAO.ObterVisitasAdmin(entity);
        }
        
        public AppReturn ObterTodosParceiro(Solicitacao entity) {
            return   DAO.ObterTodosParceiro(entity);
        }

        public AppReturn ObterTodosSemVisitaParceiro(Solicitacao entity) {
            return   DAO.ObterTodosSemVisitaParceiro(entity);
        }

        public AppReturn ObterTodosComVisitaParceiro(Solicitacao entity) {
            return   DAO.ObterTodosComVisitaParceiro(entity);
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

            if(entity.reagendado && !entity.visitado && !entity.concluido)
                entity.dataVisita =  entity.dataAgendamento = entity.dataReagendamento;// Utils.Date.GetLocalDateTime(entity.dataReagendamento);
            else
                entity.dataAgendamento = entity.dataReagendamento = entity.dataVisita;

            appReturn = DAO.Alterar(entity);

            entity = appReturn.result;




            // NOTIFICACOES


            if(entity.notificar){

                bool notificar = false;

                Mail mail    = new Mail();

                if(entity.confirmado && !entity.visitado && !entity.concluido) {           //notificar confirmacao visita
                    mail.about   = "Confirmação da Visita solicitada ID#"+entity.id.ToString();
                    mail.message = "<b>Confirmação da Visita ID#" + entity.id.ToString() + ".</b>";
                    entity.obs = entity.obsConfirmado;
                    notificar = true;
                } else if(entity.reagendado && !entity.confirmado && !entity.visitado && !entity.concluido) {       //notificar reagendamento visita
                    mail.about   = "Solicitação de REAGENDAMENTO da Visita ID#"+entity.id.ToString();
                    mail.message = "<b>Favor confirmar reagendamento solicitado da visita ID#" + entity.id.ToString() + ".</b>";
                    entity.obs = entity.obsReagendamento;
                    notificar = true;
                }

                if(notificar){

                        mail.message += "<br><br><b>Data:</b><br>";
                        mail.message += entity.dataVisita.ToString("dd/MM/yyyy', 'HH:mm'h'");

                        string endereco = "";
                        endereco += Utils.Validator.Is(entity.logradouro) ? (entity.logradouro +", ") : "";
                        endereco += Utils.Validator.Is(entity.numero) ? (entity.numero     +", ") : "";
                        endereco += Utils.Validator.Is(entity.complemento) ? (entity.complemento+", ") : "";
                        endereco += Utils.Validator.Is(entity.bairro) ? (entity.bairro+ ", ") : "";
                        endereco += Utils.Validator.Is(entity.cidade) ? (entity.cidade     +", ") : "";
                        endereco += Utils.Validator.Is(entity.estado) ? (entity.estado) : "";
                        endereco += Utils.Validator.Is(entity.cep) ? ("<br>CEP: " + entity.cep) : "";

                        mail.message += "<br><br><b>Localização:</b>";
                        if(Utils.Validator.Is(endereco)) {
                            mail.message += "<br>" + endereco;
                            //if(!entity.validadoEndereco)
                            //    mail.message += "<br><b style='color:#ff3333'>Endereço inválido</b>";
                        } else
                            mail.message    += "<br>não encontrada";

                        mail.message    += "<br><br><b>URL:</b>";
                        if(Utils.Validator.Is(entity.url)) {
                            mail.message    += "<br><a href='"+entity.url+"'>"+entity.url+"</a>";
                        } else
                            mail.message    += "<br>não informada";

                        mail.message += "<br><br><b>Obs:</b>";
                        mail.message += "<br>" + (Utils.Validator.Is(entity.obs) ? entity.obs: "sem observações");

                    
                        //entity.parceiro.email = "prvgnt@gmail.com";
                        mail.emailTo = entity.parceiro.email;

                        mail.Send();

                }


            }
            return appReturn;
        }



        public AppReturn ConfirmarAgendaVisita(Solicitacao entity) {

            appReturn = DAO.ConfirmarAgendaVisita(entity);

            Mail mail    = new Mail();

            mail.about   = "Confirmação da Visita solicitada ID#"+entity.id.ToString();
            mail.message = "<b>Confirmação de Visita ID#" + entity.id.ToString() + ".</b>";
            mail.message += "<br><br><b>Data:</b><br>";
            mail.message += entity.dataVisita.ToString("dd/MM/yyyy', 'HH:mm'h'");

            string endereco = "";
            endereco += Utils.Validator.Is(entity.logradouro) ? (entity.logradouro +", ") : "";
            endereco += Utils.Validator.Is(entity.numero) ? (entity.numero     +", ") : "";
            endereco += Utils.Validator.Is(entity.complemento) ? (entity.complemento+", ") : "";
            endereco += Utils.Validator.Is(entity.bairro) ? (entity.bairro+ ", ") : "";
            endereco += Utils.Validator.Is(entity.cidade) ? (entity.cidade     +", ") : "";
            endereco += Utils.Validator.Is(entity.estado) ? (entity.estado) : "";
            endereco += Utils.Validator.Is(entity.cep) ? ("<br>CEP: " + entity.cep) : "";

            mail.message += "<br><br><b>Localização:</b>";
            if(Utils.Validator.Is(endereco)) {
                mail.message += "<br>" + endereco;
                //if(!entity.validadoEndereco)
                //    mail.message += "<br><b style='color:#ff3333'>Endereço inválido</b>";
            } else
                mail.message    += "<br>não encontrada";

            mail.message    += "<br><br><b>URL:</b>";
            if(Utils.Validator.Is(entity.url)) {
                mail.message    += "<br><a href='"+entity.url+"'>"+entity.url+"</a>";
            } else
                mail.message    += "<br>não informada";

            mail.message += "<br><br><b>Obs:</b>";
            mail.message += "<br>" + (Utils.Validator.Is(entity.obs) ? entity.obs : "sem observações");

            //entity.parceiro.email = "prvgnt@gmail.com";
            mail.emailTo = entity.parceiro.email;

            mail.Send();

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

            entity.proprietarioNaoEncontrado = !Utils.Validator.Is(entity.proprietarioCaptacao);
            appReturn = DAO.Alterar(entity);

            if(appReturn.status.success) {

                Mail mail    = new Mail();
                mail.emailTo = entity.parceiro.email;
                mail.about   = "Retorno de sua solicitação ID#"+entity.id.ToString();
                
                mail.message = "<b>Solicitação ID#" + entity.id.ToString() + " finalizada.</b>";

                if(entity.idStatus == 11){
                    mail.message += "<br><br>";
                    if(entity.visita){
                        mail.message +="<b style='color:#ff3333'>Visita não concretizada.</b>";
                        if(entity.imovelVendido)
                            mail.message += "<br>Imóvel vendido.";
                        else if(entity.imovelNaoEncontrado)
                            mail.message += "<br>Imóvel não encontrado.";
                        //if(Utils.Validator.Is(entity.obsConcluido))
                        //    mail.message += "<br><br>Obs: " + entity.obsConcluido;
                    }
                    else
                        mail.message += "<b style='color:#ff3333'>Infelizmente não foi possível encontrar o imóvel solicitado.</b>";
                }

                if(entity.visita) {
                    mail.message += "<br><br><b>Data:</b><br>";
                    mail.message += entity.dataVisita.ToString("dd/MM/yyyy', 'HH:mm'h'");
                } else {
                    mail.message += "<br><br><b>Proprietário:</b>";
                    if(Utils.Validator.Is(entity.proprietarioCaptacao)) {
                        mail.message    += "<br>"+ ((entity.proprietarioNaoEncontrado) ? "não encontrado" : entity.proprietarioCaptacao.Replace(",","<br>") );
                        //if(!entity.validadoProprietario)
                        //    mail.message += "<br><b style='color:#ff3333'>Proprietário inválido</b>";
                    } else
                        mail.message    += "<br>não encontrado";
                }
                string endereco = "";
                mail.message += "<br><br><b>Localização:</b>";
                endereco += Utils.Validator.Is(entity.logradouro )?(entity.logradouro +", "  ) : "";
                endereco += Utils.Validator.Is(entity.numero     )?(entity.numero     +", "  ) : "";
                endereco += Utils.Validator.Is(entity.complemento)?(entity.complemento+", "  ) : "";
                endereco += Utils.Validator.Is(entity.bairro)?(entity.bairro+ ", "  ) : "";
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
