using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using JaCaptei.Model.Model;
using Newtonsoft.Json;

namespace JaCaptei.Application{

    public class UsuarioAdminDAO: DAOBase{


    
        public AppReturn Inserir(Admin entity){

            //using(var conn = new DBcontext().GetConn())
            //    entity.id           = conn.Insert<Admin,int>(entity);

            using(var conn = new DBcontext().GetConn()) {
                using(var trans = conn.EnsureOpen().BeginTransaction()) {
                    try{
                        entity.id                                       = conn.Insert<Admin,int>(entity);
                        entity.settings.idAdmin                         = entity.id;
                        entity.settings.receberSolicitacaoAgendada      = false;
                        entity.settings.receberSolicitacaoNaoAgendada   = false;
                        entity.settings.id                              = conn.Insert<AdminSettings,int>(entity.settings);
                        trans.Commit();
                    } catch(Exception ex) {
                        appReturn.SetAsException("Falha ao criar admin",ex);
                        trans.Rollback();
                    }
                }
            }

            entity.RemoverDadosSensiveis();
            appReturn.result = entity;

            return appReturn;

        }


        /*

        public AppReturn AlterarBK(Admin entity){

            try {
                using(var conn = DB.GetConn()) {

                    Admin entityDB = conn.Query<Admin>(e => e.id == entity.id).FirstOrDefault();

                    //user.idCRM              = entity.idCRM;
                    if(entityDB is not null && entityDB?.id > 0) {
                        entityDB.nome               = entity.nome;
                        entityDB.senha              = entity.senha;
                        entityDB.ativo              = entity.ativo;
                        entityDB.status             = entity.ativo? "ATIVO" : "INATIVO";
                        entityDB.atualizadoPorId    = entity.atualizadoPorId  ;
                        entityDB.atualizadoPorNome  = entity.atualizadoPorNome;
                        entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();
                        conn.Update<Admin>(entityDB);
                    }
                    else
                        appReturn.AddException("Não foi possível alterar (registro não encontrado ou inválido).");
                }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível alterar (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }
        */
     

        public AppReturn Desativar(Admin entity){

            Admin entityDB = new Admin();

            try {
                using(var conn = DB.GetConn()) {

                    if(Utils.Validator.IsCPF(entity.cpf)) {
                        entity.cpfNum = Utils.Number.ToLong(entity.cpf);
                        entityDB = conn.Query<Admin>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                    } else if(Utils.Validator.IsCNPJ(entity.cnpj)) {
                        entity.cnpjNum = Utils.Number.ToLong(entity.cnpj);
                        entityDB = conn.Query<Admin>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                    } else
                        entityDB = conn.Query<Admin>(e => e.id == entity.id).FirstOrDefault();

                    //user.idCRM              = entity.idCRM;
                    if(entityDB is not null && entityDB?.id > 0) {
                        entityDB.usernameCRM        = entity.usernameCRM;
                        entityDB.senhaCRM           = entity.senhaCRM;
                        entityDB.ativo              = entity.ativo      = false;
                        entityDB.disponivel         = entity.disponivel = false;
                        entityDB.ativoCRM           = entity.ativoCRM = (Utils.Validator.Not(entity.usernameCRM) || Utils.Validator.Not(entity.senhaCRM)); 
                        entityDB.status             = "INATIVO";
                        entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();
                        conn.Update<Admin>(entityDB);
                    }
                    else
                        appReturn.AddException("Não foi possível ativar (registro não encontrado ou inválido).");
                }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível ativar (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }



        public Admin Autenticar(Admin entity) {

            string select = "   SELECT     a.*, " 
                          +"              json_build_object('id',stt.id,'idAdmin',stt.\"idAdmin\",'receberSolicitacaoAgendada',stt.\"receberSolicitacaoAgendada\",'receberSolicitacaoNaoAgendada',stt.\"receberSolicitacaoNaoAgendada\") as \"settings\" " 
                          +"    FROM "          
                          +"              \"Admin\" a JOIN \"AdminSettings\" stt ON (stt.\"idAdmin\" = a.id) " ;

            entity.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());
              select  += " WHERE a.senha = '"+entity.senha+"' ";

            if(Utils.Validator.IsEmail(entity.username)) {
                entity.email =   Utils.String.HigienizeMail(entity.username);
                select          += " AND a.email = '"+entity.email+"' ";
            } else if(Utils.Validator.IsCPF(entity.cpf)) {
                entity.cpfNum    =   Utils.Number.ToLong(entity.username);
                select          += " AND a.\"cpfNum\" = "+entity.cpfNum.ToString()+" ";
            } else {
                entity.cnpjNum   =   Utils.Number.ToLong(entity.username);
                select          += " AND a.\"cnpjNum\" = "+entity.cnpjNum.ToString()+" ";
            }

            string  sql     = "SELECT ROW_TO_JSON(res) FROM  ( " + select  + " ORDER BY a.id ) res ";


            Admin entityDB = null;

            using(var conn = DB.GetConn()) {
                var res     = conn.ExecuteQuery(sql).FirstOrDefault();
                if(res?.row_to_json is not null)
                    entityDB   = JsonConvert.DeserializeObject<Admin>(res.row_to_json);
            }
            return entityDB;
        }





        public Admin Autenticar_BK_SemSettings(Admin entity) {

            Admin entityDB = null;

            entity.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());

            using(var conn = DB.GetConn()) {
                if(Utils.Validator.IsEmail(entity.username)) {
                    entity.email    =   Utils.String.HigienizeMail(entity.username);
                    entityDB        =   conn.Query<Admin>(e => e.ativo == true && e.email == entity.email && e.senha == entity.senha).FirstOrDefault();
                } else  if(Utils.Validator.IsCPF(entity.cpf))  {
                    entity.cpfNum   =   Utils.Number.ToLong(entity.username);
                    entityDB        =   conn.Query<Admin>(e =>e.ativo == true &&  e.cpfNum == entity.cpfNum && e.senha == entity.senha).FirstOrDefault();
                } else{
                    entity.cnpjNum   =   Utils.Number.ToLong(entity.username);
                    entityDB        =   conn.Query<Admin>(e => e.ativo == true && e.cnpjNum == entity.cnpjNum && e.senha == entity.senha).FirstOrDefault();
                }
            }
            return entityDB;
        }


        public Admin ObterPorUsername(Admin entity) {
            return ObterPorDocumentoOuEmail(entity);
        }



        public Admin ObterPorDocumentoOuEmail(Admin entity) {

            Admin entityDB = null;

            if(Utils.Validator.IsEmail(entity.username))
                entity.email = Utils.String.HigienizeMail(entity.username);
            else if(Utils.Validator.IsCPF(entity.username))
                entity.cpf = entity.username;
            else if(Utils.Validator.IsCNPJ(entity.username))
                entity.cnpj = entity.username;

            using(var conn = DB.GetConn()) {
                if(Utils.Validator.IsEmail(entity.email)) {
                    entity.email    =   Utils.String.HigienizeMail(entity.email);
                    entityDB        =   conn.Query<Admin>(e => e.email == entity.email).FirstOrDefault();
                } else if(Utils.Validator.IsCPF(entity.cpf)){
                    entity.cpfNum   =   Utils.Number.ToLong(entity.cpf);
                    entityDB        =   conn.Query<Admin>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                } else if(Utils.Validator.IsCNPJ(entity.cnpj)){
                    entity.cnpjNum  =   Utils.Number.ToLong(entity.cnpj);
                    entityDB        =   conn.Query<Admin>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                }else
                    entityDB = null;
            }
            return entityDB;
        }


        public Admin ObterPorCamposChaves(Admin entity) {

            Admin entityDB = null;

            using(var conn = DB.GetConn()) {

                if(entity.cpfNum > 0)
                    entityDB = conn.Query<Admin>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                else if(entity.cnpjNum > 0)
                    entityDB = conn.Query<Admin>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                
                if(entityDB is null || entityDB?.id == 0)
                    entityDB = conn.Query<Admin>(e => e.email == entity.email).FirstOrDefault();
               
            }
            return entityDB;
        }






        public Admin ObterPeloToken(string token) {

            Admin entityDB = null;

            using(var conn = DB.GetConn()) 
                    entityDB = conn.Query<Admin>(e => e.token == token).FirstOrDefault();

            return entityDB;
        }



        public AppReturn AlterarDisponibilidade(Admin entity) {

            try {
                    Admin entityDB = null;
                    using(var conn = new DBcontext().GetConn()) {
                        entityDB = conn.Query<Admin>(e => e.id == entity.id).FirstOrDefault();
                        if(entityDB is not null && entityDB?.id > 0) {
                            entityDB.disponivel         = entity.disponivel;
                            entityDB.atualizadoPorId    = entity.atualizadoPorId;
                            entityDB.atualizadoPorNome  = entity.atualizadoPorNome;
                            entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();
                        conn.Update(entityDB);
                        }
                        else
                            appReturn.AddException("Não foi possível alterar disponibilidade (usuário não encontrado ou inválido).");
                    }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível alterar disponibilidade.");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }
        


        public AppReturn AlterarSenha(Admin entity) {

            try {
                    var param = new { entity.token };
                    Admin entityDB = null;
                    using(var conn = new DBcontext().GetConn()) {
                        entityDB = conn.Query<Admin>(e => e.token == entity.token).FirstOrDefault();
                        if(entityDB is not null && entityDB?.id > 0) {
                            entityDB.senha = entity.senha;
                            entityDB.token = entity.token = Utils.Key.CreateToken(entityDB.id.ToString());
                            conn.Update(entityDB);
                            entityDB.senha = "";
                        }
                        else
                            appReturn.AddException("Não foi possível alterar senha (registro não encontrado ou inválido).");
                }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível alterar senha (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }
        


        public AppReturn Alterar(Admin entity) {

            try {
                    var param = new { entity.token };
                    Admin entityDB = null;
                    using(var conn = new DBcontext().GetConn()) {
                        entityDB = conn.Query<Admin>(e => e.id == entity.id && e.token == entity.token).FirstOrDefault();

                        if(entityDB is not null && entityDB?.id > 0) {

                            if(Utils.Validator.Is(entity.senha))
                                entityDB.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());
                            if(Utils.Validator.Is(entity.email))
                                entityDB.email = Utils.String.HigienizeMail(entity.email);  

                            entityDB.nome               = entity.nome;
                            entityDB.ativo              = entity.ativo;
                            //entityDB.status             = entity.ativo? "ATIVO" : "INATIVO";
                            entityDB.atualizadoPorId    = entity.atualizadoPorId  ;
                            entityDB.atualizadoPorNome  = entity.atualizadoPorNome;
                            entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();

                            conn.Update(entityDB);
                            entityDB.senha = "";

                        }
                        else
                            appReturn.AddException("Não foi possível alterar perfil (registro não encontrado ou inválido).");
                }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível alterar perfil (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }




        public Admin ObterPorId(int id) {

            Admin entityDB = null;

            using(var conn = DB.GetConn()) 
                entityDB = conn.Query<Admin>(e => e.id == id).FirstOrDefault();

            return entityDB;
        }



       public List<Admin> ObterInativos() {
            List<Admin> entities = null;
            using(var conn = DB.GetConn())
                entities = conn.Query<Admin>(e => e.ativo == false).ToList();
                //entities = conn.Query<Admin>(e => e.ativo == false && e.confirmado == true).ToList();
            return entities;
        }





    }
}
