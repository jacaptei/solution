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

    public class ProprietarioBLO:BLOBase {

        ProprietarioDAO DAO = new ProprietarioDAO();


        public AppReturn ValidarDadosChaves(Proprietario entity) {

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




        public AppReturn ValidarDadosLogin(Proprietario entity) {

            appReturn = ValidarDadosChaves(entity);

            if(appReturn.status.success && Utils.Validator.Not(entity.senha))
                    appReturn.AddException("SENHA","Senha não informada.");

            return appReturn;

        }



        public AppReturn Validar(Proprietario entity) {


            if(entity is null) {
                appReturn.SetException("Usuário não informado.");
                return appReturn;
            }

            if(Utils.Validator.Not(entity.nome))
                appReturn.AddException("NOME","NOME não informado.");

            //if(Utils.Validator.Not(entity.rg))
            //    appReturn.AddException("RG","RG não informado.");

            if(Utils.Validator.Not(entity.email))
               appReturn.AddException("E-MAIL","E-mail não informado.");
            else if(!Utils.Validator.IsEmail(entity.email))
               appReturn.AddException("E-MAIL","E-mail inválido.");

            if(Utils.Validator.Not(entity.telefone))
               appReturn.AddException("TELEFONE","Telefone não informado.");
            else if(entity.telefone.Length < 14)
               appReturn.AddException("TELEFONE","Telefone inválido.");

            if(Utils.Validator.Not(entity.estado))
               appReturn.AddException("ESTADO","Estado não selecionado.");

            if(Utils.Validator.Not(entity.cidade))
               appReturn.AddException("CIDADE","Cidade não selecionada.");

            if(Utils.Validator.Not(entity.bairro))
               appReturn.AddException("BAIRRO","Bairro não selecionado.");

            if(Utils.Validator.Not(entity.logradouro))
               appReturn.AddException("LOGRADOURO","Logradouro não informado.");

            if(Utils.Validator.Not(entity.numero))
               appReturn.AddException("NUMERO","Número não informado (digite 'SN' se não houver).");

            return appReturn;

        }






        public Proprietario Normalizar(Proprietario entity) {

            if(entity is null)
                return entity;

                entity.idTipoUsuario    =  5;
                entity.nome             =  Utils.String.HigienizeToUpper(entity.nome);
                entity.razao            =  Utils.String.HigienizeToUpper(entity.razao);
                entity.responsavel      =  "";
                entity.apelido          =  "";
                //entity.cpf              =  ""; //Utils.Format.CPF(entity.cpf);
                //entity.cpfNum           =  0;  //Utils.Number.ToLong(entity.cpf);
                //entity.cnpj             =  ""; //Utils.Format.CNPJ(entity.cnpj);
                //entity.cnpjNum          =  0;  //Utils.Number.ToLong(entity.cnpj);
                entity.rg               =  Utils.String.HigienizeToUpper(entity.rg);
                entity.cpf              =  Utils.Format.CPF(entity.cpf);
                entity.cpfNum           =  Utils.Number.ToLong(entity.cpf);
                entity.cnpj             =  ""; //Utils.Format.CNPJ(entity.cnpj);
                entity.cnpjNum          =  0;  //Utils.Number.ToLong(entity.cnpj);
                entity.email            =  Utils.String.HigienizeMail(entity.email);

                if(entity.tipoPessoa == "PJ"){ 
                    entity.apelido          = Utils.String.Capitalize(entity.razao.Split(' ')[0]);
                    entity.dataNascimento   = Utils.Date.GetUnsetDefaultDateTime();
                    entity.sexo             = "NA";
                    entity.cpf              = "";
                    entity.cpfNum           = 0;
                } else {
                    entity.apelido          = Utils.String.Capitalize(entity.nome.Split(' ')[0]);
                    entity.cnpj             = "";
                    entity.cnpjNum          = 0;
                    entity.dataNascimento   = Utils.Date.GetUnsetDefaultDateTime();
                }

                entity.cepNorm         =   Utils.String.HigienizeToUpper(entity.cep);
                entity.estado          =   Utils.String.HigienizeToUpper(entity.estado);
                entity.cidade          =   Utils.String.HigienizeToUpper(entity.cidade);
                entity.bairro          =   Utils.String.HigienizeToUpper(entity.bairro);
                entity.estadoNorm      =   Utils.String.NormalizeToUpper(entity.estado);
                entity.cidadeNorm      =   Utils.String.NormalizeToUpper(entity.cidade);
                entity.bairroNorm      =   Utils.String.NormalizeToUpper(entity.bairro);

                entity.logradouro      =   Utils.String.HigienizeToUpper(entity.logradouro);
                entity.numero          =   Utils.String.HigienizeToUpper(entity.numero);
                entity.complemento     =   Utils.String.HigienizeToUpper(entity.complemento);
                entity.logradouroNorm  =   Utils.String.NormalizeToUpper(entity.logradouro);
                
                entity.senha           =   Utils.Key.EncodeToBase64(Utils.Key.CreateDaykey().ToString());
                entity.sexo            =   entity.sexo.ToUpper();
                entity.tipo            =   entity.tipo.ToUpper();
                entity.token           =   Utils.Key.CreateToken();
                entity.tokenNum        =   Utils.Key.CreateTokenNum();
                entity.tokenUID        =   Utils.Key.CreateTokenUID();
                entity.loginCRM        =   "";
                entity.ativo           =   entity.ativoCRM = true;
                entity.data            =   entity.dataAtualizacao = Utils.Date.GetLocalDateTime();

            return entity;

        }

















    }




}
