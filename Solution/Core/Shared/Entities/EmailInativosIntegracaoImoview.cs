using NpgsqlTypes;

using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.Npgsql;

namespace JaCaptei.Model.Entities;

[Map("EmailInativosIntegracaoImoview")]
public class EmailInativosIntegracaoImoview
{
    [Map("id")]
    public int Id { get; set; }

    [Map("status")]
    public string Status { get; set; }

    [Map("dataEnvio")]
    public DateTime DataEnvio { get; set; } = DateTime.Now;
    
    [Map("idIntegracao")]
    public int IdIntegracao { get; set; }

    [Map("mensagem")]
    public string Mensagem { get; set; }

    [Map("imoveis")]
    [NpgsqlDbType(NpgsqlDbType.Jsonb)]
    public string Imoveis { get; set; } 
}