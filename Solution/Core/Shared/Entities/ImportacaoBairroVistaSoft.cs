using NpgsqlTypes;

using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Npgsql;

namespace JaCaptei.Model.Entities;

[Map("ImportacaoBairroVistaSoft")]
public class ImportacaoBairroVistaSoft : IImportacaoBairro
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idIntegracaoBairro")]
    public int IdIntegracaoBairro { get; set; }

    [Map("idOperador")]
    public int IdOperador { get; set; }

    [Map("dataInclusao")]
    public DateTime DataInclusao { get; set; }

    [Map("idPlano")]
    public int IdPlano { get; set; }

    [Map("status")]
    public string Status { get; set; }

    [Map("imoveis")]
    [NpgsqlDbType(NpgsqlDbType.Jsonb)]
    public string Imoveis { get; set; }
}