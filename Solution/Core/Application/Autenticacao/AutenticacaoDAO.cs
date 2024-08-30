using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using Npgsql;

namespace JaCaptei.Application.Autenticacao
{

    public class AutenticacaoDAO : DAOBase
    {
        public AppReturn Autenticar(Parceiro entity)
        {
            Parceiro entityDB = new Parceiro();
            entity.senha = Utils.Key.EncodeToBase64(entity.senha.ToLower());

            using (var conn = new NpgsqlConnection(DB.CS))
            {
                try
                {
                    if (Utils.Validator.IsEmail(entity.username))
                    {
                        entity.email = Utils.String.HigienizeMail(entity.username);
                        entityDB = conn.Query<Parceiro>(e => e.email == entity.email && e.senha == entity.senha).FirstOrDefault();
                    }
                    else if (Utils.Validator.IsCPF(entity.cpf))
                    {
                        entity.cpfNum = Utils.Number.ToLong(entity.username);
                        entityDB = conn.Query<Parceiro>(e => e.cpfNum == entity.cpfNum && e.senha == entity.senha).FirstOrDefault();
                    }
                    else
                    {
                        entity.cnpjNum = Utils.Number.ToLong(entity.username);
                        entityDB = conn.Query<Parceiro>(e => e.cnpjNum == entity.cnpjNum && e.senha == entity.senha).FirstOrDefault();
                    }
                }
                catch (Exception e)
                {
                    appReturn.AddException(e.ToString());
                }
            }
            appReturn.result = entityDB;
            return appReturn;
        }

        public SessaoUsuario ObterSessaoAtivaById(int id, string tokenJWT)
        {
            SessaoUsuario entity = new SessaoUsuario();
            using (var conn = new NpgsqlConnection(DB.CS))
                entity = conn.Query<SessaoUsuario>(e => e.idParceiro == id).LastOrDefault();
            return entity;
        }
        public SessaoUsuario ValidarToken(int id, string tokenJWT)
        {
            SessaoUsuario entity = new SessaoUsuario();
            using (var conn = new NpgsqlConnection(DB.CS))
                entity = conn.Query<SessaoUsuario>(e => e.idParceiro == id && e.tokenJWT == tokenJWT && e.isRevoked == true).LastOrDefault();
            return entity;
        }

        public SessaoUsuario SalvarSessao(SessaoUsuario sessaoUsuario)
        {
            SessaoUsuario entity = new SessaoUsuario();
            using (var conn = new NpgsqlConnection(DB.CS))
                entity.tokenJWT = conn.Insert<SessaoUsuario, string>(sessaoUsuario);
            return entity;
        }

        public SessaoUsuario RevogarToken(int id, string tokenJWT, SessaoUsuario novaSessaoUsuario)
        {
            using (var conn = new NpgsqlConnection(DB.CS))
            {
                SessaoUsuario entity = conn.Query<SessaoUsuario>(e => e.idParceiro == id).LastOrDefault();
                if (entity != null && entity.id > 0)
                {
                    entity.isRevoked = true;
                    entity.revokedAt = DateTime.UtcNow;
                    entity.revokedByIp = novaSessaoUsuario.ipAddress;
                    entity.replacedBySession = novaSessaoUsuario.sessionId;
                    int affectedRows = conn.Update<SessaoUsuario>(entity, e => e.id == entity.id);
                    if (affectedRows > 0)
                    {
                        return entity;
                    }
                }
            return entity;
            }
        }

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
