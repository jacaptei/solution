using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using System.Collections.Specialized;
using RepoDb.Enumerations;
using System.Runtime.Intrinsics.Arm;
using RepoDb.Extensions;
using JaCaptei.Model.Model;
using MailKit.Search;

namespace JaCaptei.Application.Autenticacao
{

    public class AutenticacaoDAO: DAOBase {

        /*
        public Usuario ObterUsuarioPeloEmail(string email) {
            Usuario entity = null;
            using(var conn = new DBcontext().GetConn())
                entity = conn.ExecuteQuery<Usuario>("SELECT id, email FROM [Usuario] WHERE email=@email",new { email }).FirstOrDefault();
            return entity;
        }



        public Usuario ObterDadosDoUsuario(Usuario usuario) {

            usuario.RemoverDadosSensiveis();
            return usuario;

        }



        public NameValueCollection ObterParametrosLogin(Usuario usuario) {
            var sqlParam = "email=@mail";
            string loginParam = "0";
            loginParam  = usuario.email;  
            sqlParam = "email=@mail" ; 
            NameValueCollection res = new NameValueCollection();
            res.Add("sqlParam"  , sqlParam  );
            res.Add("loginParam", loginParam);
            return res;
        }


        public AppReturn Autenticar(Usuario usuario) {

            NameValueCollection lparams = ObterParametrosLogin(usuario);
            var sqlParam = lparams["sqlParam"];
            var loginParam = lparams["loginParam"];
            if(string.IsNullOrWhiteSpace(loginParam))
                return appReturn;

            var param = new { login = loginParam, senha = Utils.Key.EncodeToBase64(usuario.senha) };
            var sql = "SELECT * FROM Usuario WHERE " + sqlParam + " AND senha=@senha AND bloqueado = 0 ";

            try { 
                using(var conn = DB.GetConn())
                    usuario = conn.ExecuteQuery<Usuario>(sql, param).FirstOrDefault();
            }catch(Exception e) {
                appReturn.SetAsServerException(e);
                System.Diagnostics.Debug.Write(e.ToString());
            }

            if(usuario?.id > 0)
                usuario = ObterDadosDoUsuario(usuario);

            appReturn.result = usuario;
            return appReturn;
        }

        public AppReturn ObterUsuarioPeloLogin(Usuario usuario) {

            NameValueCollection lparams = ObterParametrosLogin(usuario);
            var sqlParam = lparams["sqlParam"];
            var loginParam = lparams["loginParam"];

            if(string.IsNullOrWhiteSpace(loginParam))
                return appReturn;

            var param = new { login = loginParam };
            var sql = "SELECT * FROM Usuario WHERE " + sqlParam + " AND bloqueado = 0";

            using(var conn = DB.GetConn())
                usuario = conn.ExecuteQuery<Usuario>(sql, param).FirstOrDefault();
            if(usuario?.id > 0)
                usuario = ObterDadosDoUsuario(usuario);

            appReturn.result = usuario;
            return appReturn;
        }



        public AppReturn ObterUsuarioPeloToken(Usuario usuario) {

            var param = new { usuario.token };
            var sql = "SELECT * FROM Usuario WHERE @token=token AND bloqueado = 0";

            using(var conn = DB.GetConn())
                usuario = conn.ExecuteQuery<Usuario>(sql, param).FirstOrDefault();
            if(usuario?.id > 0)
                usuario = ObterDadosDoUsuario(usuario);

            appReturn.result = usuario;
            return appReturn;
        }




        public AppReturn VerificarSeUsuarioExiste(Usuario usuario) {

            appReturn.result = false;

            NameValueCollection lparams = ObterParametrosLogin(usuario);
            var sqlParam = lparams["sqlParam"];
            var loginParam = lparams["loginParam"];
            if(string.IsNullOrWhiteSpace(loginParam))
                return appReturn;

            var sql = "SELECT id FROM Usuario WHERE " + ((usuario.email is not null)? " email=@email OR ":" ") + sqlParam;
            var param = new { login = loginParam, email=usuario.email };

            using(var conn = new DBcontext().GetConn())
                appReturn.result = conn.ExecuteQuery<int>(sql, param).FirstOrDefault() > 0;

            return appReturn;

        }



        public AppReturn VerificarSeUsuarioAtivoExiste(Usuario usuario) {

            appReturn.result = false;

            NameValueCollection lparams = ObterParametrosLogin(usuario);
            var sqlParam = lparams["sqlParam"];
            var loginParam = lparams["loginParam"];
            if(string.IsNullOrWhiteSpace(loginParam))
                return appReturn;

            var sql = "SELECT id FROM Usuario WHERE (" + ((usuario.email is not null)? " email=@email OR ":" ") + sqlParam +")  AND ativo=1 " ;
            var param = new { login = loginParam, email=usuario.email };

            using(var conn = new DBcontext().GetConn())
                appReturn.result = conn.ExecuteQuery<int>(sql, param).FirstOrDefault() > 0;

            return appReturn;

        }




        public AppReturn VerificarSeUsuarioExistePeloEmail(string email) {
            using(var conn = new DBcontext().GetConn())
                appReturn.result = conn.Query<Usuario>(e => e.email == email).FirstOrDefault();
            return appReturn;
        }









        public AppReturn AtualizarPerfil(Usuario usuario) {

            if(usuario is null)
                return appReturn;

            NameValueCollection lparams = ObterParametrosLogin(usuario);
            var sqlParam = lparams["sqlParam"];
            var loginParam = lparams["loginParam"];
            if(string.IsNullOrWhiteSpace(loginParam))
                return appReturn;

            var param = new { login = loginParam, usuario.token };
            var sql = "SELECT * FROM Usuario WHERE " + sqlParam + " AND token=@token AND bloqueado = 0 ";

            using(var conn = new DBcontext().GetConn()) {
                Usuario usuarioBase = conn.ExecuteQuery<Usuario>(sql, param).FirstOrDefault();
                if(usuarioBase is not null && usuarioBase?.id > 0) {
                    usuarioBase.nome        = Utils.String.HigienizeToUpper(usuario.nome);
                    usuarioBase.email       = Utils.String.HigienizeMail(usuario.email);
                    usuarioBase.telefone    = usuario.telefone;
                    if(Utils.Validator.IsSet(usuario.senha))
                        usuarioBase.senha = Utils.Key.EncodeToBase64(usuario.senha);
                    usuarioBase.dataAtualizacao = Utils.Date.GetLocalDateTime();
                    conn.Update(usuarioBase);
                    usuarioBase.RemoverDadosSensiveis();
                    appReturn.result = usuarioBase;
                } else
                    appReturn.SetAsNotFound("Usuário não encontrado");
            }
            return appReturn;
        }



        public AppReturn RegistrarCodigoDeValidacao(Usuario usuario) {
            using(var conn = new DBcontext().GetConn()) {
                usuario = conn.ExecuteQuery<Usuario>("SELECT * FROM Usuario WHERE email=@email ", new { usuario.email }).FirstOrDefault();
                if(usuario is not null && usuario?.id > 0) {
                    usuario.tokenNum = Utils.Key.CreateValidationCode();
                    conn.Update(usuario);
                    usuario.RemoverDadosSensiveis();
                    appReturn.result = usuario;
                } else
                    appReturn.SetAsNotFound();
            }
            return appReturn;
        }


        public AppReturn AlterarSenha(Usuario usuario) {
            using(var conn = new DBcontext().GetConn()) {
                Usuario usuarioBase = conn.ExecuteQuery<Usuario>("SELECT * FROM Usuario WHERE email=@email AND tokenNum=@tokenNum AND token=@token ", new { usuario.email, usuario.tokenNum, usuario.token }).FirstOrDefault();
                if(usuarioBase is not null && usuarioBase?.id > 0) {
                    usuarioBase.token           = Utils.Key.CreateToken();
                    usuarioBase.tokenNum        = Utils.Key.CreateTokenNum();
                    usuarioBase.senha           = Utils.Key.EncodeToBase64(usuario.senha);
                    usuarioBase.dataAtualizacao = Utils.Date.GetLocalDateTime();
                    conn.Update(usuarioBase);
                    usuarioBase.RemoverDadosSensiveis();
                    appReturn.result = usuarioBase;
                } else
                    appReturn.SetAsNotFound();
            }
            return appReturn;
        }





*/




    }
}
