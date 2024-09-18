using JaCaptei.Model;
using JaCaptei.Model.DTO;
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
            const string queryImportPendentes = "SELECT ibi.* FROM public.\"IntegracaoImoview\" i inner join \"IntegracaoBairroImoview\" ib on i.id = ib.\"idIntegracao\" inner join \"ImportacaoBairroImoview\" ibi on ibi.\"idIntegracaoBairro\" = ib.id where i.id = @idIntegracao and ibi.status <> @status;";
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
                var fields = Field.Parse<IntegracaoVistaSoft>(e => new { e.Status, e.DataAtualizacao, e.Bairros });
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
    }
}
