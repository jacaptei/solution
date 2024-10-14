namespace JaCaptei.Model.Entities;

using RepoDb.Attributes;

public class ImovelValores
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idImovel")]
    public int IdImovel { get; set; }

    [Map("anterior")]
    public float Anterior { get; set; } = 0;

    [Map("atual")]
    public float Atual { get; set; } = 0;

    [Map("condominio")]
    public float Condominio { get; set; } = 0;

    [Map("consulta")]
    public float Consulta { get; set; } = 0;

    [Map("maximo")]
    public float Maximo { get; set; } = 0;

    [Map("minimo")]
    public float Minimo { get; set; } = 0;

    [Map("iptu")]
    public float Iptu { get; set; } = 0;
}

