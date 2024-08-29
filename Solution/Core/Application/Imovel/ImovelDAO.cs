using JaCaptei.Services;
using System.Numerics;
using JaCaptei.Application.DAL;
using JaCaptei.Application;
using JaCaptei.Model.Model;
using JaCaptei.Model;
using System.Text.Json;
using RepoDb;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using Newtonsoft.Json;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace JaCaptei.Application {


    public class ImovelDAO : DAOBase{

        public AppReturn Adicionar(Imovel entity) {
            //appReturn.result = entity;
            //return appReturn;
            using(var conn = new DBcontext().GetConn()) {
                using(var trans = conn.EnsureOpen().BeginTransaction()) {
                    try {

                        entity.tipo = conn.Query<ImovelTipo>(t => t.id == entity.idTipo || t.label == entity.tipo.label).FirstOrDefault();
                        if(entity.tipo is null)
                            entity.tipo = new ImovelTipo { id=1,nome="IMOVEL",label="Imóvel" };

                        entity.idTipo   = entity.tipo.id;

                        if(entity.endereco.idTipoComplemento > 2) {
                            entity.endereco.tipoComplemento = conn.Query<ImovelTipoComplemento>(t => t.id == entity.endereco.idTipoComplemento).FirstOrDefault();
                            if(entity.endereco.tipoComplemento is null) {
                                entity.endereco.idTipoComplemento   = 1;
                                entity.endereco.complementoTipo     = "";
                            } else
                                entity.endereco.complementoTipo     = entity.endereco.tipoComplemento.label;
                        }

                        entity.token    = Utils.Key.CreateToken();
                        entity.tokenNum = Utils.Key.CreateTokenNum();
                        entity.cod      = entity.tokenNum.ToString();
                        entity.validado = false;
                        entity.ativo    = true;

                        entity.id   = conn.Insert<Imovel,int>(entity);
                        
                        entity.cod              = (Utils.Validator.Is(entity.codCRM) ? entity.codCRM : ("JC"+entity.id.ToString("0000")));
                        entity.nome             = entity.tipo.label + ", ID " + entity.id.ToString() + ", COD " + entity.cod;
                        //entity.tag              = "imovel_id_" + entity.id.ToString() + "_cod_"+entity.cod ;
                        entity.tag              = (Config.settings.environment != "PRODUCTION")? "homolog" : "imovel_id_" + entity.id.ToString() + "_cod_"+entity.cod;
                        entity.tokenNum         = Utils.Key.CreateTokenNum(entity.id);
                        entity.dataAtualizacao  = Utils.Date.GetLocalDateTime();

                        if(Utils.Validator.Not(entity.carga))
                            entity.carga = Utils.Key.CreateDaykey().ToString();

                        entity.titulo       =    entity.ObterTitulo();
                        entity.urlPublica   =    entity.ObterUrlPublica();
                        
                        conn.Update<Imovel>(entity);

                        //short index =  0;
                        //short ordem = -5;
                        //entity.imagens.ForEach(i => {
                        //    i.idImovel  = entity.id;
                        //    i.principal = (index == 0);
                        //    i.index     = index++;
                        //    i.ordem     = ordem+=5;
                        //    i.tokenNum  = entity.tokenNum;
                        //    i.data      = entity.data;
                        //    i.tag       = entity.tag;
                        //    conn.Insert<ImovelImagem>(i);
                        //});

                        entity.endereco.idImovel = entity.id;
                        conn.Insert<ImovelEndereco>(entity.endereco);

                        entity.valor.idImovel = entity.id;
                        conn.Insert<ImovelValores>(entity.valor);

                        entity.area.idImovel = entity.id;
                        conn.Insert<ImovelAreas>(entity.area);

                        entity.lazer.idImovel = entity.id;
                        conn.Insert<ImovelLazer>(entity.lazer);

                        entity.interno.idImovel = entity.id;
                        conn.Insert<ImovelCaracteristicasInternas>(entity.interno);

                        entity.externo.idImovel = entity.id;
                        conn.Insert<ImovelCaracteristicasExternas>(entity.externo);

                        entity.documentacao.idImovel = entity.id;
                        conn.Insert<ImovelDocumentacao>(entity.documentacao);

                        entity.disposicao.idImovel = entity.id;
                        conn.Insert<ImovelDisposicao>(entity.disposicao);

                        trans.Commit();

                    } catch(Exception ex) {
                        appReturn.SetAsException("Falha ao inserir imóvel",ex);
                        trans.Rollback();
                    }
                }
            }
            appReturn.result = entity;
            return appReturn;
        }



        public AppReturn Alterar(Imovel entity) {
            //appReturn.result = entity;
            //return appReturn;

            using(var conn = new DBcontext().GetConn()) {
                using(var trans = conn.EnsureOpen().BeginTransaction()) {
                    try {

                        entity.tipo = conn.Query<ImovelTipo>(t => t.id == entity.idTipo || t.label == entity.tipo.label).FirstOrDefault();
                        if(entity.tipo is null)
                            entity.tipo = new ImovelTipo { id=1,nome="IMOVEL",label="Imóvel" };

                        if(entity.endereco.idTipoComplemento > 2){ 
                            entity.endereco.tipoComplemento = conn.Query<ImovelTipoComplemento>(t => t.id == entity.endereco.idTipoComplemento).FirstOrDefault();
                            if(entity.endereco.tipoComplemento is null) {
                                entity.endereco.idTipoComplemento   = 1;
                                entity.endereco.complementoTipo     = "";
                            } else
                                entity.endereco.complementoTipo     = entity.endereco.tipoComplemento.label;
                        }

                        entity.idTipo           = entity.tipo.id;
                        entity.titulo           = entity.ObterTitulo();
                        entity.urlPublica       = entity.ObterUrlPublica();
                        entity.dataAtualizacao  = Utils.Date.GetLocalDateTime();

                        conn.Update<Imovel>(entity);

                        conn.Update<ImovelEndereco>(entity.endereco);
                        conn.Update<ImovelValores>(entity.valor);
                        conn.Update<ImovelAreas>(entity.area);
                        conn.Update<ImovelLazer>(entity.lazer);
                        conn.Update<ImovelCaracteristicasInternas>(entity.interno);
                        conn.Update<ImovelCaracteristicasExternas>(entity.externo);
                        conn.Update<ImovelDocumentacao>(entity.documentacao);
                        conn.Update<ImovelDisposicao>(entity.disposicao);

                        trans.Commit();

                    } catch(Exception ex) {
                        appReturn.SetAsException("Falha ao inserir imóvel",ex);
                        trans.Rollback();
                    }
                }
            }
            appReturn.result = entity;
            return appReturn;
        }

        
        public AppReturn Excluir(int _id) {
            return Excluir(new Imovel{ id = _id});
        }

        public AppReturn Excluir(Imovel entity) {
            using(var conn = new DBcontext().GetConn()) {
                conn.Delete<Imovel>(entity);
            }
            return appReturn;
        }


        public AppReturn Validar(Imovel entity) {
            Imovel entityDB = new Imovel();
            try {
                using(var conn = new DBcontext().GetConn()) {

                    entityDB = conn.Query<Imovel>(e => e.id == entity.id).FirstOrDefault();

                    if(entityDB is not null && entityDB?.id > 0) {

                        entityDB.validado           = entity.validado   = true;
                        entityDB.ativo              = entity.ativo      = true;
                        entityDB.status             = "ATIVO";
                        //entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();
                        //entityDB.atualizadoPorId    = entity.atualizadoPorId;
                        //entityDB.atualizadoPorNome  = entity.atualizadoPorNome;
                        //entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();
                        conn.Update<Imovel>(entityDB);

                        entity.proprietario = conn.Query<Proprietario>(e => e.id == entity.idProprietario).FirstOrDefault();

                    } else
                        appReturn.AddException("Não foi possível validar (registro não encontrado ou inválido).");
                }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível validar (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }



        public AppReturn ObterPendentesValidacao() {
            ImovelBusca busca = new ImovelBusca();
            busca.somenteNaoValidados = true;
            busca.somenteAtivos       = true;
            busca.page = 0;
            return Buscar(busca);
        }




        public void AlterarImagens(Imovel entity) {
            AdicionarImagens(entity);
        }

        public void AdicionarImagens(Imovel entity) {
            using(var conn = DB.GetConn()) {

                using(var trans = conn.EnsureOpen().BeginTransaction()) {
                    try{ 


                        Imovel entityDB = conn.Query<Imovel>(i => i.id == entity.id).FirstOrDefault();

                        if(entityDB is not null) {

                            conn.Delete<ImovelImagem>((i) => i.idImovel == entity.id);

                            if(entity.imagens.Count > 0) {

                                entity.imagens.ForEach(i => i.principal = false);
                                entity.imagens[0].principal = true;
                                entityDB.urlImagemPrincipal = entity.imagens[0].url;
                                entityDB.possuiImagens = true;

                                entity.imagens.ForEach(i => { conn.Insert<ImovelImagem>(i); });


                            } else {
                                entityDB.urlImagemPrincipal = "https://jacaptei.com.br/resources/images/logo.png";
                                entityDB.possuiImagens = false;
                            }

                            conn.Update<Imovel>(entityDB);

                            trans.Commit();

                        } else {
                            appReturn.AddException("Não foi possível identificar o imóvel para alterar ou adicionar imagens");
                            trans.Rollback();
                        }
                    } catch(Exception ex) {
                        appReturn.SetAsException("Falha ao alterar ou adicionar imagens do imóvel",ex);
                        trans.Rollback();
                    }
                }

            }
        }

        public List<Imovel> ObterImoveisComImagensCRM(){
            List<Imovel> imoveis = new List<Imovel>();
            try {
                using(var conn = DB.GetConn())
                    imoveis = conn.ExecuteQuery<Imovel>("SELECT imv.*, (SELECT json_agg(img.*) FROM \"ImovelImagem\" img where img.\"idImovel\" = imv.id)  as imagens  FROM \"Imovel\" imv WHERE imv.\"origemImagens\" = 'NETSAC' ").AsList();
            }catch(Exception e){
                var msg = e.Message;
            }
            return imoveis;
        }
        
        public List<ImovelImagem> ObterImovelImagensCRM(){
            List<ImovelImagem> imagens = new List<ImovelImagem>();
            try {
                using(var conn = DB.GetConn())
                    imagens = conn.ExecuteQuery<ImovelImagem>("SELECT * FROM \"ImovelImagem\" WHERE \"vendor\" = 'NETSAC' ").AsList();
            }catch(Exception e){
                var msg = e.Message;
            }
            return imagens;
        }


        public AppReturn Buscar(ImovelBusca busca) {

            List<Imovel> entities = new List<Imovel>();

            if(busca is null || busca.imovel is null)
                appReturn.AddException("Busca não identificada");

            string from =" \"Imovel\" imovel                                                                                           "
                        +" JOIN \"ImovelEndereco\"                    endereco        ON (endereco.\"idImovel\"       = imovel.id)  "
                        +" JOIN \"ImovelValores\"                     valor           ON (valor.\"idImovel\"          = imovel.id)  "
                        +" JOIN \"ImovelAreas\"                       area            ON (area.\"idImovel\"           = imovel.id)  "
                        +" JOIN \"ImovelLazer\"                       lazer           ON (lazer.\"idImovel\"          = imovel.id)  "
                        +" JOIN \"ImovelCaracteristicasInternas\"     interno         ON (interno.\"idImovel\"        = imovel.id)  "
                        +" JOIN \"ImovelCaracteristicasExternas\"     externo         ON (externo.\"idImovel\"        = imovel.id)  "
                        +" JOIN \"ImovelDisposicao\"                  disposicao      ON (disposicao.\"idImovel\"     = imovel.id)  "
                        +" JOIN \"ImovelDocumentacao\"                documentacao    ON (documentacao.\"idImovel\"   = imovel.id)  "
                        +" JOIN \"ImovelTipo\"                        tipo            ON (tipo.id                     = imovel.\"idTipo\")  "
                        +" JOIN \"Proprietario\"                      proprietario    ON (proprietario.id             = imovel.\"idProprietario\")  "
            ;

            string  filter   = ObterQueryBuscaImovel(busca);

            string  sqlCount = "SELECT COUNT(*) "
                        + " FROM " + from
                        + " WHERE " + filter;

            string  sql      = " SELECT JSON_AGG(res) FROM( SELECT      imovel.*                                                                                            "
                        +"                                            , (SELECT json_agg(img.*) FROM \"ImovelImagem\" img where img.\"idImovel\" = imovel.id)  as imagens   "
                        +"                                            , to_json(endereco.*)      as endereco                                                                "
                        +"                                            , to_json(valor.*)         as valor                                                                   "
                        +"                                            , to_json(area.*)          as area                                                                    "
                        +"                                            , to_json(lazer.*)         as lazer                                                                   "
                        +"                                            , to_json(interno.*)       as interno                                                                 "
                        +"                                            , to_json(externo.*)       as externo                                                                 "
                        +"                                            , to_json(disposicao.*)    as disposicao                                                              "
                        +"                                            , to_json(documentacao.*)  as documentacao                                                            "
                        +"                                            , to_json(tipo.*)          as tipo                                                                    ";
                        if(busca.usuarioGod || busca.usuarioGestor)
                            sql+=",    to_json(proprietario.*)  as proprietario                                                                     ";
                        sql+= " FROM " + from
                        +"  WHERE  "            + Utils.Validator.ParseSafeSQL(filter)          + " "
                        +"  ORDER BY imovel."   + Utils.Validator.ParseSafeSQL(busca.orderBy)   + " "
                        +"  " + Utils.Validator.ParseSafeSQL(busca.limit)     + " "
                        +"                           ) res; "
                   ;

            using(var conn = DB.GetConn()) {
                try {

                    busca.total  = conn.ExecuteScalar<Int64>(sqlCount);

                    var res     = conn.ExecuteQuery(sql).FirstOrDefault();
                    if(res?.json_agg is not null)
                        entities    = JsonConvert.DeserializeObject<List<Imovel>>(res.json_agg);

                    busca.result.imoveis = entities;
                    appReturn.result = busca;

                } catch(Exception ex) {
                    appReturn.AddException("Não foi possível buscar imóveis");
                }
            }
            return appReturn;
        }




        private string ObterQueryBuscaImovel(ImovelBusca busca) {

                //string sql = "SELECT * from Products where cf_1019= 'Mogi das Cruzes' and cf_1021 = 'SP';";
                //sql = "SELECT * from Products where cf_1021 = 'SP';";
                //sql = "SELECT * from Products ;";

                string filter = " imovel.\"possuiToken\" = TRUE "; // discontinued <> 1


                if(busca.somenteAtivos)
                     filter += " AND imovel.ativo = TRUE ";
                else if(busca.somenteNaoAativos)
                     filter += " AND imovel.ativo = FALSE ";

                if(busca.somenteValidados)
                     filter += " AND imovel.validado = TRUE ";
                else if(busca.somenteNaoValidados)
                     filter += " AND imovel.validado = FALSE ";

                if(busca.somenteVisiveis)
                     filter += " AND imovel.visivel = TRUE ";
                else if(busca.somenteNaoVisiveis)
                     filter += " AND imovel.visivel = FALSE ";

                if(busca.somenteOutroID)
                     filter += " AND imovel.id <> " +busca.imovel.id.ToString() + " " ;
                else if(busca.imovel.id > 0)
                    filter += " AND imovel.id = " + busca.imovel.id.ToString() + " ";
                else if(Utils.Validator.Is(busca.imovel.cod))
                    filter += " AND imovel.cod = '" + busca.imovel.cod + "' ";
           

                //if(!System.String.IsNullOrWhiteSpace(busca.imovelJC.endereco.cep))
                //    filter += " AND endereco.\"cepNorm\" = '" + busca.imovelJC.endereco.cepNorm + "' ";
                //else if(!System.String.IsNullOrWhiteSpace(busca.cepBase))
                //    filter += " AND endereco.\"cepNorm\" LIKE '%" + busca.cepBase + "%' ";


                if(!System.String.IsNullOrWhiteSpace(busca.imovel.documentacao.indiceCadastral))
                    filter += " AND documentacao.\"indiceCadastral\" = '" + busca.imovel.documentacao.indiceCadastral + "' ";


                if(!System.String.IsNullOrWhiteSpace(busca.imovel.endereco.cepNorm))
                        filter += " AND endereco.\"cepNorm\" LIKE '%" +busca.imovel.endereco.cepNorm + "%' ";

                if(!System.String.IsNullOrWhiteSpace(busca.imovel.endereco.estadoNorm))
                    filter += " AND endereco.\"estadoNorm\" = '" + busca.imovel.endereco.estadoNorm + "' ";

                if(!System.String.IsNullOrWhiteSpace(busca.imovel.endereco.cidadeNorm))
                    filter += " AND endereco.\"cidadeNorm\" = '" + busca.imovel.endereco.cidadeNorm + "' ";

                if(!System.String.IsNullOrWhiteSpace(busca.imovel.endereco.logradouroNorm))
                    filter += " AND endereco.\"logradouroNorm\" LIKE '%" + busca.imovel.endereco.logradouroNorm + "%' ";

                if(!System.String.IsNullOrWhiteSpace(busca.imovel.endereco.numero))
                    filter += " AND endereco.numero = '" + busca.imovel.endereco.numero + "' ";

                if(!System.String.IsNullOrWhiteSpace(busca.imovel.endereco.complementoTipo))
                    filter += " AND endereco.\"complementoTipo\" = '" + busca.imovel.endereco.complementoTipo + "' ";

                if(!System.String.IsNullOrWhiteSpace(busca.imovel.endereco.complemento))
                    filter += " AND endereco.complemento LIKE '%" + busca.imovel.endereco.complemento + "%' ";


                if(busca.imovel.externo.totalVagas > 0)
                    filter += " AND externo.\"totalVagas\" >= " + busca.imovel.externo.totalVagas.ToString();

                if(busca.imovel.interno.totalQuartos > 0)
                    filter += " AND interno.\"totalQuartos\" >= " + busca.imovel.interno.totalQuartos.ToString();

                if(busca.imovel.interno.totalBanheiros > 0)
                    filter += " AND interno.\"totalBanheiros\" >= " + busca.imovel.interno.totalBanheiros.ToString();

                if(busca.imovel.interno.totalSuites > 0)
                    filter += " AND interno.\"totalSuites\" >= " + busca.imovel.interno.totalSuites.ToString();
                //if(!System.String.IsNullOrWhiteSpace(busca.imovelMigrado.tipo))
                //    filter += " AND tipo = '" + busca.imovelMigrado.tipo + "' ";

                if(busca.imovel.idTipo > 0)
                    filter += " AND imovel.\"idTipo\" = " + busca.imovel.idTipo.ToString() + " ";
                else if(busca.imovel.tipo.id > 0)
                    filter += " AND imovel.\"idTipo\" = " + busca.imovel.tipo.id.ToString() + " ";

                if(busca.bairros?.Count > 0) {
                    string items = "'" + String.Join("','", busca.bairros) + "'";
                    filter += " AND endereco.\"bairroNorm\" IN (" + items + ") ";
                }else if(!System.String.IsNullOrWhiteSpace(busca.imovel.endereco.bairroNorm))
                    filter += " AND endereco.\"bairroNorm\" = '" + busca.imovel.endereco.bairroNorm + "' ";


                if(busca.valorMinimo > 0)
                    filter += " AND valor.venda  >=  " + busca.valorMinimo.ToString();
                if(busca.valorMaximo > 0)
                    filter += " AND valor.venda  <=  " + busca.valorMaximo.ToString();


                if(busca.areaMinima > 0)
                    filter += " AND area.total  >=  " + busca.areaMinima.ToString();
                if(busca.areaMaxima > 0)
                    filter += " AND area.total  <=  " + busca.areaMaxima.ToString();

                if(busca.imovel.interno.areaServico)         { filter += " AND interno.\"areaServico\"        = TRUE ";  }
                if(busca.imovel.interno.closet)              { filter += " AND interno.closet                 = TRUE ";  }
                if(busca.imovel.interno.churrasqueira)       { filter += " AND interno.churrasqueira          = TRUE ";  }
                if(busca.imovel.interno.sala )               { filter += " AND interno.sala                   = TRUE ";  }
                if(busca.imovel.interno.armarioBanheiro)     { filter += " AND interno.\"armarioBanheiro\"    = TRUE ";  }
                if(busca.imovel.interno.armarioQuarto)       { filter += " AND interno.\"armarioQuarto\"      = TRUE ";  }
                if(busca.imovel.interno.boxDespejo)          { filter += " AND interno.\"boxDespejo\"         = TRUE ";  }
                if(busca.imovel.interno.lavabo)              { filter += " AND interno.lavabo                 = TRUE ";  }
                if(busca.imovel.interno.dce)                 { filter += " AND interno.dce                    = TRUE ";  }
                if(busca.imovel.interno.aguaIndividual)      { filter += " AND interno.\"aguaIndividual\"     = TRUE ";  }
                if(busca.imovel.interno.gasCanalizado)       { filter += " AND interno.\"gasCanalizado\"      = TRUE ";  }
                if(busca.imovel.interno.armarioCozinha)      { filter += " AND interno.\"armarioCozinha\"     = TRUE ";  }

                if(busca.imovel.externo.cercaEletrica)       { filter += " AND externo.\"cercaEletrica\"      = TRUE ";  }
                if(busca.imovel.externo.jardim)              { filter += " AND externo.jardim                 = TRUE ";  }
                if(busca.imovel.externo.interfone)           { filter += " AND externo.interfone              = TRUE ";  }
                if(busca.imovel.externo.portaoEletronico)    { filter += " AND externo.\"portaoEletronico\"   = TRUE ";  }
                if(busca.imovel.externo.alarme)              { filter += " AND externo.alarme                 = TRUE ";  }
                if(busca.imovel.externo.elevador)            { filter += " AND externo.elevador               = TRUE ";  }
                                                                           
                if(busca.imovel.lazer.hidromassagem)         { filter += " AND lazer.hidromassagem          = TRUE ";  }
                if(busca.imovel.lazer.piscina)               { filter += " AND lazer.piscina                = TRUE ";  }
                if(busca.imovel.lazer.quadraPoliesportiva)   { filter += " AND lazer.\"quadraPoliesportiva\"= TRUE ";  }
                if(busca.imovel.lazer.salaoFestas)           { filter += " AND lazer.\"salaoFestas\"        = TRUE ";  }


                if(busca.imovel.proprietario.id > 0 )
                    filter += " AND proprietario.id = " +busca.imovel.proprietario.id.ToString() + " " ;
                if(Utils.Validator.Is(busca.imovel.proprietario.email))
                    filter += " AND proprietario.email LIKE '%" +busca.imovel.proprietario.email + "%' " ;
                if(Utils.Validator.Is(busca.imovel.proprietario.cpf))
                    filter += " AND proprietario.cpf LIKE '%" +busca.imovel.proprietario.cpf + "%' " ;
                if(Utils.Validator.Is(busca.imovel.proprietario.cnpj))
                    filter += " AND proprietario.cnpj LIKE '%" +busca.imovel.proprietario.cnpj + "%' " ;

                filter = Utils.Validator.ParseSafeSQL(filter);


            return filter;

        }















    }



}
