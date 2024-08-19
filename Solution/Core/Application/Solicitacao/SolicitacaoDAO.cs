using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using JaCaptei.Model.Model;
using RepoDb.Extensions;
using RepoDb.Enumerations;
using MailKit.Search;
using Newtonsoft.Json;
using System.Text.Json;

namespace JaCaptei.Application {

    public class SolicitacaoDAO:DAOBase {


        public AppReturn Adicionar(Solicitacao entity) {

            string select = "SELECT     a.* "
                          + "   FROM    "        
                          + "             \"Admin\" a JOIN \"AdminSettings\" stt ON (stt.\"idAdmin\" = a.id) "
                          + "   WHERE       a.ativo = 'TRUE' AND a.disponivel = 'TRUE' AND a.god = 'FALSE' AND a.gestor = 'FALSE' "
                          +               ((entity.agendado)? " AND stt.\"receberSolicitacaoAgendada\" = 'TRUE' " : " AND stt.\"receberSolicitacaoNaoAgendada\" = 'TRUE' ") ;

            string  sql     = "SELECT ROW_TO_JSON(res) FROM  ( " + select + " ORDER BY a.\"dataCod\" ASC LIMIT 1 ) res ";


            using(var conn = DB.GetConn()) {
                using(var trans = conn.EnsureOpen().BeginTransaction()) {

                    try {

                        Admin adminDB = null;

                        var res     = conn.ExecuteQuery(sql).FirstOrDefault();
                        if(res?.row_to_json is not null)
                            adminDB   = JsonConvert.DeserializeObject<Admin>(res.row_to_json);

                        //Admin adminDB = conn.Query<Admin>(a=> a.god == false && a.gestor == false && a.disponivel == true,orderBy:OrderField.Parse(new{ dataCod = Order.Ascending })  ).FirstOrDefault();

                        if(adminDB is null)
                            appReturn.AddException("Não há captadores disponíveis no momento, favor tentar novamente daqui a pouco.");

                        
                        ParceiroSettings parceiroSettings = conn.Query<ParceiroSettings>(ps=> ps.idParceiro == entity.idParceiro).FirstOrDefault();

                        if(parceiroSettings is null)
                            appReturn.AddException("Não foi possível validar solicitação");
                        else if(    (!parceiroSettings.habilitadoFazerSolicitacoes) || 
                                    (entity.agendado  && !parceiroSettings.habilitadoFazerSolicitacoesAgendadas) || 
                                    (!entity.agendado && !parceiroSettings.habilitadoFazerSolicitacoesNaoAgendadas)  
                        )
                            appReturn.AddException("Recurso indisponível em sua conta.");
                        else {
                            int daykey = Utils.Key.CreateDaykey();
                            if(parceiroSettings.daykey < daykey) {
                                parceiroSettings.daykey = daykey;
                                parceiroSettings.totalSolicitacoesAbertasAgendadas = parceiroSettings.totalSolicitacoesAbertasNaoAgendadas = 0;
                            } else {
                                short dayWeek = (short)entity.data.DayOfWeek;
                                if(dayWeek >= 6 && !entity.agendado)
                                    appReturn.AddException("O atendimento de solicitações não agendadas não estão disponíveis no dia de hoje (favor enviar suas solicitações no próximo dia útil)");
                                else if(!entity.agendado && parceiroSettings.totalSolicitacoesAbertasNaoAgendadas >= parceiroSettings.limiteSolicitacoesDiariasNaoAgendadas)
                                    appReturn.AddException("O limite diário de solicitações foi alcançado.");
                                else if(entity.agendado && parceiroSettings.totalSolicitacoesAbertasAgendadas >= parceiroSettings.limiteSolicitacoesDiariasAgendadas)
                                    appReturn.AddException("O limite diário de solicitações agendadas foi alcançado.");
                            }
                        }


                        if(appReturn.status.success) {

                            adminDB.dataCod = Utils.Key.CreateDayTimeCode(); // 'dataCod' eh o codigo usado para ordenar distribuicao das solicitacoes, o menos atualizado eh pego primeiro
                            conn.Update<Admin>(adminDB);


                            if(entity.agendado)
                                parceiroSettings.totalSolicitacoesAbertasAgendadas      += 1;
                            else
                                parceiroSettings.totalSolicitacoesAbertasNaoAgendadas   += 1;
                            
                            conn.Update<ParceiroSettings>(parceiroSettings);


                            entity.idAdmin = adminDB.id;
                            entity.id = conn.Insert<Solicitacao,int>(entity);

                            entity.admin = new Admin { id=adminDB.id,nome=adminDB.nome,apelido=adminDB.apelido,telefone=adminDB.telefone,email=adminDB.email };

                            trans.Commit();

                        } else
                            trans.Rollback();

                    } catch(Exception ex) {
                        appReturn.SetAsException("Falha ao inserir solicitação",ex);
                        trans.Rollback();
                    }
                }
            }

            appReturn.result = entity;
            return appReturn;

        }


        public AppReturn RealocarNaFila(Solicitacao entity) {

            string select = "SELECT     a.* "
                          + "   FROM    "
                          + "             \"Admin\" a JOIN \"AdminSettings\" stt ON (stt.\"idAdmin\" = a.id) "
                          + "   WHERE       a.ativo = 'TRUE' AND a.disponivel = 'TRUE' AND a.god = 'FALSE' AND a.gestor = 'FALSE' "
                          +               ((entity.agendado)? " AND stt.\"receberSolicitacaoAgendada\" = 'TRUE' " : " AND stt.\"receberSolicitacaoNaoAgendada\" = 'TRUE' ") 
                          +                                  "  AND a.id <> " + entity.idAdmin.ToString();

            string  sql     = "SELECT ROW_TO_JSON(res) FROM  ( " + select + " ORDER BY a.\"dataCod\" ASC LIMIT 1 ) res ";


            using(var conn = DB.GetConn()) {


                Admin adminDB = null;

                var res     = conn.ExecuteQuery(sql).FirstOrDefault();
                if(res?.row_to_json is not null)
                    adminDB   = JsonConvert.DeserializeObject<Admin>(res.row_to_json);

                //Admin adminDB = conn.Query<Admin>(a=> a.god == false && a.gestor == false && a.disponivel == true && a.id != entity.idAdmin,orderBy:OrderField.Parse(new{ dataCod = Order.Ascending })  ).FirstOrDefault();

                if(adminDB is null) {
                    appReturn.AddException("Usuário não encontrado ou indisponível");
                    return appReturn;
                }

                adminDB.dataCod = Utils.Key.CreateDayTimeCode();
                conn.Update<Admin>(adminDB);

                Solicitacao entityDB = conn.Query<Solicitacao>(e => e.id == entity.id).FirstOrDefault();

                entityDB.idStatus   = entity.idStatus     = 3;
                entityDB.status     = entity.status       = "Aguardando";
                entityDB.idAdmin    = entity.idAdmin      = adminDB.id;

                conn.Update<Solicitacao>(entityDB);

                entity.admin = new Admin { id=adminDB.id,nome=adminDB.nome,apelido=adminDB.apelido,telefone=adminDB.telefone,email=adminDB.email };

            }
            appReturn.result = entity;
            return appReturn;
        }
    

        public AppReturn RealocarParaAdmin(Solicitacao entity) {
            using(var conn = DB.GetConn()) {

                Admin adminDB = conn.Query<Admin>(a=> a.id == entity.idAdmin).FirstOrDefault();
                adminDB.dataCod = Utils.Key.CreateDayTimeCode();
                conn.Update<Admin>(adminDB);

                Solicitacao entityDB = conn.Query<Solicitacao>(e => e.id == entity.id).FirstOrDefault();

                entityDB.idStatus   = entity.idStatus     = 3;
                entityDB.status     = entity.status       = "Aguardando";
                entityDB.idAdmin    = entity.idAdmin      = adminDB.id;

                conn.Update<Solicitacao>(entityDB);

                entity.admin = new Admin { id=adminDB.id,nome=adminDB.nome,apelido=adminDB.apelido,telefone=adminDB.telefone,email=adminDB.email };

            }
            appReturn.result = entity;
            return appReturn;
        }



        public AppReturn AlterarDisponibilidade(Admin entity) {
                using(var conn = DB.GetConn()) {
                        try {
                                AdminSettings entityDB = null;
                                entityDB = conn.Query<AdminSettings>(e => e.idAdmin == entity.id).FirstOrDefault();
                                if(entityDB is not null && entityDB?.id > 0) {
                                    entityDB.receberSolicitacaoAgendada    = entity.settings.receberSolicitacaoAgendada;
                                    entityDB.receberSolicitacaoNaoAgendada = entity.settings.receberSolicitacaoNaoAgendada;
                                    conn.Update<AdminSettings>(entityDB);
                                }
                                else
                                    appReturn.AddException("Não foi possível alterar disponibilidade (usuário não encontrado ou inválido).");
                        } catch(Exception ex) {
                            appReturn.AddException("Não foi possível alterar disponibilidade.");
                            appReturn.status.exception = ex.ToString();
                        }
                 }
            return appReturn;
        }
        



        public AppReturn Alterar(Solicitacao entity) {

            Solicitacao entityDB = null;
            using(var conn = DB.GetConn()) {

                entityDB = conn.Query<Solicitacao>(e => e.id == entity.id).FirstOrDefault();
                if(entityDB is null) {
                    appReturn.AddException("Solicitação não encontrada");
                    return appReturn;
                }

                entityDB.dataAtualizacao     = entity.dataAtualizacao = Utils.Date.GetLocalDateTime();

                if(entity.admin is not null && entity.admin?.id>0) {
                    entityDB.idAdmin             = entity.idAdmin = entity.admin.id;
                    entityDB.atualizadoPorId     = entity.admin.id;
                    entityDB.atualizadoPorNome   = entity.admin.nome;
                    entityDB.atualizadoPorPerfil = "ADMIN";
                } else if(entity.parceiro is not null && entity.parceiro?.id>0) {
                    entityDB.idParceiro          = entity.parceiro.id;
                    entityDB.atualizadoPorId     = entity.parceiro.id;
                    entityDB.atualizadoPorNome   = entity.parceiro.nome;
                    entityDB.atualizadoPorPerfil = "PARCEIRO";
                }

                entityDB.idAdmin             = entity.idAdmin;
                entityDB.idParceiro          = entity.idParceiro;
                entityDB.idStatus            = entity.idStatus;
                entityDB.status              = entity.status;
                entityDB.url                 = entity.url;
                entityDB.descricao           = entity.descricao;
                entityDB.avaliacao           = entity.avaliacao;
                entityDB.liberado            = entity.liberado;

                entityDB.proprietarioCaptacao= entity.proprietarioCaptacao;
                entityDB.dataVisita          = entity.dataVisita;

                entityDB.cep                 = entity.cep;
                entityDB.cepNorm             = entity.cepNorm;
                entityDB.logradouro          = entity.logradouro;
                entityDB.logradouroNorm      = entity.logradouroNorm;
                entityDB.numero              = entity.numero;
                entityDB.complemento         = entity.complemento;
                entityDB.referencia          = entity.referencia;
                entityDB.bairro              = entity.bairro;
                entityDB.bairroNorm          = entity.bairroNorm;
                entityDB.cidade              = entity.cidade;
                entityDB.cidadeNorm          = entity.cidadeNorm;
                entityDB.estado              = entity.estado;
                entityDB.estadoNorm          = entity.estadoNorm;
                entityDB.pais                = entity.pais;
                entityDB.paisNorm            = entity.paisNorm;
                entityDB.idEstado            = entity.idEstado;
                entityDB.idCidade            = entity.idCidade;
                entityDB.idBairro            = entity.idBairro;

                entityDB.validadaURL         = entity.validadaURL;
                entityDB.validadoEndereco    = entity.validadoEndereco;
                entityDB.validadoProprietario= entity.validadoProprietario;


                conn.Update<Solicitacao>(entityDB);

                /*
                if(entity.idStatus > 9){
                    ParceiroSettings parceiroSettings = conn.Query<ParceiroSettings>(ps=> ps.idParceiro == entity.idParceiro).FirstOrDefault();
                    if(entity.agendado && parceiroSettings.totalSolicitacoesAbertasAgendadas > 0)
                        parceiroSettings.totalSolicitacoesAbertasAgendadas      -= 1;
                    else if(parceiroSettings.totalSolicitacoesAbertasNaoAgendadas > 0)
                        parceiroSettings.totalSolicitacoesAbertasNaoAgendadas   -= 1;
                }
                */


                entity.admin = entity.idAdmin == 0 ? new Admin() : conn.Query<Admin>(a => a.id == entity.idAdmin,fields: Field.Parse<Pessoa>(e => new { e.id,e.nome,e.apelido,e.telefone,e.email })).FirstOrDefault();
            }
            appReturn.result = entity;
            return appReturn;
        }



        public AppReturn Excluir(int id) {

            int affectedRows = 1;

            using(var conn = DB.GetConn())
                affectedRows = conn.Delete<Solicitacao>(id);

            if(affectedRows == 0)
                appReturn.AddException("Não foi possível excluir proprietário");


            return appReturn;
        }



        public Solicitacao ObterPeloId(int id) {

            Solicitacao entityDB = null;

            using(var conn = DB.GetConn())
                entityDB = conn.Query<Solicitacao>(e => e.id == id).FirstOrDefault();

            return entityDB;
        }



        public AppReturn ObterTodasSolicitacoesPeloId(Solicitacao entity) {
            List<Solicitacao> entities = new List<Solicitacao>();
            using(var conn = DB.GetConn()) {

                var order   = OrderField.Parse(new{ id = Order.Descending });
                var fields  = Field.Parse<Pessoa>(e => new{e.id,e.nome,e.apelido,e.telefone,e.email,e.cpf,e.cnpj});

                if(entity.idParceiro > 0)
                    entities = conn.Query<Solicitacao>(e => e.idParceiro == entity.idParceiro   && e.ativo == true,orderBy: order).ToList();
                else if(entity.idAdmin > 0)
                    entities = conn.Query<Solicitacao>(e => e.idAdmin    == entity.idAdmin      && e.ativo == true,orderBy: order).ToList();
                entities.ForEach((e) => {
                    e.parceiro  = e.idParceiro == 0 ? new Parceiro() : conn.Query<Parceiro>(p => p.id == e.idParceiro,fields: fields).FirstOrDefault();
                    e.admin     = e.idAdmin == 0 ? new Admin() : conn.Query<Admin>(p => p.id == e.idAdmin,fields: fields).FirstOrDefault();
                });
            }
            appReturn.result = entities;
            return appReturn;
        }


        public AppReturn ObterTodos() {
            return appReturn;
        }


        public AppReturn ObterTodosAdmin(Admin entity) {

            DateTime finalizadosAPartirDe = DateTime.Now.AddDays(-15);
            List<Solicitacao> entities = new List<Solicitacao>();
            List<Solicitacao> finalizados = new List<Solicitacao>();

            string select = "SELECT     json_build_object('id',a.id,'nome',a.nome,'apelido',a.apelido,'email',a.email,'telefone',a.telefone,'god',a.god,'gestor',a.gestor,'roles',a.roles) as \"admin\" ," +
                         "              json_build_object('id',p.id,'nome',p.nome,'apelido',p.apelido,'email',p.email,'telefone',p.telefone) as \"parceiro\" , " +
                         "              s.* "   +
                         "      FROM "          +
                         "              \"Solicitacao\"  s JOIN \"Admin\"      a ON (s.\"idAdmin\"    = a.id) " +
                         "                                 JOIN \"Parceiro\"   p ON (s.\"idParceiro\" = p.id) ";


            string filter = " WHERE s.ativo = 'TRUE' AND s.\"idStatus\" < 9  ";
            //if(!entity.god && !entity.gestor)
            //    filter += " AND s.\"idAdmin\" = " + entity.id.ToString();
            if(entity.id > 0)
                filter += " AND s.\"idAdmin\" = " + entity.id.ToString();

            string sql  = "SELECT JSON_AGG(res) FROM  ( " + select + filter  + " ORDER BY s.\"dataConsiderada\" ASC ) res ";

            string filterFinalizados = " WHERE s.ativo = 'TRUE' AND s.\"idStatus\" > 9 AND s.\"dataConsiderada\" >= '" + finalizadosAPartirDe.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
            if (!entity.god && !entity.gestor)
                filterFinalizados += " AND s.\"idAdmin\" = " + entity.id.ToString();
            string sqlFinalizados = "SELECT JSON_AGG(resf) FROM  ( " + select + filterFinalizados  + " ORDER BY s.\"dataConsiderada\" DESC LIMIT 100 ) resf ";

            using(var conn = DB.GetConn()) {

                var res     = conn.ExecuteQuery(sql).FirstOrDefault();
                if(res?.json_agg is not null)
                    entities    = JsonConvert.DeserializeObject<List<Solicitacao>>(res.json_agg);

                var resF    = conn.ExecuteQuery(sqlFinalizados).FirstOrDefault();
                if(resF?.json_agg is not null)
                    finalizados = JsonConvert.DeserializeObject<List<Solicitacao>>(resF.json_agg);

                entities.AddRange(finalizados);

            }
            appReturn.result = entities;
            return appReturn;
        }


        public AppReturn ObterTodosParceiro(Solicitacao entity) {

            DateTime finalizadosAPartirDe = DateTime.Now.AddMonths(-2);
            List<Solicitacao> entities = new List<Solicitacao>();
            List<Solicitacao> finalizados = new List<Solicitacao>();

            string select = "SELECT     json_build_object('id',a.id,'nome',a.nome,'apelido',a.apelido,'email',a.email,'telefone',a.telefone,'god',a.god,'gestor',a.gestor,'roles',a.roles) as \"admin\" ," +
                         "              json_build_object('id',p.id,'nome',p.nome,'apelido',p.apelido,'email',p.email,'telefone',p.telefone) as \"parceiro\" , " +
                         "              s.* "   +
                         "      FROM "          +
                         "              \"Solicitacao\"  s JOIN \"Admin\"      a ON (s.\"idAdmin\"    = a.id) " +
                         "                                 JOIN \"Parceiro\"   p ON (s.\"idParceiro\" = p.id) ";

            string  filter  = " WHERE s.ativo = 'TRUE' AND s.\"idStatus\" < 9 AND s.\"idParceiro\" = " + entity.idParceiro.ToString();
            string  sql     = "SELECT JSON_AGG(res) FROM  ( " + select + filter  + " ORDER BY s.\"dataConsiderada\" DESC ) res ";

            string filterFinalizados = " WHERE s.ativo = 'TRUE' AND s.\"idStatus\" > 9 AND s.\"idParceiro\" = " + entity.idParceiro.ToString(); // + " AND s.\"dataConsiderada\" >= '" + finalizadosAPartirDe.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
            string sqlFinalizados    = "SELECT JSON_AGG(resf) FROM  ( " + select + filterFinalizados  + " ORDER BY s.\"dataConsiderada\" DESC LIMIT 400) resf ";

            using(var conn = DB.GetConn()) {

                var res     = conn.ExecuteQuery(sql).FirstOrDefault();
                if(res?.json_agg is not null)
                    entities    = JsonConvert.DeserializeObject<List<Solicitacao>>(res.json_agg);

                var resf    = conn.ExecuteQuery(sqlFinalizados).FirstOrDefault();
                if(resf?.json_agg is not null)
                    finalizados = JsonConvert.DeserializeObject<List<Solicitacao>>(resf.json_agg);

                entities.AddRange(finalizados);
            }
            appReturn.result = entities;
            return appReturn;
        }

            


        public AppReturn Buscar(Search busca) {

            List<Solicitacao> entities = new List<Solicitacao>();

            string select   = "SELECT     json_build_object('id',a.id,'nome',a.nome,'apelido',a.apelido,'email',a.email,'telefone',a.telefone,'god',a.god,'gestor',a.gestor,'roles',a.roles,'disponivel',a.disponivel) as \"admin\" ," +
                            "             json_build_object('id',p.id,'nome',p.nome,'apelido',p.apelido,'email',p.email,'telefone',p.telefone) as \"parceiro\" , " +
                            "              s.* "   +
                            "      FROM "          +
                            "              \"Solicitacao\"  s JOIN \"Admin\"      a ON (s.\"idAdmin\"    = a.id) " +
                            "                                 JOIN \"Parceiro\"   p ON (s.\"idParceiro\" = p.id) ";

            string filter = " WHERE s.ativo = 'TRUE' ";// && data >= " + finalizadosAPartirDe.ToString();



            if(busca.item?.id > 0)
                filter += " AND s.id = " + busca.item.id.ToString();
            else{

                if(busca.item?.idAdmin > 0)
                    filter += " AND s.\"idAdmin\" = " + busca.item.idAdmin.ToString();

                if(busca.item?.idParceiro > 0)
                    filter += " AND s.\"idParceiro\" = " + busca.item.idParceiro.ToString();

                if(busca.item?.idStatus > 0){
                       if(busca.item.idStatus == 9)
                            filter += " AND s.\"idStatus\" >= 9 ";
                       else
                            filter += " AND s.\"idStatus\" = " + busca.item.idStatus.ToString();
                }

                if(busca.dateFrom.Year <= 1900) {
                    if(Utils.Validator.IsDateTime(busca.item?.data))
                        filter += " AND date_trunc('day', s.\"dataConsiderada\") = '" + busca.item.data.ToString("yyyy-MM-dd") + "' ";
                }else
                        filter += " AND date_trunc('day', s.\"dataConsiderada\") >= '" + busca.dateFrom.ToString("yyyy-MM-dd") + "' AND date_trunc('day',s.\"dataConsiderada\") <= '" + busca.dateTo.ToString("yyyy-MM-dd") + "' ";


            }



            string sqlCount = "SELECT COUNT(*) FROM \"Solicitacao\" s " + filter;
            string sql      = "SELECT JSON_AGG(res) FROM  ( " + select + filter  + " ORDER BY s.\"dataConsiderada\" DESC ) res ";


            using(var conn = DB.GetConn()) {
                try {

                    busca.total  = conn.ExecuteScalar<Int64>(sqlCount);

                    var res     = conn.ExecuteQuery(sql).FirstOrDefault();
                    if(res?.json_agg is not null)
                        entities    = JsonConvert.DeserializeObject<List<Solicitacao>>(res.json_agg);

                    busca.result     = entities;
                    appReturn.result = busca;

                } catch(Exception ex) {
                    appReturn.AddException("Não foi possível buscar solicitações");
                }
            }
            return appReturn;
        }







        public AppReturn ObterDistribuicoes() {

            string sqlTotal         = "select s.\"idAdmin\", a.nome ,count(s.*) as \"total\"  from \"Solicitacao\" s JOIN \"Admin\" a on(s.\"idAdmin\" = a.id) group by(s.\"idAdmin\",a.nome)";
            string sqlAguardando    = "select s.\"idAdmin\", a.nome ,count(s.*) as \"total\"  from \"Solicitacao\" s JOIN \"Admin\" a on(s.\"idAdmin\" = a.id) WHERE s.\"idStatus\" = 3 group by(s.\"idAdmin\",a.nome)";
            string sqlVerificando   = "select s.\"idAdmin\", a.nome ,count(s.*) as \"total\"  from \"Solicitacao\" s JOIN \"Admin\" a on(s.\"idAdmin\" = a.id) WHERE s.\"idStatus\" = 5 group by(s.\"idAdmin\",a.nome)";
            //string sqlFinalizadas   = "select s.\"idAdmin\", a.nome ,count(s.*) as \"total\"  from \"Solicitacao\" s JOIN \"Admin\" a on(s.\"idAdmin\" = a.id) WHERE s.\"idStatus\" = 9 group by(s.\"idAdmin\",a.nome)";


            List<dynamic> results      = new List<dynamic>();

            using(var conn = DB.GetConn()) {
                try {

                    results.Add(conn.ExecuteQuery<dynamic>(sqlTotal).ToList());
                    results.Add(conn.ExecuteQuery<dynamic>(sqlAguardando).ToList());
                    results.Add(conn.ExecuteQuery<dynamic>(sqlVerificando).ToList());

                    //reportsTotal = conn.ExecuteQuery<dynamic>(sqlTotal).ToList();
                    //results.Add(reportsTotal);
                    //reportsAguardando = conn.ExecuteQuery<dynamic>(sqlAguardando ).ToList();
                    //results.Add(reportsAguardando);
                    //reportsVerificando = conn.ExecuteQuery<dynamic>(sqlVerificando).ToList();
                    //results.Add(reportsVerificando);
                    ////report = conn.Query<dynamic>(sqlFinalizadas).FirstOrDefault();
                    ////results.Add(report);

                    appReturn.result = results;

                } catch(Exception ex) {
                    appReturn.AddException("Não foi possível obter distribuições");
                }
            }
            return appReturn;
        }










    }
}
