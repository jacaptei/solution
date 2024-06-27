namespace JaCaptei.Model.Entities;

using RepoDb.Attributes;

public class ImovelAreas
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idImovel")]
    public int IdImovel { get; set; }

    [Map("minima")]
    public float Minima { get; set; } = 0;

    [Map("maxima")]
    public float Maxima { get; set; } = 0;

    [Map("interna")]
    public float Interna { get; set; } = 0;

    [Map("externa")]
    public float Externa { get; set; } = 0;

    [Map("total")]
    public float Total { get; set; } = 0;
}

