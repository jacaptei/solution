using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using RepoDb;
using JaCaptei.Application;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using JaCaptei.Model.Model;

namespace JaCaptei.Application
{

    public class UsuarioDAO : DAOBase{


        public bool CheckIfEmailExists(string email) {
            var res=false;
            using(var conn = new DBcontext().GetConn()) {
                var item = conn.ExecuteQuery<Usuario>("SELECT TOP 1 email FROM Usuario WHERE email=@email",new { email }).FirstOrDefault();
                res = (item is not null);
            }
            return res;
        }

        public Usuario GetByEmail(string email) {
            Usuario entity = null;
            using(var conn = new DBcontext().GetConn())
                entity = conn.ExecuteQuery<Usuario>("SELECT id, email FROM Usuario WHERE email=@email",new { email }).FirstOrDefault();
            return entity;
        }

        public AppReturn Inserir(Usuario usuario){

            using (var conn = new DBcontext().GetConn()){
                usuario.id = conn.Insert<Usuario, int>(usuario);
            }
            appReturn.result = usuario;
            return appReturn;

        }



        public AppReturn AtualizarSenha(Usuario usuario){

            if (usuario == null || string.IsNullOrWhiteSpace(usuario?.token))
                return appReturn;

            Usuario usuarioDB = null;
            var param = new { usuario.token };

            using (var conn = new DBcontext().GetConn()){
                usuarioDB = conn.ExecuteQuery<Usuario>("SELECT * FROM Usuario WHERE tokenUID=@tokenUID", param).FirstOrDefault();
                if (usuarioDB is not null && usuarioDB?.id > 0){
                    usuarioDB.senha = usuario.senha;
                    usuarioDB.token = new KeyUtil().CreateToken();
                    conn.Update(usuarioDB);
                    usuarioDB.RemoverDadosSensiveis();
                    appReturn.result = usuario;
                }
            }
            return appReturn;
        }




        public Usuario? GetUsuarioByEmail(Usuario usuario){
            /*
            if (usuario == null)
                return usuario;

            var param = new { email = usuario.email.ToLower() };
            using (var conn = new DBcontext().GetConn())
                usuario = conn.ExecuteQuery<Usuario>("SELECT * FROM Usuario WHERE email=@email", param).FirstOrDefault();

            if (usuario is not null)
                usuario.senha = "";
            */
            return usuario;

        }


        public AppReturn ObterViaCPF(Usuario usuario){
            if (usuario == null)
                return appReturn;
            var param = new { val = usuario.cpf.ToString() };
            using (var conn = new DBcontext().GetConn())
            {
                usuario = conn.ExecuteQuery<Usuario>("SELECT * FROM Usuario WHERE cpfNum=@val", param).FirstOrDefault();
                usuario.RemoverDadosSensiveis();
            }
            appReturn.result = usuario;
            return appReturn;
        }


        public AppReturn ObterViaToken(Usuario usuario){
            if (usuario == null || string.IsNullOrWhiteSpace(usuario?.token))
                return appReturn;
            var param = new { usuario.token };
            using (var conn = new DBcontext().GetConn())
            {
                usuario = conn.ExecuteQuery<Usuario>("SELECT * FROM Usuario WHERE token=@token", param).FirstOrDefault();
                usuario.RemoverDadosSensiveis();
            }
            appReturn.result = usuario;
            return appReturn;
        }

        public AppReturn ObterViaTokenUID(Usuario usuario){
            if (usuario == null || string.IsNullOrWhiteSpace(usuario?.token))
                return appReturn;
            var param = new { usuario.tokenUID };
            using (var conn = new DBcontext().GetConn())
            {
                usuario = conn.ExecuteQuery<Usuario>("SELECT * FROM Usuario WHERE tokenUID=@tokenUID", param).FirstOrDefault();
                usuario.RemoverDadosSensiveis();
            }
            appReturn.result = usuario;
            return appReturn;
        }




    }
}
