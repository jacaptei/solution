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

    public class UsuarioAdminService: ServiceBase{


        UsuarioAdminBLO BLO = new UsuarioAdminBLO();
        UsuarioAdminDAO DAO = new UsuarioAdminDAO();


        public AppReturn ObterPeloId(int id){

            if(id == 0) {
                appReturn.SetAsBadRequest("ID não informado.");
                return appReturn;
            }

            Admin entity = DAO.ObterPorId(id);

            if(entity is null || entity?.id == 0)
                appReturn.SetAsNotFound();
            else {
                entity.RemoverDadosSensiveis();
                appReturn.result = entity;
            }

            return appReturn;

        }
        

        public AppReturn ObterPeloToken(string token){

            if(Utils.Validator.Not(token)) {
                appReturn.SetAsBadRequest("Token não informado.");
                return appReturn;
            }

            Admin entity = DAO.ObterPeloToken(token);

            if(entity is null || entity?.id == 0)
                appReturn.SetAsNotFound("Admin não encontrado");
            else {
                entity.RemoverDadosSensiveis();
                appReturn.result = entity;
            }

            return appReturn;

        }
        



        public AppReturn ObterPeloDocumentoOuEmail(Admin entity){

            appReturn = BLO.ValidarDadosChaves(entity);

            if(!appReturn.status.success)
                return appReturn;

            entity = DAO.ObterPorDocumentoOuEmail(entity);

            if(entity is null || entity?.id == 0)
                appReturn.SetAsNotFound("Admin não encontrado");
            else {
                entity.RemoverDadosSensiveis();
                appReturn.result = entity;
            }

            return appReturn;

        }
        


        public AppReturn Obter(Admin entity){
            return ObterPeloDocumentoOuEmail(entity);
        }
        




        public AppReturn Inserir(Admin entity){

            appReturn = BLO.Validar(entity);

            if (!appReturn.status.success)
                return appReturn;

            entity = BLO.Normalizar(entity);

            Admin entityDB = DAO.ObterPorCamposChaves(entity);

            if(entityDB is not null && entityDB?.id > 0) {
                if(!entityDB.ativo)
                    appReturn.AddException("Já existe um Admin cadastrado com este CPF, CNPJ ou E-mail - CONSTA INATIVADO.");
                else
                    appReturn.AddException("Já existe um Admin cadastrado com este CPF, CNPJ ou E-mail.");
                return appReturn;
            }

            appReturn = DAO.Inserir(entity);

            if(appReturn.status.success) {
                Mail mail    = new Mail();
                mail.emailTo = entity.email;
                mail.about      = "Seja Bem " + (entity.sexo == "FEMININO" ? "Vinda" : "Vindo") + " ao Administrativo!";
                mail.message    = "Seja bem " + (entity.sexo == "FEMININO" ? "vinda" : "vindo") + " "+ entity.apelido + " ao <b style='color:#0072ff'>Sistema Administrativo da JáCaptei</b>.<br><br>O login já pode ser feito através do link abaixo:<br><a href='"+ Config.settings.baseURL +"'>"+ Config.settings.baseURL +"</a>";
                mail.Send();
            }

            return appReturn;
        }



        //public AppReturn Ativar(Admin entity) {

        //    if(entity is null || entity?.id == 0) {
        //        appReturn.AddException("Admin não identificado.");
        //        return appReturn;
        //     }

        //    appReturn = DAO.Ativar(entity);
            
        //    if(appReturn.status.success) {
        //        Mail mail = new Mail();
        //        mail.emailTo    = entity.email;
        //        mail.about      = "Seja Bem " + (entity.sexo == "FEMININO" ? "Vinda" : "Vindo") + "!";
        //        mail.message    = "Seja bem " + (entity.sexo == "FEMININO" ? "vinda" : "vindo") + " "+ entity.apelido + " à <b style='color:#ef5924'>Administração da JáCaptei</b>.<br><br>Seu acesso está liberado!";
        //        mail.Send();
        //    }
        //    return appReturn;

        //}
        

        public AppReturn AlterarDisponibilidade(Admin entity) {

            if(entity is null || entity?.id == 0) {
                appReturn.AddException("usuário não identificado.");
                return appReturn;
             }

            appReturn = DAO.AlterarDisponibilidade(entity);
            
            return appReturn;
        }



        

        public AppReturn AlterarSenha(Admin entity) {

            if(entity is null || Utils.Validator.Not(entity?.token)) {
                appReturn.AddException("Usuário não identificado.");
                return appReturn;
             }
            else if(Utils.Validator.Not(entity.senha)){
                appReturn.AddException("Senha não informada.");
                return appReturn;
             }

            entity.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());

            appReturn = DAO.AlterarSenha(entity);
            
            return appReturn;

        }

        
        public AppReturn Alterar(Admin entity) {

            if(entity is null || entity?.id == 0 || Utils.Validator.Not(entity?.token)) {
                appReturn.AddException("Usuário não identificado.");
                return appReturn;
            } else {

                    if(Utils.Validator.Is(entity.email))
                        entity.email = Utils.String.HigienizeMail(entity.email);
                    else
                        appReturn.AddException("EMAIL","E-MAIL não informado.");

                    if(Utils.Validator.Not(entity.nome))
                        appReturn.AddException("NOME","NOME não informado.");

                    if(entity.tipoPessoa == "PF") {
                        if(Utils.Validator.Not(entity.cpf))
                            appReturn.AddException("CPF","CPF não informado.");
                        else if(!Utils.Validator.IsCPF(entity.cpf))
                            appReturn.AddException("CPF","CPF inválido.");
                    } else if(entity.tipoPessoa == "PJ") {
                        if(Utils.Validator.Not(entity.cnpj))
                            appReturn.AddException("CNPJ","CNPJ não informado.");
                        else if(!Utils.Validator.IsCNPJ(entity.cnpj))
                            appReturn.AddException("CNPJ","CNPJ inválido.");
                     } else
                        appReturn.AddException("TIPO","Tipo de pessoa (PF ou PJ) não identificado.");

                      if(!appReturn.status.success)
                            return appReturn;

                    Admin entityDB = DAO.ObterPorDocumentoOuEmail(entity);
                    if(entityDB is not null) {
                        if(entityDB.token != entity.token) {
                            appReturn.AddException("Já existe um usuário cadastrado com este E-Mail");
                            return appReturn;
                        }
                    }
                
            }

            appReturn = DAO.Alterar(entity);
            
            return appReturn;

        }

        

        public AppReturn Desativar(Admin entity) {

            if(entity is null || entity?.id == 0) {
                appReturn.AddException("Admin não identificado.");
                return appReturn;
             }

            appReturn = DAO.Desativar(entity);
            
            return appReturn;

        }

        
        public AppReturn ObterPorDocumentoOuEmail(Admin entity) {

            entity = DAO.ObterPorDocumentoOuEmail(entity);

            if(entity is null || entity?.id == 0)
                appReturn.AddException("Nada encontrado.");
            else
                appReturn.result = entity;

            return appReturn;

        }




        
        public AppReturn Autenticar(Admin entity) {

            appReturn = BLO.ValidarDadosLogin(entity);

            if(!appReturn.status.success)
                return appReturn;

            appReturn.result = null;

            entity = DAO.Autenticar(entity);

            if(entity is null)
                appReturn.SetAsNotFound("Usuário não encontrado.");
            else {
                entity.RemoverDadosSensiveis();
                if(!entity.ativo)
                    appReturn.SetAsGone("Usuário não ativo.");
                else {
                    appReturn.result = entity;
                }
            }
            return appReturn;
        }




        public AppReturn ObterInativos() {

            List<Admin> entities = DAO.ObterInativos();

            appReturn.result = entities;

            return appReturn;

        }










        /*
                public AppReturn ObterViaCPF(Shared.Model.Admin entity){

                    return DAO.ObterViaToken(entity);
                }

                public AppReturn ObterViaToken(Shared.Model.Admin entity){
                    return DAO.ObterViaToken(entity);
                }

                public AppReturn ObterViaTokenUID(Shared.Model.Admin entity){
                    return DAO.ObterViaTokenUID(entity);
                }
        */



    }
}
