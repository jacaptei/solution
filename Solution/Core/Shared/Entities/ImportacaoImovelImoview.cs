using NpgsqlTypes;

using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Npgsql;

namespace JaCaptei.Model.Entities;

[Map("ImportacaoImovelImoview")]
public class ImportacaoImovelImoview
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

    [Map("imagens")]
    [NpgsqlDbType(NpgsqlDbType.Jsonb)]
    public string Imagens { get; set; }

    [Map("status")]
    public string Status { get; set; }

    [Map("dataInclusao")]
    public DateTime DataInclusao { get; set; }

    [Map("imoviewResponse")]
    [NpgsqlDbType(NpgsqlDbType.Jsonb)]
    public string ImoviewResponse { get; set; }

    [Map("dataAtualizacao")]
    public DateTime DataAtualizacao { get; set; }
}