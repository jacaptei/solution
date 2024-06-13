using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Npgsql;
using RepoDb;
using RepoDb.PostgreSql;
using RepoDb.PostgreSql.BulkOperations;

namespace JaCaptei.UI.Models {


    public class DAO : DAOBase{




        public Usuario InserirUsuario(Usuario entity) {
            using(var conn = DB.GetConn())
                entity.id = conn.Insert<Usuario,int>(entity);

            Mail mail    = new Mail();
            mail.emailTo = entity.email;
            mail.about   = "Confirmação de cadastro";
            mail.message = "Olá " + Utils.String.Capitalize(entity.nome.Split(' ')[0]) + ".<br><br>Clique (ou copie e cole no navegador) o link abaixo para confirmar seu cadastro:<br><a href='https://jacaptei.com.br/#/confirma?t=" + entity.token + "' target='_blank' style='color:#ef5924'>https://jacaptei.com.br/#/confirma?t=" + entity.token + "</a>";
            mail.Send();

            return entity;
        }

        public bool ConfirmarUsuario(string token) {
            bool res = false;
            try {
                using(var conn = DB.GetConn()) {
                    Usuario user = conn.Query<Usuario>(e => e.token == token).FirstOrDefault();
                    //user.idCRM              = entity.idCRM;
                    if(user is not null && user?.id > 0) {
                        user.confirmado      = true;
                        user.token           = Utils.Key.CreateToken();
                        user.dataAtualizacao = Utils.Date.GetLocalDateTime();
                        conn.Update<Usuario>(user);

                        //Mail mail = new Mail();
                        //mail.emailTo    = user.email;
                        //mail.about      = "Cadastro confirmado";
                        //mail.message    = "Olá " + Utils.String.Capitalize(user.nome.Split(' ')[0]) + ".<br><br>Clique no link abaixo para confirmar seu cadastro:<br><a href='https://jacaptei.com.br/#/confirmar/t="+user.token+"' target='_blank' style='color:#ef5924'>https://jacaptei.com.br/#/confirmar/t="+user.token+"</a>";
                        //mail.Send();

                        res = true;
                    }

                }
            }catch(Exception ex) {
                var msg = ex.ToString();
            }

            return res;
        }


        
        public bool AlterarSenhaUsuario(Usuario usuario) {
            bool res = false;
            try {
                using(var conn = DB.GetConn()) {
                    Usuario user = conn.Query<Usuario>(e => e.token == usuario.token).FirstOrDefault();
                    //user.idCRM              = entity.idCRM;
                    if(user is not null && user?.id > 0) {
                        user.senha           = Utils.Key.EncodeToBase64(usuario.senha.ToLower());
                        user.token           = Utils.Key.CreateToken();
                        user.dataAtualizacao = Utils.Date.GetLocalDateTime();
                        conn.Update<Usuario>(user);
                        res = true;
                    }

                }
            }catch(Exception ex) {
                var msg = ex.ToString();
            }

            return res;
        }

                
        public bool AlterarUsuario(Usuario usuario) {
            bool res = false;
            try {
                using(var conn = DB.GetConn()) {
                    Usuario user = conn.Query<Usuario>(e => e.token == usuario.token).FirstOrDefault();
                    //user.idCRM              = entity.idCRM;
                    if(user is not null && user?.id > 0) {
                        if(Utils.Validator.Is(usuario.senha))
                            user.senha      = Utils.Key.EncodeToBase64(usuario.senha.ToLower());
                        if(Utils.Validator.Is(usuario.email))
                            user.email       = usuario.email;
                        //user.token           = Utils.Key.CreateToken();
                        user.dataAtualizacao = Utils.Date.GetLocalDateTime();
                        conn.Update<Usuario>(user);
                        res = true;
                    }
                }
            }catch(Exception ex) {
                var msg = ex.ToString();
            }

            return res;
        }






        public bool AtivarUsuario(Usuario entity) {
            bool res = false;
            try { 
                using(var conn = DB.GetConn()) {

                    Usuario user = conn.Query<Usuario>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
                    //user.idCRM            = entity.idCRM;
                    user.usernameCRM        = entity.usernameCRM;
                    user.senhaCRM           = entity.senhaCRM;
                    user.ativo              = user.ativoCRM = entity.ativo;
                    user.dataAtualizacao    =  Utils.Date.GetLocalDateTime();
                    conn.Update<Usuario>(user);

                    Mail mail = new Mail();
                    mail.emailTo    = user.email;
                    mail.about      = "Seja Bem " + (user.sexo == "FEMININO" ? "Vinda" : "Vindo") + "!";
                    mail.message    = "Seja bem " + (user.sexo == "FEMININO" ? "vinda" : "vindo") + " "+ Utils.String.Capitalize(user.nome.Split(' ')[0]) + " à <b style='color:#ef5924'>JáCaptei</b>.<br><br>Seu acesso estará liberado logo após o pagamento da assinatura. Para isso, basta acessar o link abaixo e aproveitar todos os benefícios JáCaptei.<br><a href='https://www.asaas.com/c/a6w4nl67dwjhqhq7' target='_blank' style='color:#ef5924'>https://www.asaas.com/c/a6w4nl67dwjhqhq7</a>";
                    mail.Send();

                    res = true;

                }
            } catch(Exception e) {
                res = false;
            }

            return res;
        }




        public Usuario VerificarUsuario(Usuario entity) {
            using(var conn = DB.GetConn())
                entity = conn.Query<Usuario>(e => e.cpfNum == entity.cpfNum || e.email == entity.email).FirstOrDefault();
            return entity;
        }

        public Usuario LoginUsuario(Usuario entity) {
            Usuario user = null;
            using(var conn = DB.GetConn()) {
                    if(Utils.Validator.IsEmail(entity.username))
                        user = conn.Query<Usuario>(e => e.senha == entity.senha && e.email == entity.username).FirstOrDefault();
                    else
                        user = conn.Query<Usuario>(e => e.senha == entity.senha && e.cpfNum == Utils.Number.ToLong(entity.username)).FirstOrDefault();
                }
            return user;
        }
        
        public Usuario ObterUsuario(Usuario entity) {
            Usuario user = null;
            using(var conn = DB.GetConn()) {
                if(Utils.Validator.IsEmail(entity.username)) {
                    entity.email = Utils.String.HigienizeMail(entity.username);
                    user = conn.Query<Usuario>(e => e.email == entity.email).FirstOrDefault();
                } else
                    user = conn.Query<Usuario>(e => e.cpfNum == Utils.Number.ToLong(entity.username)).FirstOrDefault();
                }
            return user;
        }


       public List<Usuario> ObterUsuariosInativos() {
            List<Usuario> usuarios = null;
            using(var conn = DB.GetConn())
                usuarios = conn.Query<Usuario>(e => e.ativo == false).ToList();
            return usuarios;
        }




        public Usuario ObterUsuarioPeloCPF(Usuario entity) {
            Usuario user = null;
            using(var conn = DB.GetConn())
                user = conn.Query<Usuario>(e => e.cpfNum == entity.cpfNum).FirstOrDefault();
            return user;
        }
        public Usuario ObterUsuarioPeloEmail(Usuario entity) {
            Usuario user = null;
            using(var conn = DB.GetConn())
                user = conn.Query<Usuario>(e => e.email == entity.email).FirstOrDefault();
            return user;
        }




        public Favorito FavoritarImovel(Favorito entity) {
            using(var conn = DB.GetConn()) {
                if(entity.adicionar)
                    entity.id = conn.Insert<Favorito,int>(entity);
                else
                    conn.Delete<Favorito>(f=>f.idImovelCRM == entity.idImovelCRM);
            }
            return entity;
        }



        public List<Favorito> ObterImoveisFavoritos(int idUsuario) {
            List<Favorito> favs = new List<Favorito>();
            using(var conn = DB.GetConn())
                favs = conn.Query<Favorito>(e => e.idUsuario == idUsuario).ToList();
            return favs;
        }




        public Usuario AtualizarSenhaUsuario(Usuario entity) {
            var param = new { entity.token };
            Usuario user;
            using(var conn = new DBcontext().GetConn()) {
                user = conn.Query<Usuario>(e => e.token == entity.token).FirstOrDefault();
                if(user is not null && user?.id > 0) {
                    user.senha = entity.senha;
                    user.token = entity.token = Utils.Key.CreateToken(user.id);
                    conn.Update(user);
                    user.senha = "";
                }
            }
           return entity;
        }


        public bool Update(Usuario entity) {
            int affectedRows = 0;
            using(var conn = DB.GetConn())
                affectedRows = conn.Update<Usuario>(entity);
            return affectedRows > 0;
        }
        /*
        public bool Delete(int _id) {
            int affectedRows = 0;
            using(var conn = DB.GetConn())
                affectedRows = conn.Delete<Usuario>(new Usuario{ id = _id });
            return affectedRows > 0;
        }
        */

        public bool Delete(int _id) {
            int affectedRows = 0;
            var param = new { status = "REMOVIDO",id = _id, dtupdate = DateTime.Now };
            using(var conn = DB.GetConn())
                affectedRows = conn.ExecuteQuery("UPDATE \"Usuario\" SET \"idStatus\" = @status, \"dateUpdate\" = @dtupdate WHERE id = @id  RETURNING id ",param).FirstOrDefault();
            //affectedRows = conn.Delete<Client>(new Client { id = _id });
            return affectedRows > 0;
        }




        public Usuario Get(int id){

            Usuario? entity = null;

            using (var conn = DB.GetConn())
                entity = conn.Query<Usuario>(e => e.id == id).FirstOrDefault();
            
            return entity;
        }


        public bool HasUsuario(long _cpf,string _email) {
            bool hasUsuario = true;
            var param = new { cpf = _cpf,email = _email };
            using(var conn = DB.GetConn())
                hasUsuario = conn.ExecuteQuery<bool>("SELECT EXISTS(SELECT id FROM \"Usuario\" WHERE \"cpfNum\"=@cpf OR email=@email)",param).FirstOrDefault();
            return hasUsuario;
        }


        public List<Usuario> GetAll(){
            List<Usuario>? entities = null;
            using(var conn = DB.GetConn())
                entities = conn.ExecuteQuery<Usuario>("SELECT * FROM \"Usuario\" WHERE \"idStatus\" > 0 ORDER BY \"dateUpdate\" DESC, id DESC LIMIT 100 ").ToList();
            return entities;
        }


        public void Clear(){
                using(var conn = DB.GetConn())
                    conn.ExecuteQuery("TRUNCATE \"Usuario\" RESTART IDENTITY");
        }



        public Usuario Search(Usuario entity) {

            return entity;
        }



    }



}
