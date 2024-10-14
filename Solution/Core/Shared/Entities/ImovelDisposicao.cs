namespace JaCaptei.Model.Entities;

using RepoDb.Attributes;

public class ImovelDisposicao
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idImovel")]
    public int IdImovel { get; set; }

    [Map("aceitaFinanciamento")]
    public bool AceitaFinanciamento { get; set; } = false;

    [Map("aceitaPermuta")]
    public bool AceitaPermuta { get; set; } = false;

    [Map("alugado")]
    public bool Alugado { get; set; } = false;

    [Map("comissao")]
    public decimal Comissao { get; set; } = 0m;

    [Map("desativado")]
    public bool Desativado { get; set; } = false;

    [Map("disponivel")]
    public bool Disponivel { get; set; } = false;

    [Map("gestaoJacaptei")]
    public bool GestaoJacaptei { get; set; } = false;

    [Map("gestaoPremium")]
    public bool GestaoPremium { get; set; } = false;

    [Map("naPlanta")]
    public bool NaPlanta { get; set; } = false;

    [Map("placa")]
    public bool Placa { get; set; } = false;

    [Map("ocupado")]
    public bool Ocupado { get; set; } = false;

    [Map("vendido")]
    public bool Vendido { get; set; } = false;
}
