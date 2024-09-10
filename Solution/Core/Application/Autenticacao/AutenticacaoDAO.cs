using RepoDb;
using JaCaptei.Model;
using JaCaptei.Application.DAL;
using Npgsql;

namespace JaCaptei.Application.Autenticacao
{
    public class AutenticacaoDAO : DAOBase
    {
        public Parceiro GetParceiroByEmail(string email, string senha)
        {
            try
            {
                Parceiro entityDB = new Parceiro();
                using (var conn = new NpgsqlConnection(DB.CS))
                {
                    entityDB = conn.Query<Parceiro>(e => e.email == email && e.senha == senha).FirstOrDefault();
                    return entityDB;
                }
            }
            catch (NpgsqlException ex)
            {
                throw;
            }
        }

        public Parceiro GetParceiroByCpf(long cpfNum, string senha)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DB.CS))
                {
                    return conn.Query<Parceiro>(e => e.cpfNum == cpfNum && e.senha == senha).FirstOrDefault();
                }
            }
            catch (NpgsqlException ex)
            {
                throw;
            }
        }

        public Parceiro GetParceiroByCnpj(long cnpjNum, string senha)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DB.CS))
                {
                    return conn.Query<Parceiro>(e => e.cnpjNum == cnpjNum && e.senha == senha).FirstOrDefault();
                }
            }
            catch (NpgsqlException ex)
            {
                throw;
            }
        }
        public SessaoUsuario ObterSessaoAtivaById(int idParceiro)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DB.CS))
                {
                    conn.Open();
                    return conn.Query<SessaoUsuario>(e => e.idParceiro == idParceiro).LastOrDefault();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new ApplicationException("An error occurred while accessing the database. Please contact support if the problem persists.", ex);
            }
        }
        public SessaoUsuario ObterSessaoAtivaByToken(string token)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DB.CS))
                {
                    conn.Open();
                    return conn.Query<SessaoUsuario>(e => e.tokenJWT == token).LastOrDefault();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new ApplicationException("An error occurred while accessing the database. Please contact support if the problem persists.", ex);
            }
        }
        public SessaoUsuario ValidarToken(int id, string tokenJWT)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DB.CS))
                {
                    conn.Open();
                    return conn.Query<SessaoUsuario>(e => e.idParceiro == id && e.tokenJWT == tokenJWT && e.isRevoked == true).LastOrDefault();
                }
            }
            catch (NpgsqlException ex)
            {
                throw new ApplicationException("An error occurred while accessing the database. Please contact support if the problem persists.", ex);
            }
        }
        public SessaoUsuario SalvarSessao(SessaoUsuario sessaoUsuario)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DB.CS))
                {
                    conn.Open();
                    sessaoUsuario.tokenJWT = conn.Insert<SessaoUsuario, string>(sessaoUsuario);
                    return sessaoUsuario;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new ApplicationException("An error occurred while accessing the database. Please contact support if the problem persists.", ex);
            }
        }

        public SessaoUsuario RevogarToken(SessaoUsuario sessaoUsuario, SessaoUsuario novaSessaoUsuario)
        {
            using (var conn = new NpgsqlConnection(DB.CS))
            {
                try
                {
                    sessaoUsuario.isRevoked = true;
                    sessaoUsuario.revokedAt = DateTime.UtcNow;
                    sessaoUsuario.revokedByIp = novaSessaoUsuario.ipAddress;
                    sessaoUsuario.replacedBySession = novaSessaoUsuario.sessionId;

                    int affectedRows = conn.Update<SessaoUsuario>(sessaoUsuario, e => e.id == sessaoUsuario.id);
                    return affectedRows > 0 ? sessaoUsuario : null;
                }
                catch (NpgsqlException ex)
                {
                    throw new ApplicationException("An error occurred while accessing the database. Please contact support if the problem persists.", ex);
                }
            }
        }

        public async Task<bool> RevokeTokenAfterSignOutAsync(SessaoUsuario sessaoUsuario)
        {
            try
            {
                using (var conn = new NpgsqlConnection(DB.CS))
                {
                    sessaoUsuario.isRevoked = true;
                    sessaoUsuario.revokedAt = DateTime.UtcNow;

                    // Atualiza a sessão no banco
                    int affectedRows = await conn.UpdateAsync<SessaoUsuario>(sessaoUsuario, e => e.id == sessaoUsuario.id);

                    // Verifica se houve atualização
                    return affectedRows > 0;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new ApplicationException("An error occurred while accessing the database. Please contact support if the problem persists.", ex);
            }
        }
    }
}
