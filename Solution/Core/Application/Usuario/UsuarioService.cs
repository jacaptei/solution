using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.Services;
using System.ComponentModel.DataAnnotations;

namespace JaCaptei.Application
{

    public class UsuarioService : ServiceBase
    {

        UsuarioBLO BLO = new UsuarioBLO();
        UsuarioDAO DAO = new UsuarioDAO();


        public AppReturn Inserir(Usuario usuario){

            appReturn = BLO.Validar(usuario);

            if (!appReturn.status.success)
                return appReturn;

            if (Utils.Validator.IsEmail(usuario.email))
                usuario.email = usuario.email;

            usuario.nome = Utils.String.HigienizeToUpper(usuario.nome);
            usuario.email = Utils.String.Higienize(usuario.email.ToLower());
            usuario.senha = usuario.senha.ToLower();
            usuario.token = Utils.Key.CreateToken();
            usuario.tokenUID = Utils.Key.CreateTokenUID();
            //usuario.active = true;
            usuario.data = Utils.Date.GetLocalDateTime();

            return DAO.Inserir(usuario);

        }


        public AppReturn CheckIfEmailExists(string email) {
            
            email = Utils.String.HigienizeMail(email);

            if(Utils.Validator.Not(email))
                appReturn.AddException("Necessário informar o E-Mail.");
            else if(!Utils.Validator.IsEmail(email))
                appReturn.AddException("E-mail inválido.");
            else if(email.Length > 38)
                appReturn.AddException("E-mail inválido (excedeu o limite de carcteres).");
            else {
                Usuario entity = DAO.GetByEmail(email);
                if(entity is not null && entity?.id > 0)
                    appReturn.SetAsUnprocessable("Já existe um usuário cadastrado com este e-mail.");
            }
            return appReturn;
        }



        public AppReturn AtualizarSenha(Usuario usuario){
            return DAO.AtualizarSenha(usuario);
        }

        public Usuario? GetUsuarioByEmail(Usuario usuario){
            return DAO.GetUsuarioByEmail(usuario);
        }

        public AppReturn ObterViaCPF(Usuario usuario){

            return DAO.ObterViaToken(usuario);
        }

        public AppReturn ObterViaToken(Usuario usuario){
            return DAO.ObterViaToken(usuario);
        }

        public AppReturn ObterViaTokenUID(Usuario usuario){
            return DAO.ObterViaTokenUID(usuario);
        }

        //public static List<Study> StudyGetAll() {
        //    return UsuarioDAO.StudyGetAll();
        //}





    }
}
