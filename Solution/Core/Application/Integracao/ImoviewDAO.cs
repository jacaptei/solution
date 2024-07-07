using JaCaptei.Model.Entities;
using Npgsql;
using RepoDb;

namespace JaCaptei.Application.Integracao;

public class ImoviewDAO: IDisposable {
    private readonly NpgsqlConnection _conn;
    public ImoviewDAO(NpgsqlConnection conn) {
        _conn = conn;
    }

    public async Task<IntegracaoImoview?> GetIntegracao(int IdCliente)
    {
        var res = await _conn.QueryAsync<IntegracaoImoview>(i => i.IdCliente == IdCliente);
        return res.FirstOrDefault();
    }

    public void Dispose()
    {
        _conn?.Close();
        _conn?.Dispose();
    }

    public async Task<bool> SaveIntegracao(IntegracaoImoview integracao)
    {
        await _conn.InsertAsync(integracao);
        return true;
    }
}