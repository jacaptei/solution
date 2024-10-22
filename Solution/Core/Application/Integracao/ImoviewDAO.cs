using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using Newtonsoft.Json;

using Npgsql;
using RepoDb;

using ImovelImagem = JaCaptei.Model.Entities.ImovelImagem;

namespace JaCaptei.Application.Integracao;

public class ImoviewDAO : IDisposable {

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

    public async Task<IntegracaoImoview?> GetIntegracaoById(int id)
    {
        var res = await _conn.QueryAsync<IntegracaoImoview>(i => i.Id == id);
        return res.FirstOrDefault();
    }

    public async Task<bool> SaveIntegracao(IntegracaoImoview integracao)
    {
        if (integracao.Id > 0)
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
        const string queryImoveisBairro = "SELECT ie.*  FROM \"Imovel\" i INNER JOIN \"ImovelEndereco\" ie ON i.id = ie.\"idImovel\" WHERE i.ativo = true AND ie.\"codImovel\" is not null AND ie.\"codImovel\" <> '' AND ie.\"idBairro\" = @idBairro";
        //var res = await _conn.QueryAsync<ImovelEndereco>(i => i.idBairro == idBairro);
        //var res = await _conn.QueryAsync<ImovelEndereco>(queryImoveisBairro, new { idBairro });
        var res = await _conn.ExecuteQueryAsync<ImovelEndereco>(queryImoveisBairro, new { idBairro });
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
             a => a.idImovel == idImovel,  // area
             v => v.idImovel == idImovel,  // valor
             e => e.idImovel == idImovel,  // endereco
             d => d.idImovel == idImovel,  // dísposicao
             ci => ci.idImovel == idImovel,  // crsInternas
             ce => ce.idImovel == idImovel,  // crsInternas
             l => l.idImovel == idImovel   // lazer 
         );

        var tipo = await _conn.QueryAsync<ImovelTipo>(t => t.id == imovel.First().idTipo);

        var res = new ImovelFullDTO()
        {
            Imovel = imovel.First(),
            ImovelAreas = area.First(),
            ImovelValores = valores.First(),
            ImovelEndereco = endereco.First(),
            ImovelDisposicao = disposicao.First(),
            ImovelCaracteristicasInternas = crsInternas.First(),
            ImovelCaracteristicasExternas = crsExternas.First(),
            ImovelLazer = lazer.First(),
            ImovelTipo = tipo.First()
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
        //var integracoesPendentes = await _conn.QueryAsync<IntegracaoBairroImoview>
        //    (i => i.IdIntegracao == idIntegracao);
        //List<ImportacaoBairroImoview> importacoes = [];
        //foreach(var integracao in  integracoesPendentes)
        //{
        //    var res = await _conn.QueryAsync<ImportacaoBairroImoview>
        //        (i => i.IdIntegracaoBairro == integracao.Id && i.Status != StatusIntegracao.Concluido.GetDescription());
        //    importacoes.AddRange(res.ToList());
        //}
        const string queryImportPendentes = "SELECT ibi.* FROM public.\"IntegracaoImoview\" i inner join \"IntegracaoBairroImoview\" ib on i.id = ib.\"idIntegracao\" inner join \"ImportacaoBairroImoview\" ibi on ibi.\"idIntegracaoBairro\" = ib.id where i.id = @idIntegracao and ibi.status <> @status;";
        var res = await _conn.ExecuteQueryAsync<ImportacaoBairroImoview>(queryImportPendentes, new { idIntegracao, status = StatusIntegracao.Concluido.GetDescription() });
        return res.ToList();
    }

    public async Task<ImportacaoImovelImoview?> GetImportacaoImovel(int idImportacao, string codImovel)
    {
        var res = await _conn.QueryAsync<ImportacaoImovelImoview>(i => i.IdImportacaoBairro == idImportacao && i.CodImovel == codImovel);
        return res.FirstOrDefault();
    }

    public async Task<ImportacaoImovelImoview?> GetImportacaoImovel(int idImportacao)
    {
        var res = await _conn.QueryAsync<ImportacaoImovelImoview>(i => i.Id == idImportacao);
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
            var fields = Field.Parse<ImportacaoImovelImoview>(e => new { e.Status, e.ImoviewResponse, e.DataAtualizacao, e.RequestBody, e.Imagens });
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

    internal async Task<List<ImportacaoImovelImoview>> GetImportacaoImovelPendentes(int id = 0, bool process = false)
    {
        if (id == 0)
        {
            var res = await _conn.QueryAsync<ImportacaoImovelImoview>(i => i.Status != StatusIntegracao.Concluido.GetDescription()
            && (process || i.Status != StatusIntegracao.Processando.GetDescription()));
            return res.ToList();
        }
        else
        {
            var query = process ? @"SELECT ii.*
FROM public.""ImportacaoImovelImoview"" ii
inner join ""ImportacaoBairroImoview"" ibi on ibi.id = ii.""idImportacaoBairro""
inner join ""IntegracaoBairroImoview"" ib on ib.id = ibi.""idIntegracaoBairro""
WHERE ii.status != 'Concluido' and ib.""idIntegracao"" = @idIntegracao" :
@"SELECT ii.*
FROM public.""ImportacaoImovelImoview"" ii
inner join ""ImportacaoBairroImoview"" ibi on ibi.id = ii.""idImportacaoBairro""
inner join ""IntegracaoBairroImoview"" ib on ib.id = ibi.""idIntegracaoBairro""
WHERE ii.status != 'Concluido' AND ii.status != 'Processando' and ib.""idIntegracao"" = @idIntegracao";

            var res = (await _conn.ExecuteQueryAsync<ImportacaoImovelImoview>(query, new { idIntegracao = id })).ToList();
            return res;
        }
    }

    internal async Task<IntegracaoImoview?> GetIntegracaoImportacaoBairro(int idImportBairro)
    {
        const string queryIntegracaoPorImportacaoId = "SELECT i.* FROM \"ImportacaoBairroImoview\" ibi inner join \"IntegracaoBairroImoview\" ib on ibi.\"idIntegracaoBairro\" = ib.id inner join \"IntegracaoImoview\" i on ib.\"idIntegracao\" = i.id where ibi.id = @idImportBairro";
        //var importacaoBairro = (await _conn.QueryAsync<ImportacaoBairroImoview>(i => i.Id == idImportBairro)).FirstOrDefault();
        //if (importacaoBairro == null) return default;
        //var integracaoBairro = (await _conn.QueryAsync<IntegracaoBairroImoview>(i => i.Id == importacaoBairro.IdIntegracaoBairro)).FirstOrDefault();
        //if (integracaoBairro == null) return default;
        //var integracao = (await _conn.QueryAsync<IntegracaoImoview>(i => i.Id == integracaoBairro.IdIntegracao)).FirstOrDefault();
        var integracao = (await _conn.ExecuteQueryAsync<IntegracaoImoview>(queryIntegracaoPorImportacaoId, new { idImportBairro })).FirstOrDefault();
        return integracao;
    }

    internal async Task<List<IntegracaoImoview>> GetIntegracaoes()
    {
        var res = await _conn.QueryAsync<IntegracaoImoview>(i => i.Status != StatusIntegracao.Processando.GetDescription());
        return res.ToList();
    }

    internal async Task<List<IntegracaoComboDTO>> GetIntegracoes()
    {
        const string queryIntegracoesCombo = "SELECT i.id as \"Integracao\", p.nome as \"Cliente\"  FROM \"IntegracaoImoview\" i INNER JOIN \"Parceiro\"   p ON p.id = i.\"idCliente\"  ";
        var res = await _conn.ExecuteQueryAsync<IntegracaoComboDTO>(queryIntegracoesCombo);
        return res.ToList();
    }

    internal async Task<IntegracaoReport?> GetReportIntegracao(int idIntegracao)
    {
        const string queryReport = @"SELECT 
	jsonb_build_object(
    'integracao', i.id, 
    'cliente', p.nome, 
    'criadoEm', i.""dataInclusao"", 
    'plano', pl.nome, 
    'status', i.status, 
    'atualizadoEm', i.""dataAtualizacao"",
    'cpfCnpj', COALESCE(p.cnpj, p.cpf),
    'bairros', (SELECT jsonb_agg(
        jsonb_build_object(
            'bairro', jsonb_build_object('nome', ib.bairro ->> 'Nome', 'idCidade', ib.bairro ->> 'IdCidade'
	,'imoveis' , (SELECT jsonb_agg(jsonb_build_object(
	'id', ii.id,
	'cod', ii.""codImovel"",
	'data', ii.""dataInclusao"",
	'atualizadoEm', ii.""dataAtualizacao"",
	'status', ii.status,
	'imoviewResponse', ii.""imoviewResponse""
	) )
						FROM ""ImportacaoBairroImoview"" ibi inner join ""ImportacaoImovelImoview"" ii on ii.""idImportacaoBairro"" = ibi.id
                        WHERE ibi.""idIntegracaoBairro"" = ib.""id"")      
        ))
    ) 
    FROM ""IntegracaoBairroImoview"" ib 
    WHERE ib.""idIntegracao"" = i.id))
FROM 
    public.""IntegracaoImoview"" i 
    INNER JOIN ""Parceiro"" p ON p.id = i.""idCliente"" 
    INNER JOIN ""Plano"" pl ON pl.id = i.""idPlano""
WHERE i.id = @idIntegracao;";
        var reportStr = (await _conn.ExecuteQueryAsync<string>(queryReport, new { idIntegracao })).FirstOrDefault();
        var report= JsonConvert.DeserializeObject<IntegracaoReport>(reportStr);
        return report;
    }

    internal async Task<List<EmailImoveisInativadosImoview>> GetImoveisInativados()
    {
        const string queryImoveisInativados = @"SELECT 
    jsonb_build_object(
        'IdIntegracao', i.id, 
        'Cliente', jsonb_build_object('Id', p.id, 'Nome', p.nome, 'CpfCnpj', p.cnpj, 'Email', p.email),
        'DataEnvio', CURRENT_DATE,
        'Imoveis', (
            SELECT jsonb_agg(jsonb_build_object(
                'Id', ii.id,
                'CodJacaptei', ii.""codImovel"",
                'CodImoview', ii.""imoviewResponse"" ->> 'codigo',
                'DataInclusao', ii.""dataInclusao"",
                'Descricao', ii.""requestBody"" ->> 'descricao',
                'Endereco', ii.""requestBody"" -> 'endereco'
            ))
            FROM ""IntegracaoBairroImoview"" ib
            INNER JOIN ""ImportacaoBairroImoview"" ibi ON ibi.""idIntegracaoBairro"" = ib.""id""
            INNER JOIN ""ImportacaoImovelImoview"" ii ON ii.""idImportacaoBairro"" = ibi.id
            INNER JOIN ""Imovel"" im ON ii.""codImovel"" = im.cod
            LEFT JOIN ""ImovelInativoEmailImoview"" iie ON ii.id = iie.""idImportacaoImoview""
            WHERE ib.""idIntegracao"" = i.id AND ii.status = 'Concluido' AND im.ativo = false AND iie.id IS NULL
        )
    )
FROM 
    public.""IntegracaoImoview"" i 
    INNER JOIN ""Parceiro"" p ON p.id = i.""idCliente"" 
    INNER JOIN ""Plano"" pl ON pl.id = i.""idPlano""";
        var res = await _conn.ExecuteQueryAsync<string>(queryImoveisInativados);
        var listD = res.ToList();
        List<EmailImoveisInativadosImoview> list = [];
        foreach (var item in listD)
        {
            var obj = JsonConvert.DeserializeObject<EmailImoveisInativadosImoview>(item.ToString());
            list.Add(obj);
        }
        return list;
    }

    internal async Task<int> SaveEmailImovelInativo(EmailInativosIntegracaoImoview email)
    {
        var id = await _conn.InsertAsync<EmailInativosIntegracaoImoview, int>(email);
        return id;
    }

    internal async Task<bool> SaveImovelInativoEmail(ImovelInativoEmailImoview imovel)
    {
        await _conn.InsertAsync<ImovelInativoEmailImoview>(imovel);
        return true;
    }

    internal async Task<List<EmailInativosIntegracaoImoview>> GetEmailsInativos()
    {
        var res = await _conn.QueryAsync<EmailInativosIntegracaoImoview>(i => i.Status == "Concluido");
        return res.ToList();
    }

    internal async Task<int> GetIdImportacao(string cod, int idIntegracao)
    {
        const string queryIdImport = @"select ii.id from ""ImportacaoImovelImoview"" ii 
INNER JOIN ""ImportacaoBairroImoview"" ibi ON ii.""idImportacaoBairro"" = ibi.""id""
INNER JOIN ""IntegracaoBairroImoview"" ib ON ibi.""idIntegracaoBairro"" = ib.""id""
INNER JOIN ""IntegracaoImoview"" i ON ib.""idIntegracao"" = i.id
where ii.""codImovel"" = @cod and i.id = @idIntegracao";
        var res = await _conn.ExecuteQueryAsync<int>(queryIdImport, new { cod, idIntegracao });
        return res.FirstOrDefault();
    }

    internal async Task UpdateIntegracao(IntegracaoImoview integracao)
    {
        var fields = Field.Parse<IntegracaoImoview>(e => new { e.CodUsuario, e.CodUnidade, e.ChaveApi });
        await _conn.UpdateAsync<IntegracaoImoview>(integracao, fields);
    }
}