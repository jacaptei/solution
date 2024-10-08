using NpgsqlTypes;

using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Npgsql;

namespace JaCaptei.Model.Entities
{
    [Map("IntegracaoImoview")]
    public class IntegracaoImoview : IIntegracaoCRM
    {
        [Map("id")]
        public int Id { get; set; }

        [Map("idCliente")]
        public int IdCliente { get; set; }

        [Map("idOperador")]
        public int IdOperador { get; set; }

        [Map("dataInclusao")]
        public DateTime DataInclusao { get; set; }

        [Map("dataAtualizacao")]
        public DateTime? DataAtualizacao { get; set; }

        [Map("codUsuario")]
        public string CodUsuario { get; set; }

        [Map("codUnidade")]
        public string CodUnidade { get; set; }

        [Map("chaveApi")]
        public string ChaveApi { get; set; }

        [Map("idPlano")]
        public int? IdPlano { get; set; }

        [Map("status")]
        public string Status { get; set; }

        [Map("bairros")]
        [NpgsqlDbType(NpgsqlDbType.Jsonb)]
        public string Bairros { get; set; } 

        [Map("imoveis")]
        [NpgsqlDbType(NpgsqlDbType.Jsonb)]
        public string Imoveis { get; set; }
    }
}
