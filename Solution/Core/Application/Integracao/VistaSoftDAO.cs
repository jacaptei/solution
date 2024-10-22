using JaCaptei.Model;
using JaCaptei.Model.DTO;
using JaCaptei.Model.Entities;

using Newtonsoft.Json;

using Npgsql;

using RepoDb;

using System;
using System.Diagnostics;

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

        public async Task<ImovelFullDTO?> GetFullImovel(int idImovel)
        {
            var imovel = await _conn.QueryAsync<Imovel>(i => i.id == idImovel);
            if (imovel == null) return null;
            if (!imovel.Any()) return null;
            var imovelFirst = imovel.First();   
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
                 l => l.idImovel == idImovel // lazer 
             );

            var tipo = await _conn.QueryAsync<ImovelTipo>(t => t.id == imovelFirst.idTipo);
            var fotos = await _conn.QueryAsync<Model.ImovelImagem>(f => f.idImovel == idImovel);

            var res = new ImovelFullDTO()
            {
                Imovel = imovelFirst,
                ImovelAreas = area.First(),
                ImovelValores = valores.First(),
                ImovelEndereco = endereco.First(),
                ImovelDisposicao = disposicao.First(),
                ImovelCaracteristicasInternas = crsInternas.First(),
                ImovelCaracteristicasExternas = crsExternas.First(),
                ImovelLazer = lazer.First(),
                ImovelTipo = tipo.First(),
                Fotos = fotos.ToList()
            };
            return res;
        }

        internal async Task<List<ImovelEndereco>> GetImoveisBairro(int idBairro)
        {
            const string queryImoveisBairro = "SELECT ie.*  FROM \"Imovel\" i INNER JOIN \"ImovelEndereco\" ie ON i.id = ie.\"idImovel\" WHERE i.ativo = true AND ie.\"codImovel\" is not null AND ie.\"codImovel\" <> '' AND ie.\"idBairro\" = @idBairro";
            var res = await _conn.ExecuteQueryAsync<ImovelEndereco>(queryImoveisBairro, new { idBairro });
            return res.ToList();
        }

        internal async Task<List<ImportacaoBairroVistaSoft>> GetImportacaoBairros(int id)
        {
            var res = await _conn.QueryAsync<ImportacaoBairroVistaSoft>(i => i.IdIntegracaoBairro == id);
            return res.ToList();
        }

        internal async Task<List<ImportacaoBairroVistaSoft>> GetImportacaoBairrosPendentes(int idIntegracao)
        {
            const string queryImportPendentes = "SELECT ibi.* FROM public.\"IntegracaoVistaSoft\" i inner join \"IntegracaoBairroVistaSoft\" ib on i.id = ib.\"idIntegracao\" inner join \"ImportacaoBairroVistaSoft\" ibi on ibi.\"idIntegracaoBairro\" = ib.id where i.id = @idIntegracao and ibi.status <> @status;";
            var res = await _conn.ExecuteQueryAsync<ImportacaoBairroVistaSoft>(queryImportPendentes, new { idIntegracao, status = StatusIntegracao.Concluido.GetDescription() });
            return res.ToList();
        }

        internal async Task<List<ImportacaoImovelImoview>> GetImportacaoImoveis(int id)
        {
            var res = await _conn.QueryAsync<ImportacaoImovelImoview>(i => i.IdImportacaoBairro == id);
            return res.ToList();
        }

        internal async Task<ImportacaoImovelVistaSoft?> GetImportacaoImovel(int idImportacao, string codImovel)
        {
            var res = await _conn.QueryAsync<ImportacaoImovelVistaSoft>(i => i.IdImportacaoBairro == idImportacao && i.CodImovel == codImovel);
            return res.FirstOrDefault();
        }

        internal async Task<ImportacaoImovelVistaSoft?> GetImportacaoImovel(int idImportacao)
        {
            var res = await _conn.QueryAsync<ImportacaoImovelVistaSoft>(i => i.Id == idImportacao);
            return res.FirstOrDefault();
        }

        internal async Task<List<IntegracaoBairroVistaSoft>> GetIntegracaoBairroPendetes(int idIntegracao)
        {
            var integracoesPendentes = await _conn.QueryAsync<IntegracaoBairroVistaSoft>
            (i => i.IdIntegracao == idIntegracao && i.Status != StatusIntegracao.Concluido.GetDescription());
            return integracoesPendentes.ToList();
        }

        internal async Task<List<IntegracaoBairroVistaSoft>> GetIntegracaoBairros(int id)
        {
            var res = await _conn.QueryAsync<IntegracaoBairroVistaSoft>(i => i.IdIntegracao == id);
            return res.ToList();
        }

        internal async Task<Parceiro?> ObterCliente(int idCliente)
        {
            return (await _conn.QueryAsync<Parceiro>(i => i.id == idCliente)).FirstOrDefault();
        }

        internal async Task<Plano?> ObterPlano(int idPlano)
        {
            return (await _conn.QueryAsync<Plano>(i => i.id == idPlano)).FirstOrDefault();
        }

        internal async Task<bool> SaveImportacaoBairro(ImportacaoBairroVistaSoft importacaoBairro)
        {
            if (importacaoBairro.Id > 0)
            {
                var fields = Field.Parse<ImportacaoBairroVistaSoft>(e => new { e.Status });
                await _conn.UpdateAsync<ImportacaoBairroVistaSoft>(importacaoBairro, fields);
                return true;
            }
            await _conn.InsertAsync<ImportacaoBairroVistaSoft>(importacaoBairro);
            return true;
        }

        internal async Task<bool> UpdateImportacaoImovel(ImportacaoImovelVistaSoft importacaoImovel)
        {
            if (importacaoImovel.Id > 0)
            {
                var fields = Field.Parse<ImportacaoImovelVistaSoft>(e => new { e.Status, e.ApiResponse, e.RequestBody });
                await _conn.UpdateAsync<ImportacaoImovelVistaSoft>(importacaoImovel, fields);
            }
            return true;
        }

        internal async Task<bool> UpdateDateImportacaoImovel(ImportacaoImovelVistaSoft importacaoImovel)
        {
            if (importacaoImovel.Id > 0)
            {
                var fields = Field.Parse<ImportacaoImovelVistaSoft>(e => new { e.DataAtualizacao });
                await _conn.UpdateAsync<ImportacaoImovelVistaSoft>(importacaoImovel, fields);
            }
            return true;
        }

        internal async Task<bool> SaveImportacaoImovel(ImportacaoImovelVistaSoft importacaoImovel)
        {
            if (importacaoImovel.Id > 0)
            {
                var fields = Field.Parse<ImportacaoImovelVistaSoft>(e => new { e.Status, e.ApiResponse, e.DataAtualizacao, e.RequestBody });
                await _conn.UpdateAsync<ImportacaoImovelVistaSoft>(importacaoImovel, fields);
            }
            else
                await _conn.InsertAsync<ImportacaoImovelVistaSoft>(importacaoImovel);
            return true;
        }

        internal async Task<bool> SaveIntegracao(IntegracaoVistaSoft integracao)
        {
            if (integracao.Id > 0)
            {
                var fields = Field.Parse<IntegracaoVistaSoft>(e => new { e.Status, e.DataAtualizacao, e.Bairros, e.UrlApi, e.ChaveApi });
                await _conn.UpdateAsync<IntegracaoVistaSoft>(integracao, fields);
            }
            else
                await _conn.InsertAsync<IntegracaoVistaSoft>(integracao);
            return true;
        }

        internal async Task<bool> SaveIntegracaoBairro(IntegracaoBairroVistaSoft bairroIntegrado)
        {
            if (bairroIntegrado.Id > 0)
            {
                var fields = Field.Parse<IntegracaoBairroVistaSoft>(e => new { e.Status, e.DataAtualizacao });
                await _conn.UpdateAsync<IntegracaoBairroVistaSoft>(bairroIntegrado, fields);
            }
            else
                await _conn.InsertAsync<IntegracaoBairroVistaSoft>(bairroIntegrado);
            return true;
        }

        internal async Task<List<ImportacaoImovelVistaSoft>> GetImportacaoImovelPendentes(int id = 0)
        {
            var process = false;
            if (id == 0)
            {
                var res = await _conn.QueryAsync<ImportacaoImovelVistaSoft>(i => i.Status != StatusIntegracao.Concluido.GetDescription()
                && (process || i.Status != StatusIntegracao.Processando.GetDescription()));
                return res.ToList();
            }
            else
            {
                var query = process ? @"SELECT ii.*
FROM public.""ImportacaoImovelVistaSoft"" ii
inner join ""ImportacaoBairroVistaSoft"" ibi on ibi.id = ii.""idImportacaoBairro""
inner join ""IntegracaoBairroVistaSoft"" ib on ib.id = ibi.""idIntegracaoBairro""
WHERE ii.status != 'Concluido' and ib.""idIntegracao"" = @idIntegracao" :
    @"SELECT ii.*
FROM public.""ImportacaoImovelVistaSoft"" ii
inner join ""ImportacaoBairroVistaSoft"" ibi on ibi.id = ii.""idImportacaoBairro""
inner join ""IntegracaoBairroVistaSoft"" ib on ib.id = ibi.""idIntegracaoBairro""
WHERE ii.status != 'Concluido' AND ii.status != 'Processando' and ib.""idIntegracao"" = @idIntegracao";

                var res = (await _conn.ExecuteQueryAsync<ImportacaoImovelVistaSoft>(query, new { idIntegracao = id })).ToList();
                return res;
            }
        }

        internal async Task<IntegracaoVistaSoft?> GetIntegracaoImportacaoBairro(int idImportBairro)
        {
            const string queryIntegracaoPorImportacaoId = "SELECT i.* FROM \"ImportacaoBairroVistaSoft\" ibi inner join \"IntegracaoBairroVistaSoft\" ib on ibi.\"idIntegracaoBairro\" = ib.id inner join \"IntegracaoVistaSoft\" i on ib.\"idIntegracao\" = i.id where ibi.id = @idImportBairro";
            var integracao = (await _conn.ExecuteQueryAsync<IntegracaoVistaSoft>(queryIntegracaoPorImportacaoId, new { idImportBairro })).FirstOrDefault();
            return integracao;
        }

        internal async Task<DateTime?> ObterUltimaAtualizacao()
        { 
            return await _conn.ExecuteScalarAsync<DateTime>("SELECT max(\"dataAtualizacao\") FROM \"ImportacaoImovelVistaSoft\";");
        }

        internal async Task<List<IntegracaoComboDTO>> GetIntegracoes()
        {
            const string queryIntegracoesCombo = "SELECT i.id as \"Integracao\", p.nome as \"Cliente\"  FROM \"IntegracaoVistaSoft\" i INNER JOIN \"Parceiro\"   p ON p.id = i.\"idCliente\"  ";
            var res = await _conn.ExecuteQueryAsync<IntegracaoComboDTO>(queryIntegracoesCombo);
            return res.ToList();
        }

        internal async Task<IntegracaoReportVS?> GetReportIntegracao(IntegracaoComboDTO integracao)
        {
            var idIntegracao = integracao.Integracao;
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
	'apiResponse', text(ii.""apiResponse"")
	) )
						FROM ""ImportacaoBairroVistaSoft"" ibi inner join ""ImportacaoImovelVistaSoft"" ii on ii.""idImportacaoBairro"" = ibi.id
                        WHERE ibi.""idIntegracaoBairro"" = ib.""id"" and ii.status = 'Concluido')      
        ))
    ) 
    FROM ""IntegracaoBairroVistaSoft"" ib 
    WHERE ib.""idIntegracao"" = i.id))
FROM 
    public.""IntegracaoVistaSoft"" i 
    INNER JOIN ""Parceiro"" p ON p.id = i.""idCliente"" 
    INNER JOIN ""Plano"" pl ON pl.id = i.""idPlano""
WHERE i.id = @idIntegracao;";
            var reportStr = (await _conn.ExecuteQueryAsync<string>(queryReport, new { idIntegracao })).FirstOrDefault();
            var report = JsonConvert.DeserializeObject<IntegracaoReportVS>(reportStr);
            return report;
        }

        internal async Task<IntegracaoVistaSoft?> GetIntegracaoById(int id)
        {
            var res = await _conn.QueryAsync<IntegracaoVistaSoft>(i => i.Id == id);
            return res.FirstOrDefault();
        }

        internal async Task<bool> UpdateIntegracao(IntegracaoVistaSoft integracao)
        {
            var fields = Field.Parse<IntegracaoVistaSoft>(e => new { e.UrlApi, e.ChaveApi });
            await _conn.UpdateAsync<IntegracaoVistaSoft>(integracao, fields);
            return true;
        }
    }
}
