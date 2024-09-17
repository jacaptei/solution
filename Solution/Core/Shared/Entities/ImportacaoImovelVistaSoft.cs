using NpgsqlTypes;

using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Npgsql;

namespace JaCaptei.Model.Entities;

[Map("ImportacaoImovelVistaSoft")]
public class ImportacaoImovelVistaSoft : IImportacaoImovel
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idImportacaoBairro")]
    public int IdImportacaoBairro { get; set; }

    [Map("idImovel")]
    public int IdImovel { get; set; }
    [Map("codImovel")]
    public string CodImovel { get; set; }

    [Map("requestBody")]
    [NpgsqlDbType(NpgsqlDbType.Jsonb)]
    public string RequestBody { get; set; }

    [Map("status")]
    public string Status { get; set; }

    [Map("dataInclusao")]
    public DateTime DataInclusao { get; set; }

    [Map("apiResponse")]
    [NpgsqlDbType(NpgsqlDbType.Jsonb)]
    public string ApiResponse { get; set; }

    [Map("dataAtualizacao")]
    public DateTime DataAtualizacao { get; set; }
}