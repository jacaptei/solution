﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using JaCaptei.Model.Model;
using Npgsql;
using Newtonsoft.Json;
using RepoDb.Enumerations;

namespace JaCaptei.Application{

    public class ParceiroDAO : DAOBase{

        //public AppReturn Inserir(Parceiro entity){

        //    using (var conn = new DBcontext().GetConn())
        //        entity.id = conn.Insert<Parceiro, int>(entity);

        //    entity.RemoverDadosSensiveis();
        //    appReturn.result = entity;
        //    return appReturn;

        //}

        
        public AppReturn Adicionar(Parceiro entity){

            Conta conta = new Conta();
            Plano plano = new Plano();

                using(var conn = new DBcontext().GetConn()) {
                    using(var trans = conn.EnsureOpen().BeginTransaction()) {

                    try {
                        if(Utils.Validator.Not(entity.tokenConta)) {

                            plano = conn.Query<Plano>(e => e.id == entity.idPlano).FirstOrDefault();

                            if(plano is null || plano?.id == 0)
                                appReturn.AddException("Plano não encontrado.");
                            else{
                                    entity.donoConta     =  true;
                                    conta.idPlano        =  entity.idPlano    ;
                                    conta.valorMensal    =  plano.valorMensal ;
                                    conta.nome           =  entity.nome       ;
                                    conta.razao          =  entity.razao      ;
                                    conta.responsavel    =  entity.responsavel;
                                    conta.tipoPessoa     =  entity.tipoPessoa ;
                                    conta.cpf            =  entity.cpf        ;
                                    conta.cpfNum         =  entity.cpfNum     ;
                                    conta.cnpj           =  entity.cnpj       ;
                                    conta.cnpjNum        =  entity.cnpjNum;
                                    conta.token          =  Utils.Key.CreateToken();

                                    conta.totalUsuarios  = 1; 
                                    conta.limiteUsuarios = (conta.idPlano == 4)? 6 : (conta.idPlano == 3) ? 4 : 1; 

                                    conta.data           =  conta.dataAtualizacao = Utils.Date.GetLocalDateTime();
                                    conta.id             =  conn.Insert<Conta,int>(conta);
                            }
                        } else {
                            entity.donoConta = false;
                            var tokenConta = Utils.String.RemoveAllSpaces(entity.tokenConta);
                            conta = conn.Query<Conta>(e => e.token == tokenConta).FirstOrDefault();
                            if(conta is not null && conta?.id > 0) {
                                if(conta.totalUsuarios >= conta.limiteUsuarios)
                                    appReturn.AddException("Esta conta não contempla mais usuários.");
                                else {
                                    conta.totalUsuarios += 1;
                                    conta.dataAtualizacao = Utils.Date.GetLocalDateTime();
                                    conn.Update<Conta>(conta);
                                }
                            }else
                                appReturn.AddException("Token da conta não encontrado ou inválido (solicite o token da conta com o dono da mesma).");
                        }

                        if(appReturn.status.success) { 
                                if(conta.id > 0 && Utils.Validator.Is(conta.token)) {
                                    entity.idPlano              = conta.idPlano;
                                    entity.idConta              = conta.id;
                                    entity.tokenConta           = conta.token;
                                    entity.data                 = entity.dataAtualizacao = conta.data;
                                    entity.id                   = conn.Insert<Parceiro,int>(entity);
                                    entity.settings.idParceiro  = entity.id;
                                    entity.settings.id          = conn.Insert<ParceiroSettings,int>(entity.settings);
                                    trans.Commit();
                                } else {
                                    trans.Rollback();
                                    appReturn.SetAsException("Falha ao criar conta");
                                }
                        } else {
                            trans.Rollback();
                            //appReturn.SetAsException("Falha ao criar conta");
                        }

                    }catch(Exception ex) {
                        appReturn.SetAsException("Falha ao inserir",ex);
                        trans.Rollback();
                    }
                }
            }

            entity.RemoverDadosSensiveis();
            appReturn.result = entity;

            return appReturn;

        }




        public AppReturn Confirmar(string token){

            try {
                using(var conn = new DBcontext().GetConn()) {

                    Parceiro entityDB = conn.Query<Parceiro>(e => e.token == token).FirstOrDefault();

                    if(entityDB is not null && entityDB?.id > 0) {
                        entityDB.confirmado      = true;
                        entityDB.status          = "CONFIRMADO";
                        //entityDB.token           = Utils.Key.CreateToken();
                        entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();
                        conn.Update<Parceiro>(entityDB);
                    }
                    else
                        appReturn.AddException("Não foi possível confirmar cadastro com token especificado (não encontrado ou inválido).");
                }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível confirmar cadastro com token especificado (não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }

     

        public AppReturn Validar(Parceiro entity){
            Parceiro entityDB = new Parceiro();
            try {
                using(var conn = new DBcontext().GetConn()) {

                    if(Utils.Validator.IsCPF(entity.cpf)) {
                        entity.cpfNum = Utils.Number.ToLong(entity.cpf);
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                    } else if(Utils.Validator.IsCNPJ(entity.cnpj)){
                        entity.cnpjNum = Utils.Number.ToLong(entity.cnpj);
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                    }else
                        entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

                    //user.idCRM              = entity.idCRM;
                    if(entityDB is not null && entityDB?.id > 0) {
                        entityDB.senhaCRM           = entity.senhaCRM;
                        entityDB.usernameCRM        = entity.usernameCRM;
                        entityDB.ativoCRM           = entity.ativoCRM   = (Utils.Validator.Not(entity.usernameCRM) || Utils.Validator.Not(entity.senhaCRM));
                        entityDB.confirmado         = entity.confirmado = true;
                        entityDB.validado           = entity.validado   = true;
                        entityDB.ativo              = entity.ativo      = true;
                        entityDB.status             = "ATIVO";
                        entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();
                        conn.Update<Parceiro>(entityDB);
                    }
                    else
                        appReturn.AddException("Não foi possível validar (registro não encontrado ou inválido).");
                }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível validar (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }

     
        public AppReturn Ativar(Parceiro entity){
            Parceiro entityDB = new Parceiro();
            try {
                using(var conn = new DBcontext().GetConn()) {

                    if(Utils.Validator.IsCPF(entity.cpf)) {
                        entity.cpfNum = Utils.Number.ToLong(entity.cpf);
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                    } else if(Utils.Validator.IsCNPJ(entity.cnpj)){
                        entity.cnpjNum = Utils.Number.ToLong(entity.cnpj);
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                    }else
                        entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

                    //user.idCRM              = entity.idCRM;
                    if(entityDB is not null && entityDB?.id > 0) {
                        entityDB.confirmado         = entity.confirmado = true;
                        entityDB.validado           = entity.validado   = true; 
                        entityDB.ativo              = entity.ativo      = true;
                        entityDB.ativoCRM           = entity.ativoCRM   = (Utils.Validator.Not(entity.usernameCRM) || Utils.Validator.Not(entity.senhaCRM));
                        entityDB.status             = "ATIVO";
                        entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();

                        entityDB.atualizadoPorId    = entity.atualizadoPorId;
                        entityDB.atualizadoPorNome  = entity.atualizadoPorNome;
                        entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();

                        conn.Update<Parceiro>(entityDB);
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

     

        public AppReturn Desativar(Parceiro entity){

            Parceiro entityDB = new Parceiro();

            try {
                using(var conn = new DBcontext().GetConn()) {

                    if(Utils.Validator.IsCPF(entity.cpf)) {
                        entity.cpfNum = Utils.Number.ToLong(entity.cpf);
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                    } else if(Utils.Validator.IsCNPJ(entity.cnpj)) {
                        entity.cnpjNum = Utils.Number.ToLong(entity.cnpj);
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                    } else
                        entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

                    //user.idCRM              = entity.idCRM;
                    if(entityDB is not null && entityDB?.id > 0) {
                        entityDB.ativo              = entity.ativo = false;
                        entityDB.ativoCRM           = entity.ativoCRM = (Utils.Validator.Not(entity.usernameCRM) || Utils.Validator.Not(entity.senhaCRM)); 
                        entityDB.status             = "INATIVO";
                        entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();

                        entityDB.atualizadoPorId    = entity.atualizadoPorId;
                        entityDB.atualizadoPorNome  = entity.atualizadoPorNome;
                        entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();

                        conn.Update<Parceiro>(entityDB);
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



        public AppReturn Autenticar(Parceiro entity) {

            Parceiro entityDB = null;

            entity.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());

            using(var conn = new NpgsqlConnection(DB.CS)) {
                try{
                    if(Utils.Validator.IsEmail(entity.username)) {
                        entity.email    =   Utils.String.HigienizeMail(entity.username);
                        entityDB        =   conn.Query<Parceiro>(e => e.email == entity.email && e.senha == entity.senha).FirstOrDefault();
                    } else  if(Utils.Validator.IsCPF(entity.cpf))  {
                        entity.cpfNum   =   Utils.Number.ToLong(entity.username);
                        entityDB        =   conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum && e.senha == entity.senha).FirstOrDefault();
                    } else{
                        entity.cnpjNum   =   Utils.Number.ToLong(entity.username);
                        entityDB        =   conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum && e.senha == entity.senha).FirstOrDefault();
                    }
                }catch(Exception e){
                        appReturn.AddException(e.ToString());
                }
            }
            appReturn.result = entityDB;
            return appReturn;
        }


        public Parceiro ObterPorUsername(Parceiro entity) {
            return ObterPorDocumentoOuEmail(entity);
        }



        public Parceiro ObterPorDocumentoOuEmail(Parceiro entity) {

            Parceiro entityDB = null;

            if(Utils.Validator.IsEmail(entity.username))
                entity.email = Utils.String.HigienizeMail(entity.username);
            else if(Utils.Validator.IsCPF(entity.username))
                entity.cpf = entity.username;
            else if(Utils.Validator.IsCNPJ(entity.username))
                entity.cnpj = entity.username;

            using(var conn = new DBcontext().GetConn()) {
                if(Utils.Validator.IsEmail(entity.email)) {
                    entity.email    =   Utils.String.HigienizeMail(entity.email);
                    entityDB        =   conn.Query<Parceiro>(e => e.email == entity.email).FirstOrDefault();
                } else if(Utils.Validator.IsCPF(entity.cpf)){
                    entity.cpfNum   =   Utils.Number.ToLong(entity.cpf);
                    entityDB        =   conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                } else if(Utils.Validator.IsCNPJ(entity.cnpj)){
                    entity.cnpjNum  =   Utils.Number.ToLong(entity.cnpj);
                    entityDB        =   conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                }else
                    entityDB = null;
            }

            /*
            using(var conn = new DBcontext().GetConn()) {
                if(Utils.Validator.IsEmail(entity.email)) {
                    entity.email    =   Utils.String.HigienizeMail(entity.email);
                    entityDB        =   conn.Query<Parceiro>(e => e.email == entity.email).FirstOrDefault();
                } else if(Utils.Validator.IsCPF(entity.cpf) && Utils.Validator.IsCNPJ(entity.cnpj)) {
                    entity.cpfNum   =   Utils.Number.ToLong(entity.cpf);
                    entity.cnpjNum  =   Utils.Number.ToLong(entity.cnpj);
                    entityDB        =   conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum && e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                }  else if(Utils.Validator.IsCPF(entity.cpf)){
                    entity.cpfNum   =   Utils.Number.ToLong(entity.cpf);
                    entityDB        =   conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                } else if(Utils.Validator.IsCNPJ(entity.cnpj)){
                    entity.cnpjNum  =   Utils.Number.ToLong(entity.cnpj);
                    entityDB        =   conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                }else
                    entityDB = null;
            }             
             */


            return entityDB;
        }


        public Parceiro ObterPorCamposChaves(Parceiro entity) {

            Parceiro entityDB = null;

            using(var conn = new DBcontext().GetConn()) {

                if(entity.cpfNum > 0)
                    entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                else if(entity.cnpjNum > 0)
                    entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum).FirstOrDefault();
                
                if(entityDB is null || entityDB?.id == 0)
                    entityDB = conn.Query<Parceiro>(e => e.email == entity.email).FirstOrDefault();
               
            }
            return entityDB;
        }



        public Parceiro ObterPorCamposChavesParaAlteracao(Parceiro entity) {

            Parceiro entityDB = null;

            using(var conn = new DBcontext().GetConn()) {

                 entityDB = conn.Query<Parceiro>(e => e.email == entity.email && e.id != entity.id).FirstOrDefault();

                if(entityDB is null || entityDB?.id == 0) {
                    if(entity.cpfNum > 0)
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum && e.id != entity.id).FirstOrDefault();
                    if(entity.cnpjNum > 0)
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum && e.id != entity.id).FirstOrDefault();
                }
               
            }
            return entityDB;
        }






        public Parceiro ObterPeloToken(string token) {

            Parceiro entityDB = null;

            using(var conn = new DBcontext().GetConn()) 
                    entityDB = conn.Query<Parceiro>(e => e.token == token).FirstOrDefault();

            return entityDB;
        }



        public AppReturn AlterarSenha(Parceiro entity) {

            try {
                    var param = new { entity.token };
                    Parceiro entityDB = null;
                    using(var conn = new DBcontext().GetConn()) {
                        entityDB = conn.Query<Parceiro>(e => e.token == entity.token).FirstOrDefault();
                        if(entityDB is not null && entityDB?.id > 0) {
                            entityDB.senha = entity.senha;
                            //entityDB.token = entity.token = Utils.Key.CreateToken(entityDB.id.ToString());
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
        


        public AppReturn AlterarPerfil(Parceiro entity) {

            try {
                    var param = new { entity.token };
                    Parceiro entityDB = null;
                    using(var conn = new DBcontext().GetConn()) {
                        entityDB = conn.Query<Parceiro>(e => e.id == entity.id && e.token == entity.token).FirstOrDefault();

                        if(entityDB is not null && entityDB?.id > 0) {

                            if(Utils.Validator.Is(entity.senha))
                                entityDB.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());
                            if(Utils.Validator.Is(entity.email))
                                entityDB.email = Utils.String.HigienizeMail(entity.email);  

                            //entityDB.token = entity.token = Utils.Key.CreateToken(entityDB.id.ToString());
                            entityDB.dataAtualizacao = Utils.Date.GetLocalDateTime();

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




        public Parceiro ObterPorId(int id) {

            Parceiro entityDB = null;

            using(var conn = new DBcontext().GetConn()) 
                entityDB = conn.Query<Parceiro>(e => e.id == id).FirstOrDefault();

            return entityDB;
        }



       public List<Parceiro> ObterPendentesValidacao() {
            List<Parceiro> entities = null;
            using(var conn = new DBcontext().GetConn()) {
                entities = conn.Query<Parceiro>(e => e.validado == false,orderBy: OrderField.Parse(new { data = Order.Descending }) ).ToList();
                //Admin adminDB = conn.Query<Admin>(a=> a.god == false && a.gestor == false && a.disponivel == true,orderBy:OrderField.Parse(new{ dataCod = Order.Ascending })  ).FirstOrDefault();
                //entities = conn.Query<Parceiro>(e => e.ativo == false && e.confirmado == true).ToList();
                entities.ForEach((e) => {
                    e.conta  = conn.Query<Conta>(c => c.id == e.idConta).FirstOrDefault();
                });
            }
            return entities;
        }



       public List<Parceiro> ObterInativos() {
            List<Parceiro> entities = null;
            using(var conn = new DBcontext().GetConn())
                entities = conn.Query<Parceiro>(e => e.ativo == false).ToList();
                //entities = conn.Query<Parceiro>(e => e.ativo == false && e.confirmado == true).ToList();
            return entities;
        }







        //                              FUNCOES ADMIN
        // -----------------------------------------------------------------


        public AppReturn Alterar(Parceiro entity) {
            //using(var conn = DB.GetConn())
            //    conn.Update<Parceiro>(entity);


            try {
                Parceiro entityDB = null;
                using(var conn = new DBcontext().GetConn()) {

                    entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

                    if(entityDB is not null && entityDB?.id > 0) {

                        entityDB.tipoPessoa     = entity.tipoPessoa;
                        entityDB.nome           = entity.nome;
                        entityDB.razao          = entity.razao;
                        entityDB.responsavel    = entity.responsavel;
                        
                        entityDB.rg             = entity.rg;
                        entityDB.cpf            = entity.cpf;
                        entityDB.cpfNum         = entity.cpfNum;
                        entityDB.cnpj           = entity.cnpj;
                        entityDB.cnpjNum        = entity.cnpjNum;
                        
                        entityDB.email          = entity.email;
                        entityDB.telefone       = entity.telefone;
                        entityDB.telefone2      = entity.telefone2;
                        entityDB.telefone3      = entity.telefone3;

                        entityDB.dataNascimento = entity.dataNascimento;
                        entityDB.diaNascimento  = entity.diaNascimento;
                        entityDB.mesNascimento  = entity.mesNascimento;
                        entityDB.anoNascimento  = entity.anoNascimento;
                        entityDB.sexo           = entity.sexo;

                        entityDB.creci          = entity.creci;
                        entityDB.creciEstado    = entity.creciEstado;
                        
                        entityDB.cep            = entity.cep;
                        entityDB.cepNorm        = entity.cepNorm;
                        entityDB.estado         = entity.estado;
                        entityDB.estadoNorm     = entity.estadoNorm;
                        entityDB.cidade         = entity.cidade;
                        entityDB.cidadeNorm     = entity.cidadeNorm;
                        entityDB.bairro         = entity.bairro;
                        entityDB.bairroNorm     = entity.bairroNorm;
                        entityDB.logradouro     = entity.logradouro;
                        entityDB.logradouroNorm = entity.logradouroNorm;
                        entityDB.numero         = entity.numero;
                        entityDB.complemento    = entity.complemento;
                        entityDB.complemento    = entity.complemento;
                        entityDB.usernameCRM    = entity.usernameCRM;
                        entityDB.senhaCRM       = entity.senhaCRM;
                        entityDB.obs            = entity.obs;

                        entityDB.ativo          = entity.ativo;
                        entityDB.excluido       = false;

                        entityDB.atualizadoPorId    = entity.atualizadoPorId;
                        entityDB.atualizadoPorNome  = entity.atualizadoPorNome;
                        entityDB.dataAtualizacao    = Utils.Date.GetLocalDateTime();

                        if(Utils.Validator.Is(entity.senha))
                            entityDB.senha = entity.senha;

                        conn.Update(entityDB);


                    
                    } else
                        appReturn.AddException("Não foi possível alterar parceiro (registro não encontrado ou inválido).");
                }
            } catch(Exception ex) {
                appReturn.AddException("Não foi possível alterar parceiro (registro não encontrado ou inválido).");
                appReturn.status.exception = ex.ToString();
            }

            return appReturn;

        }



        

        public AppReturn Excluir(Parceiro entity) {
           
            try {
                Parceiro entityDB = null;
                using(var conn = new DBcontext().GetConn()) {

                    entityDB = conn.Query<Parceiro>(e => e.id == entity.id).FirstOrDefault();

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









        public AppReturn Buscar(Search busca) {

            List<Parceiro> entities = new List<Parceiro>();

            if(busca is null || busca.item is null)
                appReturn.AddException("Busca não identificada");

            string select   = "SELECT     p.* , " +
                            "             json_build_object('id',c.id,'idPlano',c.\"idPlano\",'razao',c.razao,'nome',c.nome,'responsavel',c.responsavel,'token',c.token) as \"conta\"   "    +              
                            "      FROM "           +
                            "              \"Parceiro\"  p JOIN \"Conta\" c ON (p.\"idConta\" = c.id) ";

            string filter = " WHERE p.validado = 'TRUE'  ";


            if(busca.item?.id > 0)
                filter += " AND p.id = " + busca.item.id.ToString();
            else if(busca.item?.cpfNum > 0)
                filter += " AND p.\"cpfNum\" = " + busca.item.cpfNum.ToString();
            else if(busca.item?.cnpjNum > 0)
                filter += " AND p.\"cnpjNum\" = " + busca.item.cnpjNum.ToString();
            else if(busca.item.conta?.id > 0)
                filter += " AND c.id = " + busca.item.conta.id.ToString();
            else if(Utils.Validator.Is(busca.item?.conta?.token))
                filter += " AND c.token LIKE '%" + busca.item.conta.token + "%' ";
            
        //    else{

                    if(Utils.Validator.Is(busca.item?.nome))
                        filter += " AND p.nome LIKE '%" + busca.item.nome + "%' ";

                    if(Utils.Validator.Is(busca.item?.email))
                        filter += " AND p.email LIKE '%" + busca.item.email + "%' ";

                    if(Utils.Validator.Is(busca.item?.telefone))
                        filter += " AND p.telefone LIKE '%" + busca.item.telefone + "%' ";

                    if(busca.idStatus == 1)
                        filter += " AND p.ativo = TRUE ";
                    else if(busca.idStatus == 2)
                        filter += " AND p.ativo = FALSE ";
                    if(busca.idStatus == 12)
                        filter += " AND p.excluido = TRUE ";

        //    }


            string  complete  = (Utils.Validator.Is(busca.item?.conta?.token) || busca.item.conta?.id > 0 )? " ORDER BY p.\"donoConta\" DESC " : " ORDER BY p.\"data\" DESC " ;
                    //complete += " LIMIT 2";
                    //complete += busca.item.id <= -1? " LIMIT 200 ":"";
                    complete += " LIMIT "+ busca.resultsPerPage + " OFFSET "+ busca.offset + " ";

            string sqlCount = "SELECT COUNT(*) FROM \"Parceiro\"  p JOIN \"Conta\" c ON (p.\"idConta\" = c.id)  " + filter;
            string sql      = "SELECT JSON_AGG(res) FROM  ( " + select + filter  + complete + " ) res ; ";


            using(var conn = DB.GetConn()) {
                try {

                    busca.total  = conn.ExecuteScalar<Int64>(sqlCount);

                    var res     = conn.ExecuteQuery(sql).FirstOrDefault();
                    if(res?.json_agg is not null)
                        entities    = JsonConvert.DeserializeObject<List<Parceiro>>(res.json_agg);

                    busca.result     = entities;
                    appReturn.result = busca;

                } catch(Exception ex) {
                    appReturn.AddException("Não foi possível buscar solicitações");
                }
            }
            return appReturn;
        }





        public AppReturn BuscarConta(Search busca) {

            string sql      = "SELECT * FROM \"Parceiro\" ";
            string sqlCount = "SELECT COUNT(*) FROM \"Parceiro\" ";

            string filter = " WHERE ativo = true ";

            if(busca.item?.id > 0)
                filter += " AND id = " + busca.item.id.ToString();
            if(Utils.Validator.Is(busca.item.cpf))
                filter += " AND cpf LIKE '%" + busca.item.cpf + "%' ";
            if(Utils.Validator.Is(busca.item.cnpj))
                filter += " AND cnpj LIKE '%" + busca.item.cnpj.ToUpper() + "%' ";
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
                    busca.result = conn.ExecuteQuery<Parceiro>(sql).ToList();
                    appReturn.result = busca;
                } catch(Exception ex) {
                    appReturn.AddException("Não foi possível buscar parceiros.");
                }
            }
            return appReturn;
        }







    }
}
