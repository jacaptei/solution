using JaCaptei.Application.DAL;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using Npgsql;

using RepoDb;

namespace JaCaptei.Application;

public class ImovelDAO
{
    private readonly NpgsqlConnection _conn;

    public ImovelDAO(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<ImovelFullDTO?> GetFullImovel(int idImovel)
    {
        var imovel = await _conn.QueryAsync<Imovel>(i => i.Id == idImovel);
        if (imovel == null) return null;
        if (!imovel.Any()) return null;

        var (area, valores, endereco, disposicao, crsInternas, crsExternas, lazer) =
            await _conn.QueryMultipleAsync<ImovelAreas, ImovelValores, ImovelEndereco,
            ImovelDisposicao, ImovelCaracteristicasInternas, 
            ImovelCaracteristicasExternas, ImovelLazer>(
            a  => a.IdImovel  == idImovel, // area
            v  => v.IdImovel  == idImovel, // valor
            e  => e.IdImovel  == idImovel, // endereco
            d  => d.IdImovel  == idImovel, // dísposicao
            ci => ci.IdImovel == idImovel, // crsInternas
            ce => ce.IdImovel == idImovel, // crsInternas
            l  => l.IdImovel  == idImovel // lazer 
        );

        var res = new ImovelFullDTO()
        {
            Imovel                        = imovel.First(),
            ImovelAreas                   = area.First(),
            ImovelValores                 = valores.First(),
            ImovelEndereco                = endereco.First(),
            ImovelDisposicao              = disposicao.First(),
            ImovelCaracteristicasInternas = crsInternas.First(),
            ImovelCaracteristicasExternas = crsExternas.First(),
            ImovelLazer                   = lazer.First()
        };
        return res;
    }
}
