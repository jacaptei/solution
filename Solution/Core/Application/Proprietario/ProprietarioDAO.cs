using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using JaCaptei.Model.Model;

namespace JaCaptei.Application{

    public class ProprietarioDAO : DAOBase{

        public AppReturn Adicionar(Proprietario entity){

            Conta conta = new Conta();
            Plano plano = new Plano();

            using(var conn = DB.GetConn()) {

                        entity.idPlano      = 0;
                        entity.idConta      = 0;
                        entity.tokenConta   = conta.token;
                        entity.data         = entity.dataAtualizacao = conta.data;
                        entity.id           = conn.Insert<Proprietario,int>(entity);
            }

            entity.RemoverDadosSensiveis();
            appReturn.result = entity;
            return appReturn;
        }



        public AppReturn Alterar(Proprietario entity) {
            using(var conn = DB.GetConn())
               conn.Update<Proprietario>(entity);
            return appReturn;
        }


        public AppReturn Excluir(Proprietario entity) {

            try {
                Proprietario entityDB = null;
                using(var conn = new DBcontext().GetConn()) {

                    entityDB = conn.Query<Proprietario>(e => e.id == entity.id).FirstOrDefault();

                    if(entityDB is not null && entityDB?.id > 0) {

                        //entityDB.tipoPessoa     = entity.tipoPessoa;
                        //entityDB.nome           = entity.nome;
                        //entityDB.razao          = entity.razao;
                        //entityDB.responsavel    = entity.responsavel;
                        //
                        //entityDB.rg             = entity.rg;
                        //entityDB.cpf            = entity.cpf;
                        //entityDB.cpfNum         = entity.cpfNum;
                        //entityDB.cnpj           = entity.cnpj;
                        //entityDB.cnpjNum        = entity.cnpjNum;
                        //
                        //entityDB.email          = entity.email;
                        entityDB.telefone       = "";
                        entityDB.telefone2      = "";
                        entityDB.telefone3      = "";

                        entityDB.dataNascimento = Utils.Date.GetUnsetDefaultDateTime();
                        entityDB.diaNascimento  = 1;
                        entityDB.mesNascimento  = 1;
                        entityDB.anoNascimento  = 1900;
                        entityDB.sexo           = "NA";

                        entityDB.creci          = "";
                        entityDB.creciEstado    = "";

                        entityDB.cep            = "";
                        entityDB.cepNorm        = "";
                        entityDB.estado         = "";
                        entityDB.estadoNorm     = "";
                        entityDB.cidade         = "";
                        entityDB.cidadeNorm     = "";
                        entityDB.bairro         = "";
                        entityDB.bairroNorm     = "";
                        entityDB.logradouro     = "";
                        entityDB.logradouroNorm = "";
                        entityDB.numero         = "";
                        entityDB.complemento    = "";
                        entityDB.complemento    = "";
                        entityDB.usernameCRM    = "";
                        entityDB.senhaCRM       = "";
                        entityDB.obs            = "";

                        entityDB.senha          = "";
                        entityDB.ativo          = false;
                        entityDB.excluido       = true;

                        entityDB.atualizadoPorId    = entity.atualizadoPorId;
                        entityDB.atualizadoPorNome  = entity.atualizadoPorNome;
                        entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();

                        conn.Update(entityDB);

                        appReturn.result = entityDB;

                    } else
                        appReturn.AddException("Não foi possível excluir parceiro (registro não encontrado ou inválido).");
                }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível excluir parceiro (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }



        public Proprietario ObterPeloId(int id) {

            Proprietario entityDB = null;

            using(var conn = DB.GetConn()) 
                entityDB = conn.Query<Proprietario>(e => e.id == id).FirstOrDefault();

            return entityDB;
        }


        public Proprietario ObterPorCamposChaves(Proprietario entity) {

            Proprietario entityDB = null;

            using(var conn = DB.GetConn()) {

                if(entity.cpfNum > 0)
                    entityDB = conn.Query<Proprietario>(e => e.id != entity.id && e.cpfNum == entity.cpfNum).FirstOrDefault();
                else if(entity.cnpjNum > 0)
                    entityDB = conn.Query<Proprietario>(e => e.id != entity.id && e.cnpjNum == entity.cnpjNum).FirstOrDefault();

                if(entityDB is null || entityDB?.id == 0)
                    entityDB = conn.Query<Proprietario>(e => e.id != entity.id && e.email == entity.email).FirstOrDefault();

            }
            return entityDB;
        }


        public AppReturn Buscar(Search busca) {

            string sql      = "SELECT * FROM \"Proprietario\" ";
            string sqlCount = "SELECT COUNT(*) FROM \"Proprietario\" ";

            string filter = " WHERE id > 0 ";
            
            if(busca.item?.id > 0)
                filter += " AND id = " + busca.item.id.ToString();
            if(Utils.Validator.Is(busca.item.cpf))
                filter += " AND cpf LIKE '%" + busca.item.cpf + "%' ";
            if(Utils.Validator.Is(busca.item.rg))
                filter += " AND rg LIKE '%" + busca.item.rg.ToUpper() + "%' ";
            if(Utils.Validator.Is(busca.item.nome))
                filter += " AND nome LIKE '%" + busca.item.nome.ToUpper() + "%' ";
            if(Utils.Validator.Is(busca.item.email))
                filter += " AND email LIKE '%" + busca.item.email.ToLower() + "%' ";
            if(Utils.Validator.Is(busca.item.telefone))
                filter += " AND telefone LIKE '%" + busca.item.telefone + "%' ";

            sql      += filter + " ORDER BY " + busca.orderBy;
            sql      += " LIMIT "+ busca.resultsPerPage + " OFFSET "+ busca.offset + " ;";
            sqlCount += filter;

            using(var conn = DB.GetConn()) {
                try {
                    busca.total  = conn.ExecuteScalar<Int64>(sqlCount);
                    busca.result = conn.ExecuteQuery<Proprietario>(sql).ToList();
                    appReturn.result = busca;
                }catch(Exception ex) {
                    appReturn.AddException("Não foi possível buscar proprietários");
                }
            }
            return appReturn;
        }




    }
}
