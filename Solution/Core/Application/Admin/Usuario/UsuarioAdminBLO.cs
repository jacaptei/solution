using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RepoDb;
using System.Text.Json;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.BLL;
using JaCaptei.Model.Model;

namespace JaCaptei.Application {

    public class UsuarioAdminBLO:BLOBase {

        UsuarioAdminDAO DAO = new UsuarioAdminDAO();

        public AppReturn ValidarDadosChaves(Admin entity) {

            if(entity is null) {
                appReturn.SetException("Usuário não informado.");
                return appReturn;
            }

            if(Utils.Validator.IsEmail(entity.username))
                entity.email = entity.username = Utils.String.HigienizeMail(entity.username);
            else if(Utils.Validator.IsCPF(entity.username)) {
                entity.cpf      = entity.username;
                entity.cpfNum   = Utils.Number.ToLong(entity.cpf); 
            } else if(Utils.Validator.IsCNPJ(entity.username)) { 
                entity.cnpj     = entity.username;
                entity.cnpjNum = Utils.Number.ToLong(entity.cnpj); 
            }else
                entity.username = null;

            if(Utils.Validator.Not(entity.username))
                appReturn.SetAsNotAcceptable("Necessário informar CPF, CNPJ ou E-mail");

            if(appReturn.status.success)
                appReturn.result = entity;

            return appReturn;

        }




        public AppReturn ValidarDadosLogin(Admin entity) {

            appReturn = ValidarDadosChaves(entity);

            if(appReturn.status.success && Utils.Validator.Not(entity.senha))
                    appReturn.AddException("SENHA","Senha não informada.");

            return appReturn;

        }



        public AppReturn Validar(Admin entity) {


            if(entity is null) {
                appReturn.SetException("Usuário não informado.");
                return appReturn;
            }


            if(entity.tipoPessoa == "PF") {
                if(Utils.Validator.Not(entity.cpf)) {
                    appReturn.AddException("CPF","CPF não informado.");
                    return appReturn;
                }else if(!Utils.Validator.IsCPF(entity.cpf)) {
                    appReturn.AddException("CPF","CPF inválido.");
                    return appReturn;
                }
            }else if(entity.tipoPessoa == "PJ") {
                if(Utils.Validator.Not(entity.cnpj)) {
                    appReturn.AddException("CNPJ","CNPJ não informado.");
                    return appReturn;
                }else if(!Utils.Validator.IsCNPJ(entity.cnpj)) {
                    appReturn.AddException("CNPJ","CNPJ inválido.");
                    return appReturn;
                }
            }else
                appReturn.AddException("TIPO","Tipo de pessoa (PF ou PJ) não identificado.");


            if(Utils.Validator.Not(entity.email))
               appReturn.AddException("E-MAIL","E-mail não informado.");
            else if(!Utils.Validator.IsEmail(entity.email))
               appReturn.AddException("E-MAIL","E-mail inválido.");

            if(Utils.Validator.Not(entity.senha))
               appReturn.AddException("SENHA","Senha não informada.");

            if(Utils.Validator.Not(entity.nome))
               appReturn.AddException("NOME","NOME não informado.");



            return appReturn;

        }






        public Admin Normalizar(Admin entity) {

            if(entity is null)
                return entity;

                entity.idTipoUsuario    =  3;

                entity.nome             =  Utils.String.HigienizeToUpper(entity.nome);
                entity.apelido          =  Utils.String.Capitalize(entity.nome.Split(' ')[0]);
                entity.cpf              =  Utils.Format.CPF(entity.cpf);
                entity.cpfNum           =  Utils.Number.ToLong(entity.cpf);
                entity.cnpj             =  Utils.Format.CNPJ(entity.cnpj);
                entity.cnpjNum          =  Utils.Number.ToLong(entity.cnpj);
                entity.email            =  Utils.String.HigienizeMail(entity.email);

                if(entity.tipoPessoa == "PJ"){ 
                    entity.sexo             = "NA";
                    entity.cpf              = "";
                    entity.cpfNum           = 0;
                } else {
                    entity.cnpj             = "";
                    entity.cnpjNum          = 0;
                }
                
                entity.senha           =   Utils.Key.EncodeToBase64(entity.senha.ToLower());
                entity.sexo            =   entity.sexo.ToUpper();
                entity.token           =   Utils.Key.CreateToken();
                entity.tokenNum        =   Utils.Key.CreateTokenNum();
                entity.tokenUID        =   Utils.Key.CreateTokenUID();
                entity.loginCRM        =   "";
                entity.status          =   "ATIVO"; 
                entity.ativo           =   true; 
                entity.ativoCRM        =   false;
                entity.data            =   entity.dataAtualizacao = Utils.Date.GetLocalDateTime();

                entity.roles           = Utils.Validator.Is(entity.roles)? Utils.String.HigienizeToUpper(entity.roles) : entity.god? "ADMIN_GOD" : entity.gestor? "ADMIN_SUPER" : "ADMIN_DEFAULT"; // BASIC

                return entity;

        }



    }




}
