using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;
using Npgsql;
using RepoDb;

using ImovelImagem = JaCaptei.Model.Entities.ImovelImagem;

namespace JaCaptei.Application.Integracao;

public class ImoviewDAO: IDisposable {

    private readonly NpgsqlConnection _conn;
    public ImoviewDAO(NpgsqlConnection conn) 
    {
        _conn = conn;
    }

    public void Dispose()
    {
        _conn?.Close();
        _conn?.Dispose();
    }

    public async Task<IntegracaoImoview?> GetIntegracao(int IdCliente)
    {
        var res = await _conn.QueryAsync<IntegracaoImoview>(i => i.IdCliente == IdCliente);
        return res.FirstOrDefault();
    }


    public async Task<bool> SaveIntegracao(IntegracaoImoview integracao)
    {
        if(integracao.Id > 0)
        {
            var fields = Field.Parse<IntegracaoImoview>(e => new { e.Status, e.DataAtualizacao, e.Bairros });
            await _conn.UpdateAsync<IntegracaoImoview>(integracao, fields);
        }
        else
            await _conn.InsertAsync<IntegracaoImoview>(integracao);
        return true;
    }

    public async Task<List<IntegracaoBairroImoview>> GetIntegracaoBairros(int idIntegracao)
    {
        var res = await _conn.QueryAsync<IntegracaoBairroImoview>(i => i.IdIntegracao == idIntegracao);
        return res.ToList();
    }

    public async Task<bool> SaveIntegracaoBairro(IntegracaoBairroImoview bairroIntegrado)
    {
        if (bairroIntegrado.Id > 0)
        {
            var fields = Field.Parse<IntegracaoBairroImoview>(e => new { e.Status, e.DataAtualizacao });
            await _conn.UpdateAsync<IntegracaoBairroImoview>(bairroIntegrado, fields);
        }
        else
            await _conn.InsertAsync<IntegracaoBairroImoview>(bairroIntegrado);
        return true;
    }

    public async Task<List<ImportacaoBairroImoview>> GetImportacaoBairros(int idIntegracaoBairro)
    {
        var res = await _conn.QueryAsync<ImportacaoBairroImoview>(i => i.IdIntegracaoBairro == idIntegracaoBairro);
        return res.ToList();
    }

    public async Task<bool> SaveImportacaoBairro(ImportacaoBairroImoview importacaoBairro)
    {
        if (importacaoBairro.Id > 0)
        {
            var fields = Field.Parse<ImportacaoBairroImoview>(e => new { e.Status });
            await _conn.UpdateAsync<ImportacaoBairroImoview>(importacaoBairro, fields);
            return true;
        }
        await _conn.InsertAsync<ImportacaoBairroImoview>(importacaoBairro);
        return true;
    }

    public async Task<List<ImportacaoImovelImoview>> GetImportacaoImoveis(int idImportacaoBairro)
    {
        var res = await _conn.QueryAsync<ImportacaoImovelImoview>(i => i.IdImportacaoBairro == idImportacaoBairro);
        return res.ToList();
    }

    public async Task<List<ImovelEndereco>> GetImoveisBairro(int idBairro)
    {
        var res = await _conn.QueryAsync<ImovelEndereco>(i => i.idBairro == idBairro);
        return res.ToList();
    }

    public async Task<ImovelFullDTO?> GetFullImovel(int idImovel)
    {
        var imovel = await _conn.QueryAsync<Imovel>(i => i.id == idImovel);
        if (imovel == null) return null;
        if (!imovel.Any()) return null;

        var (area, valores, endereco, disposicao, crsInternas, crsExternas, lazer) =
             await _conn.QueryMultipleAsync<ImovelAreas,
             ImovelValores, ImovelEndereco, ImovelDisposicao,
             ImovelCaracteristicasInternas,
             ImovelCaracteristicasExternas, ImovelLazer>(
             a  => a.idImovel   == idImovel,  // area
             v  => v.idImovel   == idImovel,  // valor
             e  => e.idImovel   == idImovel,  // endereco
             d  => d.idImovel   == idImovel,  // dísposicao
             ci => ci.idImovel  == idImovel,  // crsInternas
             ce => ce.idImovel  == idImovel,  // crsInternas
             l  => l.idImovel   == idImovel   // lazer 
         );

        var res = new ImovelFullDTO()
        {
            Imovel = imovel.First(),
            ImovelAreas = area.First(),
            ImovelValores = valores.First(),
            ImovelEndereco = endereco.First(),
            ImovelDisposicao = disposicao.First(),
            ImovelCaracteristicasInternas = crsInternas.First(),
            ImovelCaracteristicasExternas = crsExternas.First(),
            ImovelLazer = lazer.First()
        };
        return res;
    }

    public async Task<List<IntegracaoBairroImoview>> GetIntegracaoBairroPendetes(int idIntegracao)
    {
        var integracoesPendentes = await _conn.QueryAsync<IntegracaoBairroImoview>
            (i => i.IdIntegracao == idIntegracao && i.Status != StatusIntegracao.Concluido.GetDescription());
        return integracoesPendentes.ToList();
    }

    public async Task<List<ImportacaoBairroImoview>> GetImportacaoBairrosPendentes(int idIntegracao)
    {
        var integracoesPendentes = await _conn.QueryAsync<IntegracaoBairroImoview>
            (i => i.IdIntegracao == idIntegracao);
        List<ImportacaoBairroImoview> importacoes = [];
        foreach(var integracao in  integracoesPendentes)
        {
            var res = await _conn.QueryAsync<ImportacaoBairroImoview>
                (i => i.IdIntegracaoBairro == integracao.Id && i.Status != StatusIntegracao.Concluido.GetDescription());
            importacoes.AddRange(res.ToList());
        }
        return importacoes;
    }

    public async Task<ImportacaoImovelImoview?> GetImportacaoImovel(int idImportacao, int idImovel)
    {
        var res = await _conn.QueryAsync<ImportacaoImovelImoview>(i => i.IdImportacaoBairro == idImportacao && i.IdImovel == idImovel);
        return res.FirstOrDefault();
    }

    public async Task<List<ImovelImagem>> ObterImagensImovel(int idImovel)
    {
        var list = await _conn.QueryAsync<ImovelImagem>(e => e.IdImovel == idImovel);
        return list.ToList();
    }

    internal async Task<bool> SaveImportacaoImovel(ImportacaoImovelImoview importacaoImovel)
    {
        if (importacaoImovel.Id > 0)
        {
            var fields = Field.Parse<ImportacaoImovelImoview>(e => new { e.Status, e.ImoviewResponse, e.DataAtualizacao });
            await _conn.UpdateAsync<ImportacaoImovelImoview>(importacaoImovel, fields);
        }
        else
            await _conn.InsertAsync<ImportacaoImovelImoview>(importacaoImovel);
        return true;
    }

    internal async Task<Plano?> ObterPlano(int idPlano)
    {
        var res = await _conn.QueryAsync<Plano>(i => i.id == idPlano);
        return res.FirstOrDefault();
    }

    internal async Task<Parceiro?> ObterCliente(int idCliente)
    {
        var res = await _conn.QueryAsync<Parceiro>(i => i.id == idCliente);
        return res.FirstOrDefault();
    }
}