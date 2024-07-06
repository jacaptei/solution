namespace JaCaptei.Model.Entities;

using RepoDb.Attributes;

public class ImovelAreas
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idImovel")]
    public int IdImovel { get; set; }

    [Map("minima")]
    public Single Minima { get; set; } = 0;

    [Map("maxima")]
    public Single Maxima { get; set; } = 0;

    [Map("interna")]
    public Single Interna { get; set; } = 0;

    [Map("externa")]
    public Single Externa { get; set; } = 0;

    [Map("total")]
    public Single Total { get; set; } = 0;
}

