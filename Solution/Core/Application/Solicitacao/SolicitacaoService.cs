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
using System.ComponentModel;
using System.Text;
using JaCaptei.Model.DTO;

namespace JaCaptei.Application {

    public class SolicitacaoService:ServiceBase {


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

            if(!entity.visita) {
                appReturn.AddException("Agendamento da visita não definido.");
                return appReturn;
            }

            entity.idStatus = 3;
            entity.status = "Aguardando";

            appReturn = DAO.Adicionar(entity);

            if(appReturn.status.success && entity.visita) {

                Mail mail    = new Mail();

                if(entity.imovelJC) {           //notificar confirmacao visita
                    mail.about   = "Agendamento de Visita ID #" + entity.id.ToString() + " - Imóvel CÓD "+entity.codImovel;
                    mail.message = "<b>Prezado(a) " + entity.parceiro.apelido + ".</b><br><br>";
                    mail.message += "Recebemos sua solicitação de visita para o imóvel com código "+entity.codImovel+", disponível em nossa plataforma JáCaptei.<br><br>";
                    mail.message += "Já estamos entrando em contato com o proprietário, e assim que tivermos uma atualização sobre a disponibilidade para a visita, informaremos você imediatamente.<br><br>";
                    mail.message += "Agradecemos pela confiança e estamos à disposição para qualquer dúvida.<br><br>";
                    mail.message += "Atenciosamente,<br>";
                    mail.message += "Equipe JáCaptei";
                } else {
                    mail.about   = "Agendamento de Visita ID #" + entity.id.ToString() + " - Imóvel a identificar ";
                    mail.message = "<b>Prezado(a) " + entity.parceiro.apelido + ".</b><br><br>";
                    mail.message += "Recebemos sua solicitação de visita a um imóvel que não está em nossa base no momento.<br>";
                    mail.message += "Já iniciamos o procedimento de identificação e localização do Imóvel e proprietário para atender à sua demanda e, assim que tivermos uma atualização sobre a disponibilidade para a visita, informaremos você imediatamente.<br><br>";
                    mail.message += "Agradecemos pela confiança e estamos à disposição para qualquer dúvida.<br><br>";
                    mail.message += "Atenciosamente,<br>";
                    mail.message += "Equipe JáCaptei";
                }
                mail.emailTo = entity.parceiro.email;
                mail.Send();
            }


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
            return DAO.ObterTodos();
        }

        public AppReturn ObterTodosAdmin(Admin entity) {
            return DAO.ObterTodosAdmin(entity);
        }
        public AppReturn ObterSolicitacoesAdmin(Admin entity) {
            return DAO.ObterSolicitacoesAdmin(entity);
        }
        public AppReturn ObterVisitasAdmin(Admin entity) {
            return DAO.ObterVisitasAdmin(entity);
        }

        public AppReturn ObterTodosParceiro(Solicitacao entity) {
            return DAO.ObterTodosParceiro(entity);
        }

        public AppReturn ObterTodosSemVisitaParceiro(Solicitacao entity) {
            return DAO.ObterTodosSemVisitaParceiro(entity);
        }

        public AppReturn ObterTodosComVisitaParceiro(Solicitacao entity) {
            return DAO.ObterTodosComVisitaParceiro(entity);
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









        public AppReturn AlterarStatus(Solicitacao entity) {
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






        public AppReturn Alterar(Solicitacao entity) {

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

            if(entity.reagendado && !entity.visitado && !entity.concluido)
                entity.dataVisita =  entity.dataAgendamento = entity.dataReagendamento;// Utils.Date.GetLocalDateTime(entity.dataReagendamento);
            else
                entity.dataAgendamento = entity.dataReagendamento = entity.dataVisita;

            appReturn = DAO.Alterar(entity);

            entity = appReturn.result;




            // NOTIFICACOES


            if(entity.notificar) {

                bool notificar = false;

                Mail mail    = new Mail();

                if(entity.confirmado && !entity.visitado && !entity.concluido) {           //notificar confirmacao visita
                    mail.about   = "Confirmação da Visita ID#"+entity.id.ToString();
                    mail.message = "<b>Prezado(a) " + entity.parceiro.apelido + ".</b><br><br>";
                    mail.message += "Conforme nossa conversa, confirmamos a visita ao seu imóvel na data e localização informados abaixo.<br><br>";
                    mail.message += "Corretor:" + entity.parceiro.nome + ".<br>";
                    mail.message += "Telefone:" + entity.parceiro.telefone ;
                    //mail.message = "<b>Confirmação da Visita ID#" + entity.id.ToString() + ".</b>";
                    entity.obs = entity.obsConfirmado;
                    notificar = true;
                } else if(entity.reagendado && !entity.confirmado && !entity.visitado && !entity.concluido) {       //notificar reagendamento visita
                    mail.about   = "Solicitação de REAGENDAMENTO da Visita ID#"+entity.id.ToString();
                    mail.message = "<b>Prezado(a) " + entity.parceiro.apelido + ".</b><br><br>";
                    mail.message = "<b>Favor confirmar reagendamento solicitado da visita ID#" + entity.id.ToString() + ".</b>";
                    entity.obs = entity.obsReagendamento;
                    notificar = true;
                }

                if(notificar) {

                    mail.message += "<br><br><b>Data:</b><br>";
                    mail.message += entity.dataVisita.ToString("dd/MM/yyyy', 'HH:mm'h'");

                    string endereco = entity.ObterEndereco();

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

            mail.about   = "Confirmação da Visita ID#"+entity.id.ToString();
            mail.message = "<b>Prezado(a) " + entity.parceiro.apelido + ".</b><br><br>";
            mail.message += "Confirmamos a visita ao seu imóvel na data e localização informados abaixo.";
            //mail.message += "Corretor:" + entity.parceiro.nome + ".<br>";
            //mail.message += "Telefone:" + entity.parceiro.telefone + ".<br><br>";

            string endereco = entity.ObterEndereco();

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




                if(entity.idStatus == 11) {
                    mail.message += "<br><br>";
                    if(entity.visita) {
                        mail.about   = "Agendamento de Visita ID #" + entity.id.ToString() + " - CANCELADO ";
                        mail.message = "<b>Prezado(a) " + entity.parceiro.apelido + ".</b><br><br>";
                        mail.message += "Atualização da solicitação de visita ID " +  entity.id.ToString() + ".<br><br>";
                        if(entity.imovelJC)
                            mail.message += "Infelizmente, o imóvel de código "+entity.codImovel+" foi vendido recentemente.<br><br> Podemos ajudar você a encontrar outro imóvel?";
                        else
                            mail.message += "Infelizmente, não conseguimos localizar o imóvel e por isso não será possível prosseguir com a visita no momento. Mas não desanime! Você pode enviar outros imóveis para que possamos nos empenhar em localizá-los.";
                        if(entity.imovelVendido)
                            mail.message += "<br>Imóvel vendido.";
                        else if(entity.imovelNaoEncontrado)
                            mail.message += "<br>Imóvel não encontrado.";
                        //if(Utils.Validator.Is(entity.obsConcluido))
                        //    mail.message += "<br><br>Obs: " + entity.obsConcluido;
                    } else
                        mail.message += "<b style='color:#ff3333'>Infelizmente não foi possível encontrar o imóvel solicitado.</b>";
                }

                if(entity.visita) {
                    mail.message += "<br><br><b>Data:</b><br>";
                    mail.message += entity.dataVisita.ToString("dd/MM/yyyy', 'HH:mm'h'");
                } else {
                    mail.message += "<br><br><b>Proprietário:</b>";
                    if(Utils.Validator.Is(entity.proprietarioCaptacao)) {
                        mail.message    += "<br>"+ ((entity.proprietarioNaoEncontrado) ? "não encontrado" : entity.proprietarioCaptacao.Replace(",","<br>"));
                        //if(!entity.validadoProprietario)
                        //    mail.message += "<br><b style='color:#ff3333'>Proprietário inválido</b>";
                    } else
                        mail.message    += "<br>não encontrado";
                }

                string endereco = entity.ObterEndereco();

                if(Utils.Validator.Is(endereco)) {
                    mail.message += "<br>" + endereco;
                    //if(!entity.validadoEndereco)
                    //    mail.message += "<br><b style='color:#ff3333'>Endereço inválido</b>";
                } else
                    mail.message    += "<br>não encontrada";



                mail.message    += "<br><br><b>URL:</b>";
                if(Utils.Validator.Is(entity.url)) {
                    mail.message    += "<br><a href='"+entity.url+"'>"+entity.url+"</a>";
                    //if(!entity.validadaURL)
                    //    mail.message += "<br><b style='color:#ff3333'>URL inválida</b>";
                } else
                    mail.message    += "<br>não informada";


                mail.message += "<br><br><b>Obs:</b>";
                mail.message += "<br>" + (Utils.Validator.Is(entity.descricao) ? entity.descricao : "sem observações");


                mail.message += "<br><br><b>Avaliação:</b>";
                mail.message += "<br>" + (Utils.Validator.Is(entity.avaliacao) ? entity.avaliacao : "não avaliado");

                mail.message += "<br><br>Atenciosamente,<br>";
                mail.message += "Equipe JáCaptei";

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
