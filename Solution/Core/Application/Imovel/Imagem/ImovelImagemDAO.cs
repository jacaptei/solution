using JaCaptei.Model.Entities;

using Npgsql;

using RepoDb;

namespace JaCaptei.Application;

public class ImovelImagemDAO: IDisposable
{
    private readonly NpgsqlConnection _conn;

    public ImovelImagemDAO(NpgsqlConnection conn)
    {
        _conn = conn;
    }
    public async Task<bool> Create(ImovelImagem entity)
    {
        var affectedRows = (int) (await _conn.InsertAsync(entity));
        return affectedRows > 0;
    }

    public async Task<ImovelImagem?> GetById(int id)
    {
        return (await _conn.QueryAsync<ImovelImagem>(i => i.Id == id)).FirstOrDefault();
    }

    public async Task<bool> Update(ImovelImagem entity)
    {
        var affectedRows = await _conn.UpdateAsync(entity);
        return affectedRows > 0;
    }

    public async Task<bool> Delete(int id)
    {
        var affectedRows = await _conn.DeleteAsync<ImovelImagem>(i => i.Id == id); 
        return affectedRows > 0;
    }


    public async Task<List<ImovelImagem>> GetByImovelId(int idImovel)
    {
        var list = await _conn.QueryAsync<ImovelImagem>(e => e.IdImovel == idImovel);
        return list.ToList();
    }

    public void Dispose()
    {
        _conn?.Close();
        _conn?.Dispose();
    }
}
