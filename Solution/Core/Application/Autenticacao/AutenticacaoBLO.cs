using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using System.Text.Json;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.BLL;
using JaCaptei.Model.Model;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace JaCaptei.Services
{





    public class AutenticacaoBLO : BLOBase
    {


        public AppReturn ValidarAutenticacaoParceiro(Parceiro entity){
            if (entity is null || string.IsNullOrWhiteSpace(entity?.username) || string.IsNullOrWhiteSpace(entity?.senha))
                appReturn.SetAsNotAcceptable("Necessário informar CPF ou E-mail e senha.");
            return appReturn;
        }


        public AppReturn ValidarDadosLogin(Parceiro entity){
            if (entity is null || string.IsNullOrWhiteSpace(entity?.username))
                appReturn.SetAsNotAcceptable("Necessário informar algum username (CPF ou E-Mail).");
            return appReturn;
        }


        public AppReturn ValidarAtualizarPerfil(Parceiro entity)
        {

            if (Utils.Validator.IsNotSet(entity.nome))
                appReturn.AddException("Nome não informado.");
            else if (entity.nome.Length > 38)
                appReturn.AddException("Nome inválido (excedeu o limite de carcteres).");

            if (Utils.Validator.IsNotSet(entity.email))
                appReturn.AddException("Necessário informar o E-Mail.");
            else if (!Utils.Validator.IsEmail(entity.email))
                appReturn.AddException("E-mail inválido.");
            else if (entity.email.Length > 80)
                appReturn.AddException("E-mail inválido (excedeu o limite de carcteres).");

            if (Utils.Validator.IsNotSet(entity.telefone))
                appReturn.AddException("Necessário informar o Telefone.");
            else if (entity.telefone.Length < 11)
                appReturn.AddException("Telefone inválido.");
            else if (entity.telefone.Length > 38)
                appReturn.AddException("Telefone inválido (excedeu o limite de carcteres).");

            if(Utils.Validator.IsSet(entity.senha)) {
                if(entity.senha.Length < 4)
                    appReturn.AddException("Senha deve ter pelo menos 4 dígitos.");
                else if(entity.senha.Length > 38)
                    appReturn.AddException("Senha inválida (excedeu o limite de carcteres).");
            }
            return appReturn;
        }




        public AppReturn Handle(Parceiro entity)
        {

            if (entity is null)
            {
                appReturn.SetException("Entidade não informada.");
                return appReturn;
            }

            appReturn.result = entity;


            if (Utils.Validator.IsNotSet(entity.nome))
                appReturn.AddException("Nome não informado.");
            else if (entity.nome.Length > 58)
                appReturn.AddException("Nome inválido (excedeu o limite de carcteres).");

            if (Utils.Validator.IsNotSet(entity.email))
                appReturn.AddException("Necessário informar o E-Mail.");
            else if (!Utils.Validator.IsEmail(entity.email))
                appReturn.AddException("E-mail inválido.");
            else if (entity.email.Length > 80)
                appReturn.AddException("E-mail inválido (excedeu o limite de carcteres).");

            if (Utils.Validator.IsNotSet(entity.telefone))
                appReturn.AddException("Necessário informar o Telefone.");
            else if (entity.telefone.Length < 11)
                appReturn.AddException("Telefone inválido.");
            else if (entity.telefone.Length > 38)
                appReturn.AddException("Telefone inválido (excedeu o limite de carcteres).");


            if (Utils.Validator.IsNotSet(entity.senha))
                appReturn.AddException("Senha não informada.");
            if (entity.senha.Length < 4)
                appReturn.AddException("Senha deve ter pelo menos 4 dígitos.");
            else if (entity.senha.Length > 38)
                appReturn.AddException("Senha inválida (excedeu o limite de carcteres).");


            if (Utils.Validator.IsNotSet(entity.cpf))
                appReturn.AddException("Necessário informar o CPF ou CNPJ.");
            else if (Utils.Validator.IsSet(entity.cpf))
            {
                if (!Utils.Validator.IsCPF(entity.cpf))
                    appReturn.AddException("CPF inválido.");
                else if (entity.cpf.Length > 18)
                    appReturn.AddException("CPF inválido (excedeu o limite de carcteres).");
            }
            else if (Utils.Validator.IsSet(entity.cpf))
            {
                if (!Utils.Validator.IsCNPJ(entity.cpf))
                    appReturn.AddException("CNPJ inválido.");
                else if (entity.cpf.Length > 22)
                    appReturn.AddException("CNPJ inválido (excedeu o limite de carcteres).");
            }



            /*
            if (Utils.Validator.IsNotSet(entity.city))
                appReturn.AddException("Cidade não informada.");
            else if (entity.city.Length > 38)
                appReturn.AddException("Cidade inválida (excedeu o limite de carcteres).");

            if (Utils.Validator.IsNotSet(entity.state))
                appReturn.AddException("Estado não informado.");
            else if (entity.state.Length > 38)
                appReturn.AddException("Estado inválido (excedeu o limite de carcteres).");

            if (Utils.Validator.IsNotSet(entity.cpfType))
                appReturn.AddException("Tipo de documento não informado.");

            if (Utils.Validator.IsNotSet(entity.cpf))
                appReturn.AddException("Documento não informado.");
            else if (entity.cpf.Length < 4)
                appReturn.AddException("Documento inválido.");
            */
            return appReturn;
        }





    }




}
