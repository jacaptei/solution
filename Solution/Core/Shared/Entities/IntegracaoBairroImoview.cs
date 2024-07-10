using NpgsqlTypes;

using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Npgsql;

namespace JaCaptei.Model.Entities;


[Map("IntegracaoBairroImoview")]
public class IntegracaoBairroImoview
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idIntegracao")]
    public int IdIntegracao { get; set; }

    [Map("idOperador")]
    public int IdOperador { get; set; }

    [Map("dataInclusao")]
    public DateTime DataInclusao { get; set; }

    [Map("dataAtualizacao")]
    public DateTime? DataAtualizacao { get; set; }

    [Map("idPlano")]
    public int IdPlano { get; set; }

    [Map("status")]
    public string Status { get; set; }

    [Map("bairro")]
    [NpgsqlDbType(NpgsqlDbType.Jsonb)]
    public string Bairro { get; set; }

    [Map("idCidade")]
    public int IdCidade { get; set; }

    [Map("idBairro")]
    public int IdBairro { get; set; }
}
