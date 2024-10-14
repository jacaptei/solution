namespace JaCaptei.Model.Entities;

using RepoDb.Attributes;

public class ImovelLazer
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idImovel")]
    public int IdImovel { get; set; }

    [Map("cinema")]
    public bool Cinema { get; set; } = false;

    [Map("hidromassagem")]
    public bool Hidromassagem { get; set; } = false;

    [Map("piscina")]
    public bool Piscina { get; set; } = false;

    [Map("playground")]
    public bool Playground { get; set; } = false;

    [Map("quadraPoliesportiva")]
    public bool QuadraPoliesportiva { get; set; } = false;

    [Map("quadraTenis")]
    public bool QuadraTenis { get; set; } = false;

    [Map("salaoFestas")]
    public bool SalaoFestas { get; set; } = false;

    [Map("salaoJogos")]
    public bool SalaoJogos { get; set; } = false;

    [Map("salaoMassagem")]
    public bool SalaoMassagem { get; set; } = false;
}

