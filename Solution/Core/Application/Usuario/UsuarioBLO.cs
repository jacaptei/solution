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


    public class UsuarioBLO:BLOBase {

        UsuarioDAO userDAO = new UsuarioDAO();

        public Usuario Normalizar(Usuario entity) {

            if(entity is null)
                return entity;

            entity.nome     = Utils.String.HigienizeToUpper(entity.nome);
            entity.email    = Utils.String.HigienizeMail(entity.email);

            entity.pais     = Utils.String.HigienizeToUpper(entity.pais);
            entity.estado   = Utils.String.HigienizeToUpper(entity.estado);
            entity.cidade   = Utils.String.HigienizeToUpper(entity.cidade);

            return entity;

        }


        public AppReturn Validar(Usuario entity) {

            if(entity is null) {
                appReturn.SetException("Entidade não informada.");
                return appReturn;
            }


            //entity = Normalize(entity);
            appReturn.result = entity;

            if(Utils.Validator.Not(entity.nome))
                appReturn.AddException("name","Necessário informar o Nome.");
            else if(entity.nome.Length > 38)
                appReturn.AddException("name","Nome inválido (excedeu o limite de carcteres).");

            if(Utils.Validator.Is(entity.email) && !Utils.Validator.IsEmail(entity.email))
                appReturn.AddException("email","E-mail inválido.");

            if(appReturn.status.success) {
                //Usuario c = userDAO.GetByEmail(entity.documentNum);
                Usuario c = null;
                if(c is not null && c?.id > 0) {
                    //appReturn.result = c;
                    //appReturn.SetAsUnprocessable("Já existe um usere cadastrado com este documento.");
                    //appReturn.AddNote("document","Usuarioe já cadastrado: #ID: " + c.id.ToString() + ", NOME: " + c.nome + ", " + ((c.documentType == "OTHER") ? "OUTRO DOC: " : (c.documentType + ": ")) + c.document);
                    appReturn.result = c;
                    appReturn.SetAsUnprocessable("Já existe um usere cadastrado com este documento.");
                    Note nt = new Note();
                    nt.key = "document";
                    nt.info = "Usuarioe já cadastrado:";
                    nt.complement = "#ID: " + c.id.ToString() + ", NOME: " + c.nome + ", CPF: " + c.cpf;
                    appReturn.AddNote(nt);
                }
            }

            //if (Utils.Validator.Not(entity.pais))
            //    appReturn.AddException("country","País não informado.");
            //else if(Utils.Validator.Not(entity.estado)){
            //    if(entity.pais == "BRASIL")
            //        appReturn.AddException("state","Estado não informado.");
            //    else
            //        appReturn.AddException("state","Província não informada.");
            //}


            //if (Utils.Validator.IsNotSet(entity.password))
            //    appReturn.AddException("password","Senha não informada.");
            //if (entity.password.Length < 4)
            //    appReturn.AddException("password","Senha deve ter pelo menos 4 dígitos.");
            //else if (entity.password.Length > 38)
            //    appReturn.AddException("password","Senha inválida (excedeu o limite de carcteres).");


            //if (Utils.Validator.IsNotSet(entity.email))
            //    appReturn.AddException("email","Necessário informar o E-Mail.");
            //else if (!Utils.Validator.IsEmail(entity.email))
            //    appReturn.AddException("email","E-mail inválido.");
            //else if (entity.email.Length > 38)
            //    appReturn.AddException("email","E-mail inválido (excedeu o limite de carcteres).");

            return appReturn;
        }





    }




}
