using JaCaptei.Model.Entities;

using Npgsql;

using RepoDb;

namespace JaCaptei.Application.Integracao
{
    public class VistaSoftDAO: IDisposable
    {
        private readonly NpgsqlConnection _conn;
        public VistaSoftDAO(NpgsqlConnection conn)
        {
            _conn = conn;
        }

        public void Dispose()
        {
            _conn?.Close();
            _conn?.Dispose();
        }

        public async Task<IntegracaoVistaSoft?> GetIntegracao(int idCliente)
        {
            return (await _conn.QueryAsync<IntegracaoVistaSoft>(i => i.IdCliente == idCliente)).FirstOrDefault();
        }
    }
}
